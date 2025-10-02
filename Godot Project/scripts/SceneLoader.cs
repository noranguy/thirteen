using Godot;
using System;

//Node used to change scenes
//Call the node's ChangeToScene function to change scenes
public partial class SceneLoader : Node
{
	public static SceneLoader Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public void ChangeToScene(string sceneName)
	{
		GetTree().ChangeSceneToFile($"res://scenes/{sceneName}.tscn");
	}
}
