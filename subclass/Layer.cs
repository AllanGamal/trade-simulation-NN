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
		this.mutationRate = 0.25f;

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
	for (int i = 0; i < numInputsNodes; i++)
	{
		for (int j = 0; j < numOutputNodes; j++)
		{
			weights[i, j] = random.NextDouble() * 2 - 1; // Initialize weights between -1 and 1
		}
	}

	for (int j = 0; j < numOutputNodes; j++)
	{
		biases[j] = random.NextDouble() * 2 - 1; // Initialize biases between -1 and 1
	}
}

	private static readonly Random randy = new Random();

public void Mutate()
{
	for (int nodeOut = 0; nodeOut < numOutputNodes; nodeOut++)
	{
		for (int nodeIn = 0; nodeIn < numInputsNodes; nodeIn++)
		{
			float min = 1 - (float)mutationRate;
			float max = 1 + (float)mutationRate;
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
}
	public void CopyWeightsFrom(Layer other)
{
	if (this.numInputsNodes != other.numInputsNodes || this.numOutputNodes != other.numOutputNodes)
	{
		throw new ArgumentException("Layers must have the same dimensions to copy weights.");
	}

	// Copy weights
	for (int i = 0; i < numInputsNodes; i++)
	{
		for (int j = 0; j < numOutputNodes; j++)
		{
			this.weights[i, j] = other.weights[i, j];
		}
	}

	// Copy biases
	for (int i = 0; i < numOutputNodes; i++)
	{
		this.biases[i] = other.biases[i];
	}
}
}
