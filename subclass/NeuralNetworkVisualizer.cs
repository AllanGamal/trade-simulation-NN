using Godot;
using System;
using System.Collections.Generic;

public partial class NeuralNetworkVisualizer : Node2D
{
	private NeuralNetwork neuralNetwork;
	private List<List<Vector2>> nodePositions = new List<List<Vector2>>();
	private float nodeRadius = 9;
	private float layerSpacing = 250;
	private float nodeSpacing = 33;

	public NeuralNetwork NeuralNetwork
	{
		get => neuralNetwork;
	}

	public NeuralNetworkVisualizer(NeuralNetwork neuralNetwork)
	{
		this.neuralNetwork = neuralNetwork;
		CreateNodePositions();
	}

	private void CreateNodePositions()
	{
		var layers = neuralNetwork.Layers;
		float xOffset = 500; 

		for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
		{
			var layer = layers[layerIndex];
			int numNodes = layer.NumInputsNodes;
			float yOffset = (1805 - (numNodes - 1) * nodeSpacing) / 2; 

			var positions = new List<Vector2>();
			for (int nodeIndex = 0; nodeIndex < numNodes; nodeIndex++)
			{
				var position = new Vector2(xOffset, yOffset + nodeIndex * nodeSpacing);
				GD.Print("position: " + position.X + ", " + position.Y);
				positions.Add(position);
			}
			nodePositions.Add(positions);

			xOffset += layerSpacing;
		}

		// positions for output nodes
		int numOutputNodes = neuralNetwork.Layers[layers.Length - 1].NumOutputsNodes;
		float yOutputOffset = (1805 - (numOutputNodes - 1) * nodeSpacing) / 2; 
		var outputPositions = new List<Vector2>();
		for (int nodeIndex = 0; nodeIndex < numOutputNodes; nodeIndex++)
		{
			var position = new Vector2(xOffset, yOutputOffset + nodeIndex * nodeSpacing);
			outputPositions.Add(position);
		}
		nodePositions.Add(outputPositions);
	}

	public override void _Draw()
	{
		DrawConnections();
		DrawNodes();
	}

	private void DrawNodes()
	{
		foreach (var layerPositions in nodePositions)
		{
			foreach (var position in layerPositions)
			{
				DrawCircle(position, nodeRadius, new Color(0.5f, 0.5f, 0.5f));
			}
		}
	}

	private void DrawConnections()
	{
		for (int layerIndex = 0; layerIndex < nodePositions.Count - 1; layerIndex++)
		{
			var currentLayerPositions = nodePositions[layerIndex];
			var nextLayerPositions = nodePositions[layerIndex + 1];

			foreach (var startPosition in currentLayerPositions)
			{
				foreach (var endPosition in nextLayerPositions)
				{
					DrawLine(startPosition, endPosition, new Color(0.8f, 0.8f, 0.8f), 2);
				}
			}
		}
	}
}
