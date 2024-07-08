using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class world : Node2D
{
	private chart cookedFishChart;
	private chart rawFishChart;
	private chart hungerChart;
	private chart shoppedTreeChart;
	private chart woodChart;
	private chart fishingHooksChart;
	private chart cookedFishChart2;
	private chart rawFishChart2;
	private chart hungerChart2;
	private chart shoppedTreeChart2;
	private chart woodChart2;
	private chart fishingHooksChart2;
	private chart populationSizeChart;
	 private float shoppedTreeRandom;
    private float woodRandom;
    private float fishingHooksRandom;
    private float rawFishRandom;
    private float cookedFishRandom;
	private int count;
	private int highestCount = 0;

	private int iteration = 0;
	private int iterationsSinceLastGraphUpdate = 0;

	private Timer actionTimer;


	// spawn a blobly at a random location
	public void spawn_blobly()
	{
		SpawnNewBloblyMode();
	}

	private void SpawnNewBloblyMode()
	{
		string path = "agent/blobly.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);
		for (int i = 0; i < 999; i++)
		{
			blobly blobly = packedScene.Instantiate<blobly>();
			CallDeferred("add_child", blobly);
		}
	
	}

	private void SpawnCopiesMode() {
		string path = "agent/blobly.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);
			for (int j = 0; j < 10; j++) {
				if (blobly.AllInstances.Count < 999)
				{
					blobly blobly = packedScene.Instantiate<blobly>();
					NeuralNetwork nn = Serialization.LoadNeuralNetworkFromJson("weights_and_biases/neural_network_data_" + j);
					blobly.NeuralNetwork = nn;
					CallDeferred("add_child", blobly);
					blobly childBlobly = CreateNewBlobly(blobly, true);
					CallDeferred("add_child", childBlobly);
				}
			}
	}

	public override void _Ready()
	{
		spawn_blobly();

		hungerChart = InstantiateThreeCharts(new Vector2(1281, 1), "Hunger", new Vector2(600, 250),
			blobly.GetAverageOfHungerOfOfHighestHalfForInitialScore, blobly.GetAverageHunger, blobly.GetAverageOfHungerOfLowestHalfForInitialScore);
		fishingHooksChart = InstantiateThreeCharts(new Vector2(1883, 1), "Fishing Hooks",new Vector2(600, 250),
			blobly.GetAverageOfFishingHooksOfOfHighestHalfForInitialScore, blobly.GetAverageFishingHooks, blobly.GetAverageOfFishingHooksOfLowestHalfForInitialScore);
		cookedFishChart = InstantiateThreeCharts(new Vector2(1281, 350), "Cooked Fish",new Vector2(600, 250),
			blobly.GetAverageOfCookedFishOfOfHighestHalfForInitialScore, blobly.GetAverageCookedFish, blobly.GetAverageOfCookedFishOfLowestHalfForInitialScore);
		rawFishChart = InstantiateThreeCharts(new Vector2(1883, 350), "Raw Fish",new Vector2(600, 250),
			blobly.GetAverageOfRawFishOfOfHighestHalfForInitialScore, blobly.GetAverageRawFish, blobly.GetAverageOfRawFishOfLowestHalfForInitialScore);
		shoppedTreeChart = InstantiateThreeCharts(new Vector2(1281, 700), "Chopped Trees",new Vector2(600, 250),
			blobly.GetAverageOfShoppedTreeOfOfHighestHalfForInitialScore, blobly.GetAverageShoppedTree, blobly.GetAverageOfShoppedTreeOfLowestHalfForInitialScore);
		woodChart = InstantiateThreeCharts(new Vector2(1883, 700), "Wood", new Vector2(600, 250),
			blobly.GetAverageOfWoodOfOfHighestHalfForInitialScore, blobly.GetAverageWood, blobly.GetAverageOfWoodOfLowestHalfForInitialScore);

		hungerChart2 = InstantiateOneChart(new Vector2(1, 724), "Hunger", new Vector2(425, 125),
			blobly.GetAverageHunger2);
		fishingHooksChart2 = InstantiateOneChart(new Vector2(1, 870), "Fishing Hooks",new Vector2(425, 125),
			blobly.GetAverageFishingHooks2);
		cookedFishChart2 = InstantiateOneChart(new Vector2(428, 724), "Cooked Fish",new Vector2(425, 125),
			blobly.GetAverageCookedFish2);
		rawFishChart2 = InstantiateOneChart(new Vector2(428, 870), "Raw Fish",new Vector2(425, 125),
			blobly.GetAverageRawFish2);
		shoppedTreeChart2 = InstantiateOneChart(new Vector2(856, 724), "Chopped Trees",new Vector2(425, 125),
			blobly.GetAverageShoppedTree2);
		woodChart2 = InstantiateOneChart(new Vector2(856, 870), "Wood", new Vector2(425, 125),
			blobly.GetAverageWood2);	

		populationSizeChart = InstantiateOneChart(new Vector2(420, 580), "Population Size", new Vector2(425, 125),
			blobly.GetPopulationSize);

		// Configure and start the timer
		actionTimer = new Timer();
		actionTimer.WaitTime = 0.01; // 0.01 second
		actionTimer.OneShot = false; // Repeat timer
		actionTimer.Connect("timeout", new Callable(this, nameof(OnActionTimerTimeout)));
		AddChild(actionTimer);
		actionTimer.Start();
	}


	private chart InstantiateThreeCharts(Vector2 offset, string title, Vector2 chartSize,
		chart.UpdateValueDelegate getValueMethod1,
		chart.UpdateValueDelegate getValueMethod2,
		chart.UpdateValueDelegate getValueMethod3)
	{
		chart newChart = new chart(chartSize);
		newChart.Offset = offset;
		newChart.Title = title;
		newChart.GetValues.Add(getValueMethod1);
		newChart.GetValues.Add(getValueMethod2);
		newChart.GetValues.Add(getValueMethod3);
		AddChild(newChart);
		return newChart;
	}

	private chart InstantiateOneChart(Vector2 offset, string title, Vector2 chartSize, chart.UpdateValueDelegate getValueMethod)
	{
		chart newChart = new chart(chartSize);
		newChart.Offset = offset;
		newChart.Title = title;
		newChart.GetValues.Add(getValueMethod);
		AddChild(newChart);
		return newChart;
	}
	
	

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left_click"))
		{
			GD.Print("Left click pressed");
			UpdateAllGraphs();
		}
	}

	private void UpdateAllGraphs()
	{
		cookedFishChart.UpdateGraph(
			blobly.GetAverageCookedFish(),
			blobly.GetAverageOfCookedFishOfLowestHalfForInitialScore(),
			blobly.GetAverageOfCookedFishOfOfHighestHalfForInitialScore()
		);
		hungerChart.UpdateGraph(
			blobly.GetAverageHunger(),
			blobly.GetAverageOfHungerOfLowestHalfForInitialScore(),
			blobly.GetAverageOfHungerOfOfHighestHalfForInitialScore()
		);
		shoppedTreeChart.UpdateGraph(
			blobly.GetAverageShoppedTree(),
			blobly.GetAverageOfShoppedTreeOfLowestHalfForInitialScore(),
			blobly.GetAverageOfShoppedTreeOfOfHighestHalfForInitialScore()
		);
		woodChart.UpdateGraph(
			blobly.GetAverageWood(),
			blobly.GetAverageOfWoodOfLowestHalfForInitialScore(),
			blobly.GetAverageOfWoodOfOfHighestHalfForInitialScore()
		);
		fishingHooksChart.UpdateGraph(
			blobly.GetAverageFishingHooks(),
			blobly.GetAverageOfFishingHooksOfLowestHalfForInitialScore(),
			blobly.GetAverageOfFishingHooksOfOfHighestHalfForInitialScore()
		);
		rawFishChart.UpdateGraph(
			blobly.GetAverageRawFish(),
			blobly.GetAverageOfRawFishOfLowestHalfForInitialScore(),
			blobly.GetAverageOfRawFishOfOfHighestHalfForInitialScore()
		);
		cookedFishChart2.UpdateGraph(
			blobly.GetAverageCookedFish2()
		);
		hungerChart2.UpdateGraph(
			blobly.GetAverageHunger2()
		);
		shoppedTreeChart2.UpdateGraph(
			blobly.GetAverageShoppedTree2()
		);
		woodChart2.UpdateGraph(
			blobly.GetAverageWood2()
		);
		fishingHooksChart2.UpdateGraph(
			blobly.GetAverageFishingHooks2()
		);
		rawFishChart2.UpdateGraph(
			blobly.GetAverageRawFish2()
		);

		// population
		populationSizeChart.UpdateGraph(
			blobly.GetPopulationSize()
		);
	

	}

