using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public partial class Serialization
{
    public static void SaveNeuralNetworkToJson(NeuralNetwork neuralNetwork, string fileName)
    {
        var nnData = new Dictionary<string, object>();
        Layer[] layers = neuralNetwork.Layers;

        for (int i = 0; i < layers.Length; i++)
        {
            var layerData = new Dictionary<string, object>
            {
                ["weights"] = layers[i].Weights,
                ["biases"] = layers[i].Biases
            };
            nnData[$"layer_{i}"] = layerData;
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonConverterForMultidimensionalArrays() }
        };
        string jsonString = JsonSerializer.Serialize(nnData, options);

        string filePath = $"json/{fileName}.json";
        using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write))
        {
            file.StoreString(jsonString);
        }

        //GD.Print($"Neural network data saved to: {filePath}");
    }

    private class JsonConverterForMultidimensionalArrays : JsonConverter<double[,]>
    {
        public override double[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
{
    if (reader.TokenType != JsonTokenType.StartArray)
    {
        throw new JsonException();
    }

    var data = new List<List<double>>();
    while (reader.Read())
    {
        if (reader.TokenType == JsonTokenType.EndArray)
        {
            break;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        var row = new List<double>();
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            row.Add(reader.GetDouble());
        }
        data.Add(row);
    }

    var result = new double[data.Count, data[0].Count];
    for (int i = 0; i < data.Count; i++)
    {
        for (int j = 0; j < data[i].Count; j++)
        {
            result[i, j] = data[i][j];
        }
    }

    return result;
}

        public override void Write(Utf8JsonWriter writer, double[,] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            for (int i = 0; i < value.GetLength(0); i++)
            {
                writer.WriteStartArray();
                for (int j = 0; j < value.GetLength(1); j++)
                {
                    writer.WriteNumberValue(value[i, j]);
                }
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
        }
    }


    public static NeuralNetwork LoadNeuralNetworkFromJson(string fileName)
    {
        
        NeuralNetwork neuralNetwork = new NeuralNetwork(11, 64, 16, 8, 5);
        string filePath = $"{fileName}.json";



        try
        {
            string jsonString;
            using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
            {
                jsonString = file.GetAsText();
            }
    
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonConverterForMultidimensionalArrays() }
            };
    
            var nnData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, JsonElement>>>(jsonString, options);
    
            Layer[] layers = neuralNetwork.Layers;
            if (layers != null)
            {
                for (int i = 0; i < layers.Length; i++)
                {
                    if (nnData.TryGetValue($"layer_{i}", out var layerData))
                    {
                        layers[i].Weights = layerData["weights"].Deserialize<double[,]>(options);
                        layers[i].Biases = layerData["biases"].Deserialize<double[]>(options);
                    }
                    else
                    {
                        GD.Print($"Data for layer_{i} not found in kson.");
                    }
                }
            }
            else
            {
                GD.Print("Error: Neural network layers are null.");
            }
    
            //GD.Print($"Neural network data loaded from: {filePath}");
            return neuralNetwork;
        }
        catch (Exception e)
        {
            GD.PrintErr($"Error loading neural network data from {filePath}: {e.Message}");
            return null;
        }
    }
}