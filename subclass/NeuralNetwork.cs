public class NeuralNetwork {
    Layer[] layers;


    public NeuralNetwork(params int[] layersSized) {
        layers = new Layer[layersSized.Length - 1];
        for (int i = 0; i < layersSized.Length - 1; i++) {
            layers[i] = new Layer(layersSized[i], layersSized[i + 1]);
        }
    }

    public double[] CalculateOutputs(double[] inputs) {
        foreach (Layer layer in layers) {
            inputs = layer.CalculateOutputs(inputs);
        }
        return inputs;
    }
}