using Godot;
using System;
using System.Threading.Tasks;

public enum Location {
	Player,
	Enemy,
	Table
}

public partial class Card : Control, IComparable<Card> {
	[Signal]
	public delegate void CardClickedEventHandler(Card card);
	
	public static readonly Vector2 SIZE = new Vector2(92, 128);
	private static readonly Vector2[] DEFAULT_VERTICES = new Vector2[] {
		new Vector2(0, 0),
		new Vector2(SIZE.X, 0),
		new Vector2(SIZE.X, SIZE.Y),
		new Vector2(0, SIZE.Y)
	};
	
	public bool locked = false;
	
	public string rank;
	public string suit;
	public Location location;
	
	public bool visible;
	public bool isPlayer;
	public Polygon2D sprite;
	
	public Tween tween;
	public Vector2 upperPosition;
	public Vector2 lowerPosition;
	
	public bool hoveringCard = false;
	
	public override void _Ready() {
		Connect("mouse_entered", new Callable(this, nameof(OnMouseEntered)));
		Connect("mouse_exited", new Callable(this, nameof(OnMouseExited)));
		
		sprite = GetNode<Polygon2D>("CardImage");
	}
	
	public override void _Process(double delta) {
		if (hoveringCard) {
			Highlight();
		} else {
			Unhighlight();
		}
	}
	
	public void Init(string rank, string suit, Location location, Vector2? scaleN = null, Vector2[] vertices = null) {
		Vector2 scale = scaleN ?? Vector2.One;
		vertices ??= DEFAULT_VERTICES;
		
		sprite = GetNode<Polygon2D>("CardImage");
		
		var collisionPolygon = GetNode<CollisionPolygon2D>("Area2D/CollisionPolygon2D");
		this.rank = rank;
		this.suit = suit;
		this.location = location;
		this.visible = location != Location.Enemy;
		this.isPlayer = location == Location.Player;
		
		sprite.UV = DEFAULT_VERTICES;
		collisionPolygon.Polygon = sprite.Polygon = vertices;
		
		CustomMinimumSize = SIZE * scale;
		PivotOffset = SIZE / 2f;
		Scale = new Vector2(1 / 3f, 1 / 3f);
		Position = new Vector2(-220, -50);
		
		upperPosition = Position + new Vector2(0, -10);
		lowerPosition = Position;
		
		UpdateTexture();
	}
	
	public async Task UpdatePosition(Vector2 position, Vector2 scale) {
		tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", position, 0.1f);
		tween.TweenProperty(this, "scale", scale, 0.1f);
		await ToSignal(tween, "finished");
	}
	
	public async Task UpdatePosition(Vector2 position) {
		tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", position, 0.1f);
		await ToSignal(tween, "finished");
	}
	
	public async Task SwapPositions(Card other) {
		Vector2 thisPosition = GlobalPosition;
		Vector2 otherPosition = other.GlobalPosition;
		
		tween = GetTree().CreateTween();
		tween.TweenProperty(this, "global_position", otherPosition, 0.25f);
		
		other.tween = GetTree().CreateTween();
		other.tween.TweenProperty(other, "global_position", thisPosition, 0.25f);
		
		await ToSignal(tween, "finished");
		await ToSignal(other.tween, "finished");
	}
	
	public void UpdateTexture() {
		string fileName = visible ? $"{rank}_{suit}" : "back";
		var texture = GD.Load<Texture2D>($"res://assets/cards/{fileName}.png");
		sprite.Texture = texture;
	}
	
	public void OnInputEvent(Node viewport, InputEvent @event, int shapeIdx) {
		_GuiInput(@event);
	}
	
	public override void _GuiInput(InputEvent @event) {
		if (
			@event is InputEventMouseButton mouseEvent &&
			mouseEvent.Pressed &&
			mouseEvent.ButtonIndex == MouseButton.Left &&
			GlobalState.Instance.allowCardSelect
		) {
			EmitSignal(SignalName.CardClicked, this);
		}
	}
	
	public void Highlight() {
		if (!isPlayer || !GlobalState.Instance.allowCardSelect) return;
		Shader shader = GD.Load<Shader>("res://shaders/card_highlight.gdshader");
		ShaderMaterial mat = new ShaderMaterial { Shader = shader };
		sprite.Material = mat;
		
		if (tween != null && tween.IsRunning()) return;

		tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", upperPosition, 0.05f);
	}
	
	public void Unhighlight() {
		if (!isPlayer || locked) return;
		sprite.Material = null;
		
		tween = GetTree().CreateTween();
		tween.TweenProperty(this, "position", lowerPosition, 0.05f);
	}

	public void OnMouseEntered() {
		hoveringCard = true;
	}
	
	public void OnMouseExited() {
		hoveringCard = false;
	}
	
	public int CompareTo(Card other) {
		if (other == null) return 1;
		
		int rankComparison = Array.IndexOf(GlobalState.Instance.RankOrder, this.rank)
			.CompareTo(Array.IndexOf(GlobalState.Instance.RankOrder, other.rank));
		
		if (rankComparison != 0) {
			return rankComparison;
		}
		
		return Array.IndexOf(GlobalState.Instance.SuitOrder, this.suit).CompareTo(Array.IndexOf(GlobalState.Instance.SuitOrder, other.suit));
	}
}
