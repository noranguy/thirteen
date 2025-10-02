using Godot;
using System;

public partial class Homebase : Node2D
{
	private CharacterBody2D player;
	private int area_in = 0; // int to track what interactable area the player is in to try and avoid a thousand bool flags
	private string[] area_scenes = ["Nowhere", "table", "shop"];
	// 0 - Not in any meaningful area
	// 1 - Table Area - maybe deckbuilding area it dont really matter
	// 2 - Shop Area

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<PlayerCharacter>("PlayerCharacter");
		player.Position = GlobalState.Instance.GetPlayerLoadPosition();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// If the player is trying to interact with something
		if (Input.IsActionJustPressed("interact"))
		{
			if(area_in != 0){	
				ChangeScene();
				
			}
		}
	}

	void _on_shop_area_body_entered(Node2D body){
		area_in = 2;
	}

	void _on_shop_area_body_exited(Node2D body){
		area_in = 0;
	}

	// Determines if the player is allowed to change scenes. If they are send them to area corresponding to area_in
	void ChangeScene(){
		GlobalState.Instance.SetPlayerLoadPosition(player.Position);
		SceneLoader.Instance.ChangeToScene(area_scenes[area_in]);
	}
}
