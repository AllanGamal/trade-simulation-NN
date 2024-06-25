using Godot;
using System;
using System.Collections.Generic;

public partial class Locations : Node
{

	public Godot.Vector2 get_position_lumberyard()
	{
		return new Godot.Vector2(new Random().Next(903, 1270), new Random().Next(440, 710));
	}

	public Godot.Vector2 get_position_workshop()
	{
		return new Godot.Vector2(new Random().Next(136, 294), new Random().Next(6, 55));
	}

	public Godot.Vector2 get_position_market()
	{	
		return new Godot.Vector2(new Random().Next(917, 1267),50);
	}

	public Godot.Vector2 get_position_kitchen()
	{	
		return new Godot.Vector2(24, new Random().Next(122, 344));
	}

	public Godot.Vector2 get_position_fishingLake()
	{	

		List<Godot.Vector2> positions = new List<Godot.Vector2>
        {
            new Godot.Vector2(new Random().Next(10, 174), new Random().Next(448, 496)),
            new Godot.Vector2(new Random().Next(149, 223), new Random().Next(494, 560)),
            new Godot.Vector2(new Random().Next(278, 325), new Random().Next(559, 630)),
            new Godot.Vector2(new Random().Next(317, 350), new Random().Next(637, 700))
        };

		return positions[new Random().Next(0, 4)];

	}




	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
