

using Godot;
using System;

public partial class actionTimer : Node2D

{
	private Timer actionTimers;

	public override void _Ready()
	{
		
		

		// Start the timer
		actionTimers = GetNode<Timer>("ActionTimer");
		actionTimers.WaitTime = 1.0; // 1 sekund
		actionTimers.OneShot = false; // Upprepa timern
		actionTimers.Connect("timeout", new Callable(this, nameof(OnReadyTimerTimeout)));
		actionTimers.Start();
	}

	// process
	public void OnReadyTimerTimeout()
	{
		GD.Print("1 second has passed since _Ready was called");
		// Perform actions that should happen after the delay here
	}
}
