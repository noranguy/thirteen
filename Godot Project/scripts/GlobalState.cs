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

	// in play stuff
	
	public bool allowCardSelect = true;
	
	public readonly Random Rand = new Random();
	
	
	public readonly string[] RankOrder = new string[] {
		"3", "4", "5", "6", "7", "8", "9", "10", "j", "q", "k", "a", "2"
	};
	
	public readonly string[] SuitOrder = new string[] {
		"s", "c", "d", "h"
	};
}