private void OnActionTimerTimeout()
{
		count++;		
		if (count % 1000 == 0)
		{
			GD.Print("---------------------------");
			GD.Print("Fitness:: " + count);
			
		}
	// perform action for each blobly
	foreach (var blobly in blobly.AllInstances.ToArray())
	{
		blobly.PerformNeuralNetworkAction(count);
	}

	if (!blobly.IsAnyBloblyWithInitialScore() || count > 2000)
	{
		
		GD.Print("----------------------------");
		if (count > highestCount){
			highestCount=count;
		}
		if (count > 2000) {
		GD.Print("Fitness above 2000!: " + count);
		if (blobly.Res > 2.53f){
			
		blobly.Res = blobly.Res*0.990f;
		}
		//blobly.Res = blobly.Res*0.994f;
		blobly.Res = blobly.Res*1;
		highestCount = 0;
	
	}
		iteration++;
		GD.Print("Iteration: " + iteration);
		GD.Print("Res: " + blobly.Res);
		GD.Print("Fitness: " + count);
		GD.Print("Highest Fitness: " + highestCount);
		count = 0;

		ReproduceTopAndRemoveOthers();
	}
	
	if (blobly.AllInstances.Count > 0)
	{
		iterationsSinceLastGraphUpdate++;
		if (iterationsSinceLastGraphUpdate >= 10)
		{
			UpdateAllGraphs();
			
			iterationsSinceLastGraphUpdate = 0;
		}
	}
	else
	{
		GD.Print("No bloblys left. Stopping updates.");
		actionTimer.Stop();
	}
}

