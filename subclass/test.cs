using Godot;
using System;

public partial class Test : Node2D
{
	private bool _shouldDraw = false;
	private Vector2 _drawPosition = Vector2.Zero;
	private float _drawRadius = 0f;
	private Color _drawColor = new Color(0.5f, 0.5f, 0.5f);

	public void UpdateDrawNodes()
	{
		GD.Print("UpdateDrawNodes1111");

		// Ställ in parametrarna för ritningen
		_drawPosition = new Vector2(100, 200);
		_drawRadius = 50f; // Sätt en rimlig radie
		_drawColor = new Color(0.5f, 0.5f, 0.5f);

		// Sätt flaggan till true och anropa QueueRedraw för att tvinga omritning
		_shouldDraw = true;
		QueueRedraw();
	}

	public override void _Draw()
	{
		if (_shouldDraw)
		{
			DrawCircle(_drawPosition, _drawRadius, _drawColor);
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left_click"))
		{
			GD.Print("Left click pressed");
			UpdateDrawNodes();
		}
	}
}
