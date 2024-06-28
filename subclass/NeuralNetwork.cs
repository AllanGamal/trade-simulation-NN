using System.Collections.Generic;
using Godot;

public class NeuralNetwork
{
    private Layer[] layers;
    private List<double[]> allLayerOutputs;

    public Layer[] Layers {
        get => layers;
    }

    public List<double[]> AllLayerOutputs {
        get => allLayerOutputs;
    }

    public NeuralNetwork(params int[] layersSized)
    {
        layers = new Layer[layersSized.Length - 1];
        allLayerOutputs = new List<double[]>();  // Initialisera allLayerOutputs
        for (int i = 0; i < layersSized.Length - 1; i++)
        {
            layers[i] = new Layer(layersSized[i], layersSized[i + 1]);
        }
    }

    public double[] CalculateOutputs(double[] inputs)
    {
        allLayerOutputs.Clear();  
        allLayerOutputs.Add((double[])inputs.Clone());
        foreach (Layer layer in layers)
        {
            inputs = layer.CalculateOutputs(inputs);
            allLayerOutputs.Add((double[])inputs.Clone());
        }
        
        return inputs;
    }
}