private void GenerateRandomValuesForIteration()
{
    Random randy = new Random();
    shoppedTreeRandom = randy.Next(50, 1000);
    woodRandom = randy.Next(50, 1000);
    fishingHooksRandom = randy.Next(50, 1000);
    rawFishRandom = randy.Next(50, 1000);
    cookedFishRandom = randy.Next(50, 1000);
}

private void ReproduceTopAndRemoveOthers()
{
	GenerateRandomValuesForIteration();
    blobly[] sortedBloblys = blobly.AllInstances.OrderByDescending(b => b.Score).ToArray();
    int halfPopulation = sortedBloblys.Length;

    
    var top30 = sortedBloblys.Take((int)(halfPopulation * 0.4f)).ToList();

    // remove all instances
    foreach (var blobly in blobly.AllInstances.ToList())
    {
        blobly.QueueFree();
    }
    blobly.AllInstances.Clear();

    Random rand = new Random();
    blobly.LastWinner = null;
    List<blobly> newPopulation = new List<blobly>();

    blobly lastWinner = CreateNewBlobly(top30[0], false);
	
	Serialization.SaveNeuralNetworkToJson(lastWinner.NeuralNetwork, "neural_network_data");
	// save top 10
	for (int i = 0; i < 10; i++)
	{
		Serialization.SaveNeuralNetworkToJson(top30[i].NeuralNetwork, "neural_network_data_" + i);
	}
    newPopulation.Add(lastWinner);
    blobly.LastWinner = lastWinner;
    bool isFirstIteration = true; // flag to check the first iteration

    while (newPopulation.Count < 1000)
    {
        if (isFirstIteration)
        {
            for (int i = 0; i < 7; i++)
            {
                blobly newBlobly = CreateNewBlobly(top30[0], true);
                newPopulation.Add(newBlobly);
            }
            isFirstIteration = false;
        }

        foreach (var parentBlobly in top30)
        {
            if (newPopulation.Count >= 1000) break;

            int index = top30.IndexOf(parentBlobly) + 1;
            double probability = 1.00 - ((index - 1) * 0.001); 

            if (rand.NextDouble() <= probability)
            {
                blobly newBlobly = CreateNewBlobly(parentBlobly, true);
                newPopulation.Add(newBlobly);
            }
        }
    }

    // remove excess if needed
    while (newPopulation.Count > 1000)
    {
        int indexToRemove = rand.Next(newPopulation.Count);
        blobly bloblyToRemove = newPopulation[indexToRemove];
        newPopulation.RemoveAt(indexToRemove);
        bloblyToRemove.QueueFree();
    }

    // add bloblys to scene
    foreach (var newBlobly in newPopulation)
    {
        CallDeferred("add_child", newBlobly);
    }
}
private blobly CreateNewBlobly(blobly parentBlobly, bool mutate)
{
    string path = "agent/blobly.tscn";
    PackedScene packedScene = GD.Load<PackedScene>(path);
    blobly newBlobly = packedScene.Instantiate<blobly>();

    // set initial values
    newBlobly.Hunger = 100;
    newBlobly.Shopped_tree = 0;
    newBlobly.Wood = 0;
    newBlobly.Fishing_hooks = 0;
    newBlobly.Raw_fish = 0;
    newBlobly.CookedFish = 80;
    newBlobly.Skill_cooking = 0;
    newBlobly.Skill_chopping_tree = 0;
    newBlobly.Skill_chopping_wood = 0;
    newBlobly.Skill_fishing = 0;
    newBlobly.Skill_craft_fishing_hooks = 0;

    // copy nn weights and biases 
    newBlobly.NeuralNetwork.CopyWeightsFrom(parentBlobly.NeuralNetwork, mutate);

    return newBlobly;
}

}
