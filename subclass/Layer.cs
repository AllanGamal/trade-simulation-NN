using Godot;

public class Layer
{

    private int numInputsNodes;
    private int numOutputNodes;

    private double [,] weights;
    private double [] biases;

    public Layer(int numInputsNodes, int numOutputNodes)
    {
        this.numInputsNodes = numInputsNodes;
        this.numOutputNodes = numOutputNodes;

        weights = new double[numInputsNodes, numOutputNodes];
        biases = new double[numOutputNodes];
    }

    public double[] CalculateOutputs(double[] inputs) {
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
    double ActivationFunction(double weightedInput)
    {
        return 1.0 / (1.0 + System.Math.Exp(-weightedInput));
    }

    public static void Main(string[] args)
    {
        

    }



}