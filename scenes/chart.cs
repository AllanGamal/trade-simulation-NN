using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class chart : Node2D
{
	private string title;
	private List<List<float>> multiValues = new List<List<float>>();
	private int maxPoints = 10000;
	private Vector2 graphSize = new Vector2(320, 250);
	private Vector2 offset = new Vector2(1280, 0);
	private float maxY = float.MinValue;
	private float minY = float.MaxValue;

	public delegate float UpdateValueDelegate();
	public List<UpdateValueDelegate> GetValues = new List<UpdateValueDelegate>();

	private Font font = ResourceLoader.Load<Font>("res://misc/fonts/cmu-typewriter/Typewriter/cmuntb.ttf");

	public Vector2 Offset
	{
		get { return offset; }
		set { offset = value; }
	}

	public string Title
	{
		get { return title; }
		set { title = value; }
	}

	public override void _Draw()
	{
		if (multiValues.Count == 0 || multiValues[0].Count == 0) return;

		UpdateMinMax();

		DrawAxes();
		DrawGraph();
		DrawTitle();
	}

	private void UpdateMinMax()
	{
		maxY = multiValues.SelectMany(list => list).Max();
		minY = multiValues.SelectMany(list => list).Min();
		if (maxY == minY)
		{
			maxY += 1;
			minY -= 1;
		}
	}

	private void DrawAxes()
	{
		DrawLine(offset + new Vector2(0, graphSize.Y), offset + new Vector2(graphSize.X, graphSize.Y), Colors.White);
		DrawLine(offset + Vector2.Zero, offset + new Vector2(0, graphSize.Y), Colors.White);
	}

	private void DrawGraph()
	{
		Color[] colors = { Colors.AliceBlue, Colors.Red, Colors.Green };
		for (int lineIndex = 0; lineIndex < multiValues.Count; lineIndex++)
		{
			var values = multiValues[lineIndex];
			int startIndex = Math.Max(0, values.Count - maxPoints);
			int visiblePoints = values.Count - startIndex;
			for (int i = 0; i < visiblePoints - 1; i++)
			{
				Vector2 start = ScalePoint(i, visiblePoints, lineIndex);
				Vector2 end = ScalePoint(i + 1, visiblePoints, lineIndex);
				DrawLine(offset + start, offset + end, colors[lineIndex % colors.Length], 3);
			}
			if (visiblePoints > 0)
			{
				Vector2 lastPoint = ScalePoint(visiblePoints - 1, visiblePoints, lineIndex);
				DrawCircle(offset + lastPoint, 2, colors[lineIndex % colors.Length]);
			}
		}
	}

	private Vector2 ScalePoint(int index, int visiblePoints, int lineIndex)
	{
		float x = index * graphSize.X / (visiblePoints + 9);
		float y = graphSize.Y - ((multiValues[lineIndex][multiValues[lineIndex].Count - visiblePoints + index] - minY) / (maxY - minY) * graphSize.Y);
		return new Vector2(x, y);
	}

	private void DrawTitle()
	{
		if (!string.IsNullOrEmpty(title))
		{
			DrawString(font, offset + new Vector2(0, graphSize.Y + 13), title,
				HorizontalAlignment.Center, 150, 17);
		}

		for (int i = 0; i < GetValues.Count; i++)
		{
			Color color;
			if (GetValues[i] != null)
			{
				if (i == 0)
				{
					color = Colors.Green;
				}
				else if (i == 1)
				{
					color = Colors.AliceBlue;
				}
				else
				{
					color = Colors.Red;
				}
				float value = GetValues[i]();
				DrawString(font, offset + new Vector2(140, graphSize.Y + 15 + (i * 20)), value.ToString("F2"),
					HorizontalAlignment.Center, 150, 15, color);
			}
		}
	}


	public void UpdateGraph(params float[] newValues)
	{
		while (multiValues.Count < newValues.Length)
		{
			multiValues.Add(new List<float>());
		}
		for (int i = 0; i < newValues.Length; i++)
		{
			multiValues[i].Add(newValues[i]);
		}
		QueueRedraw();
	}
}
