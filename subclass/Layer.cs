using System;
using Godot;

public class Layer
{
    private float mutationRate;
    private int numInputsNodes;
    private int numOutputNodes;

    private double[,] weights;
    private double[] biases;

    public Layer(int numInputsNodes, int numOutputNodes)
    {
        this.numInputsNodes = numInputsNodes;
        this.numOutputNodes = numOutputNodes;
        this.mutationRate = 0.1f;

        weights = new double[numInputsNodes, numOutputNodes];
        biases = new double[numOutputNodes];

        InitializeWeightsAndBiases();
    }

     public float MutationRate
    {
        get => mutationRate;
        set => mutationRate = value;
    }

    public int NumInputsNodes
    {
        get => numInputsNodes;
    }

    public int NumOutputsNodes
    {
        get => numOutputNodes;
    }

    public double[] CalculateOutputs(double[] inputs)
    {
        double[] activations = new double[numOutputNodes];

        for (int nodeOut = 0; nodeOut < numOutputNodes; nodeOut++)
        {
            double weightedInput = biases[nodeOut];

            for (int nodeIn = 0; nodeIn < numInputsNodes; nodeIn++)
            {
                weightedInput += inputs[nodeIn] * weights[nodeIn, nodeOut];
            }
            activations[nodeOut] = ActivationFunction(weightedInput);
        }
        return activations;
    }

    // Sigmoid
    public double ActivationFunction(double weightedInput)
    {
        return 1.0 / (1.0 + Math.Exp(-weightedInput));
    }

   private void InitializeWeightsAndBiases()
{
    Random random = new Random();
    double standardDeviation = Math.Sqrt(2.0 / numInputsNodes); // Xavier/Glorot initialization

    for (int i = 0; i < numInputsNodes; i++)
    {
        for (int j = 0; j < numOutputNodes; j++)
        {
            weights[i, j] = RandomGaussian(random, 0, standardDeviation);
        }
    }

    for (int j = 0; j < numOutputNodes; j++)
    {
        biases[j] = RandomGaussian(random, 0, standardDeviation); // Biases can also be initialized similarly
    }
}

// Function to generate Gaussian distributed random numbers
private double RandomGaussian(Random random, double mean, double stddev)
{
    // Using Box-Muller transform to generate a pair of independent standard normally distributed random numbers
    double u1 = 1.0 - random.NextDouble(); // uniform(0,1] random doubles
    double u2 = 1.0 - random.NextDouble();
    double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // random normal(0,1)
    return mean + stddev * randStdNormal; // random normal(mean,stdDev)
}

    public void Mutate()
    {
        Random randy = new Random();
        for (int nodeOut = 0; nodeOut < numOutputNodes; nodeOut++)
        {
            for (int nodeIn = 0; nodeIn < numInputsNodes; nodeIn++)
            {
				float min = 1-(float)mutationRate;
				float max = 1+(float)mutationRate;
                float multiplier = (float)(randy.NextDouble() * (max - min) + min);

                weights[nodeIn, nodeOut] *= multiplier;
                weights[nodeIn, nodeOut] = Math.Max(-1, Math.Min(1, weights[nodeIn, nodeOut])); // Clamping
            }
        }

        for (int i = 0; i < numOutputNodes; i++)
        {
            float min = 1 - (float)mutationRate;
            float max = 1 + (float)mutationRate;
            float multiplier = (float)(randy.NextDouble() * (max - min) + min);

            biases[i] *= multiplier;
            biases[i] = Math.Max(-1, Math.Min(1, biases[i])); // Clamping
        }

        float multiplier2 = (float)(randy.NextDouble() * (1.1 - 0.9) + 0.9);
        MutationRate *= multiplier2;
    }
}