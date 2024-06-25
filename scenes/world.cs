using Godot;
using System;

public partial class world : Node2D
{

	// spawn a blobly at a random location
	public void spawn_blobly()
	{
		string path = "agent/blobly.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);
		

		for (int i = 0; i < 49; i++)
		{
			blobly blobly = packedScene.Instantiate<blobly>();
			blobly.GlobalPosition = new Vector2(new Random().Next(10, 174), new Random().Next(448, 496));
			CallDeferred("add_child", blobly);
			

		}
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawn_blobly();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
