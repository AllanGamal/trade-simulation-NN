using Godot;
using System;
using System.Collections.Generic;

public partial class NeuralNetworkVisualizer : Node2D
{
    private NeuralNetwork neuralNetwork;
    private List<List<Vector2>> nodePositions = new List<List<Vector2>>();
    //private float nodeRadius = 9;
	//private float layerSpacing = 250;
    //private float nodeSpacing = 33;
    private float nodeRadius = 6;
    private float layerSpacing = 250;
    private float nodeSpacing = 10;

    private bool _shouldDraw = false;
    private bool _updateNodes = false;

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
        nodePositions.Clear();
        var layers = neuralNetwork.Layers;
		
        float xOffset = 50;

        for (int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
        {
            var layer = layers[layerIndex];
            int numNodes = (layerIndex == layers.Length - 1) ? layer.NumOutputsNodes : layer.NumInputsNodes;
            float yOffset = (1805 - (numNodes - 1) * nodeSpacing) / 2;

            var positions = new List<Vector2>();
            for (int nodeIndex = 0; nodeIndex < numNodes; nodeIndex++)
            {
                var position = new Vector2(xOffset, yOffset + nodeIndex * nodeSpacing);
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
        if (_updateNodes)
        {
            DrawUpdatedNodes();
            _updateNodes = false;
        }
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

    public void UpdateDrawNodes()
    {
        _updateNodes = true;
        QueueRedraw(); // För att säkerställa att _Draw anropas
    }

    private void DrawUpdatedNodes()
{
    double[] inputs = new double[neuralNetwork.Layers[0].NumInputsNodes];
    Array.Copy(inputs, inputs, inputs.Length);

    for (int layerIndex = 0; layerIndex < neuralNetwork.AllLayerOutputs.Count; layerIndex++)
    {
        double[] outputs = neuralNetwork.AllLayerOutputs[layerIndex];
        

        var layerPositions = nodePositions[layerIndex];

        for (int nodeIndex = 0; nodeIndex < outputs.Length; nodeIndex++)
        {
            var position = layerPositions[nodeIndex];
            float activation = nodeIndex < outputs.Length ? (float)outputs[nodeIndex] : 0.5f;

            // Apply an exponential scale to the activation to enhance differences
            activation = (float)Math.Pow(activation, 2);
            activation = Math.Max(0, Math.Min(1, activation));
            // avrunda till 2 decimaler
            activation = (float)Math.Round(activation, 1);

            // Determine node color based on activation
            Color nodeColor = new Color(activation, 0, 1 - activation);
            DrawCircle(position, nodeRadius, nodeColor);
        }

        Array.Copy(outputs, inputs, outputs.Length);
    }
}

    public void UpdateNeuralNetwork(NeuralNetwork newNeuralNetwork)
    {
        this.neuralNetwork = newNeuralNetwork;
        CreateNodePositions();
        UpdateDrawNodes();
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
                    DrawLine(startPosition, endPosition, new Color(0.8f, 0.8f, 0.8f), 1);
                }
            }
        }
    }
}