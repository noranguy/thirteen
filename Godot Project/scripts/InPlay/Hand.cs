using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Hand : Control {
	[Signal]
	public delegate void ActiveCardCheckEventHandler();
	
	public readonly int startingAmount = 13;
	private Curve hand_curve = GD.Load<Curve>($"res://curves/hand_y_curve.tres");
	private Curve rotation_curve = GD.Load<Curve>($"res://curves/hand_rotation_curve.tres");
	private int max_rotation_degrees = 10;
	private int x_sep = 0;
	
	public HashSet<Card> activeCards = new();
	public List<Card> cards = new();
	
	private int y_min;
	private int y_max;
	private Vector2 scaleV;
	private bool isPlayer;
	
	public void Init(List<(string, string)> cardInfo, int y, float scale, bool isPlayer) {
		SetMouseFilter(Control.MouseFilterEnum.Ignore);
		y_max = y;
		y_min = y + 100;
		Size = new Vector2(500 * scale, 100 * scale);
		scaleV = new Vector2(scale, scale);
		this.isPlayer = isPlayer;
		SpawnCards(cardInfo);
	}

	public async virtual void SpawnCards(List<(string, string)> cardInfo) {
		PackedScene cardScene = GD.Load<PackedScene>("res://components/card.tscn");
		foreach ((string rank, string suit) in cardInfo) {
			Card card = cardScene.Instantiate<Card>();
			card.Position = Vector2.Zero;
			card.ZIndex = 15;
			card.Init(rank, suit, isPlayer ? Location.Player : Location.Enemy);
			card.Connect(Card.SignalName.CardClicked, new Callable(this, nameof(OnCardClicked)));

			AddChild(card);
			cards.Add(card);
		}

		await UpdateCardPositions(true);
		if (!isPlayer) {
			foreach (Card card in cards) {
				card.ZIndex = 4;
			}
		}
	}
	
	public async virtual Task UpdateCardPositions(bool first) {
		int numCards = cards.Count;
		float cardSize = Card.SIZE.X * scaleV.X;
		if (numCards == 1) {
			Vector2 pos = new Vector2(-cardSize / 2f, cards[0].Position.Y);
			cards[0].UpdatePosition(pos);
			cards[0].RotationDegrees = 0;
			cards[0].upperPosition = pos + new Vector2(0, -10);
			cards[0].lowerPosition = pos;
			return;
		}
		
		float final_x_sep = (Size.X - cardSize * numCards) / (numCards - 1);
		float step = cardSize + final_x_sep;
		float centerOffset = -step * (numCards - 1) / 2f;
		
		for (int i = 0; i < numCards; i++) {
			Card card = cards[i];
			float y_multiplier, rot_multiplier;
			
			if (numCards > 1) {
				y_multiplier = hand_curve.Sample(1f / (numCards-1) * i);
				rot_multiplier = rotation_curve.Sample(1f / (numCards-1) * i);
			} else {
				y_multiplier = rot_multiplier = 0;
			}
			
			
			float centerX = centerOffset + step * i;
			float finalX = centerX - Card.SIZE.X / 2;
			
			float finalY = y_min - Size.Y * y_multiplier;
			Vector2 finalV = new Vector2(finalX, finalY);
			
			if (first) {
				card.visible = isPlayer;
				card.UpdateTexture();
				await card.UpdatePosition(finalV, scaleV);
			} else {
				card.UpdatePosition(finalV);
			}
			
			card.RotationDegrees = max_rotation_degrees * rot_multiplier;
			
			card.upperPosition = finalV + -10 * Vector2.FromAngle((card.RotationDegrees + 90) * (float)Math.PI / 180);
			card.lowerPosition = finalV;
		}
	}

	public async void RemoveCard(Card card) {
		if (cards.Contains(card)) {
			await card.UpdatePosition(card.Position + new Vector2(0, 40));
			cards.Remove(card);
			card.QueueFree();
			Size -= new Vector2(26.5f * scaleV.X, 0);
			UpdateCardPositions(false);
		}
	}
	
	public virtual void OnCardClicked(Card card) {
		if (!isPlayer) return;
		
		if (activeCards.Contains(card)) {
			card.locked = false;
			card.Unhighlight();
			activeCards.Remove(card);
		} else {
			card.locked = true;
			card.Highlight();
			activeCards.Add(card);
		}
		
		EmitSignal(SignalName.ActiveCardCheck);
	}
}
