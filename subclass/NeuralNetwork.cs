using System;
using System.Collections.Generic;
using Godot;

public class NeuralNetwork
{
    private Layer[] layers;
    private List<double[]> allLayerOutputs;

    public Layer[] Layers => layers;
    public List<double[]> AllLayerOutputs => allLayerOutputs;


    public NeuralNetwork(params int[] layersSized)
    {
        layers = new Layer[layersSized.Length - 1];
        allLayerOutputs = new List<double[]>();
        
        for (int i = 0; i < layersSized.Length - 1; i++)
        {
            ActivationType activationType;
            if (i == layersSized.Length - 2)  // last layer
            {
                activationType = ActivationType.Softmax;
            }
            else
            {
                activationType = ActivationType.ReLU;
            }
            
            layers[i] = new Layer(layersSized[i], layersSized[i + 1], activationType);
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

   
    public void CopyWeightsFrom(NeuralNetwork other, Boolean mutate)
    {
        if (this.layers.Length != other.layers.Length)
        {
            throw new System.ArgumentException("Neural networks must have the same structure to copy weights.");
        }

        for (int i = 0; i < this.layers.Length; i++)
        {
            this.layers[i].CopyWeightsFrom(other.layers[i]);
            if (mutate){
            this.layers[i].Mutate();
            }
        }
    }
  
}