using Godot;
using System;

public partial class GlobalState : Node
{
	// variables for determine where to load the player in after changning scenes
	private Vector2 playerLoadPosition = new Vector2(0, 0); // where in the homebase the player will load into
	
	public static GlobalState Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Vector2 GetPlayerLoadPosition(){
		return playerLoadPosition;
	}

	public void SetPlayerLoadPosition(Vector2 position){
		playerLoadPosition = position;
	}

}
