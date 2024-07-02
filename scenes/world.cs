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
	private chart populationSizeChart;
	private int count;
	private int highestCount = 0;

	private int iteration = 0;
	private int iterationsSinceLastGraphUpdate = 0;

	private Timer actionTimer;
	private NeuralNetworkVisualizer neuralNetworkVisualizer;

	// spawn a blobly at a random location
	public void spawn_blobly()
	{
		string path = "agent/blobly.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);

		for (int i = 0; i < 249; i++)
		{
			blobly blobly = packedScene.Instantiate<blobly>();
			CallDeferred("add_child", blobly);
		}

	}

	public override void _Ready()
	{
		spawn_blobly();
		


		hungerChart = InstantiateThreeCharts(new Vector2(1281, 1), "Hunger",
			blobly.GetAverageOfHungerOfTheHighest10Percent, blobly.GetAverageHunger, blobly.GetAverageOfHungerOfTheLowest10Percent);
		fishingHooksChart = InstantiateThreeCharts(new Vector2(1883, 1), "Fishing Hooks",
			blobly.GetAverageOfFishingHooksOfTheHighest10Percent, blobly.GetAverageFishingHooks, blobly.GetAverageOfFishingHooksOfTheLowest10Percent);
		cookedFishChart = InstantiateThreeCharts(new Vector2(1281, 350), "Cooked Fish",
			blobly.GetAverageOfCookedFishOfTheHighest10Percent, blobly.GetAverageCookedFish, blobly.GetAverageOfCookedFishOfTheLowest10Percent);
		rawFishChart = InstantiateThreeCharts(new Vector2(1883, 350), "Raw Fish",
			blobly.GetAverageOfRawFishOfTheHighest10Percent, blobly.GetAverageRawFish, blobly.GetAverageOfRawFishOfTheLowest10Percent);
		shoppedTreeChart = InstantiateThreeCharts(new Vector2(1281, 700), "Shopped Trees",
			blobly.GetAverageOfShoppedTreeOfTheHighest10Percent, blobly.GetAverageShoppedTree, blobly.GetAverageOfShoppedTreeOfTheLowest10Percent);
		woodChart = InstantiateThreeCharts(new Vector2(1883, 700), "Wood",
			blobly.GetAverageOfWoodOfTheHighest10Percent, blobly.GetAverageWood, blobly.GetAverageOfWoodOfTheLowest10Percent);
		
		// Configure and start the timer
		actionTimer = new Timer();
		actionTimer.WaitTime = 0.01; // 0.01 second
		actionTimer.OneShot = false; // Repeat timer
		actionTimer.Connect("timeout", new Callable(this, nameof(OnActionTimerTimeout)));
		AddChild(actionTimer);
		actionTimer.Start();
	}

	private chart InstantiateThreeCharts(Vector2 offset, string title,
		chart.UpdateValueDelegate getValueMethod1,
		chart.UpdateValueDelegate getValueMethod2,
		chart.UpdateValueDelegate getValueMethod3)
	{
		chart newChart = new chart();
		newChart.Offset = offset;
		newChart.Title = title;
		newChart.GetValues.Add(getValueMethod1);
		newChart.GetValues.Add(getValueMethod2);
		newChart.GetValues.Add(getValueMethod3);
		AddChild(newChart);
		return newChart;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left_click"))
		{
			GD.Print("Left click pressed");
			UpdateAllGraphs();
			neuralNetworkVisualizer.UpdateDrawNodes();
		}
	}

	private void UpdateAllGraphs()
	{
		cookedFishChart.UpdateGraph(
			blobly.GetAverageCookedFish(),
			blobly.GetAverageOfCookedFishOfTheLowest10Percent(),
			blobly.GetAverageOfCookedFishOfTheHighest10Percent()
		);
		hungerChart.UpdateGraph(
			blobly.GetAverageHunger(),
			blobly.GetAverageOfHungerOfTheLowest10Percent(),
			blobly.GetAverageOfHungerOfTheHighest10Percent()
		);
		shoppedTreeChart.UpdateGraph(
			blobly.GetAverageShoppedTree(),
			blobly.GetAverageOfShoppedTreeOfTheLowest10Percent(),
			blobly.GetAverageOfShoppedTreeOfTheHighest10Percent()
		);
		woodChart.UpdateGraph(
			blobly.GetAverageWood(),
			blobly.GetAverageOfWoodOfTheLowest10Percent(),
			blobly.GetAverageOfWoodOfTheHighest10Percent()
		);
		fishingHooksChart.UpdateGraph(
			blobly.GetAverageFishingHooks(),
			blobly.GetAverageOfFishingHooksOfTheLowest10Percent(),
			blobly.GetAverageOfFishingHooksOfTheHighest10Percent()
		);
		rawFishChart.UpdateGraph(
			blobly.GetAverageRawFish(),
			blobly.GetAverageOfRawFishOfTheLowest10Percent(),
			blobly.GetAverageOfRawFishOfTheHighest10Percent()
		);
	}
private int visualizationUpdateCounter = 0;
private const int VISUALIZATION_UPDATE_FREQUENCY = 10;
private void OnActionTimerTimeout()
{

		count++;		
	// Make each blobly perform an action
	foreach (var blobly in blobly.AllInstances.ToArray())
	{
		blobly.PerformRandomAction();
	}
	
	if (!blobly.IsHalfPopulationAboveMinimalHunger() || count > 2000)
	{
		
		GD.Print("----------------------------");
		if (count > highestCount){
			highestCount=count;
		}
		if (count > 2000) {
		GD.Print("Count above 2000!: " + count);
		if (blobly.Res > 2.53f){
			
		blobly.Res = blobly.Res*0.994f;
		}
		blobly.Res = blobly.Res*0.999f;
		highestCount = 0;
	
	}
	
		iteration++;
		GD.Print("Iteration: " + iteration);
		GD.Print("Res: " + blobly.Res);
		GD.Print("Count: " + count);
		GD.Print("Highest Count: " + highestCount);
		count = 0;
		
		
		


		ReproduceTopHalfAndRemoveOthers();
		
		// Uppdatera neural network visualizer med en ny blobly
		visualizationUpdateCounter++;
		if (visualizationUpdateCounter >= VISUALIZATION_UPDATE_FREQUENCY)
		{
			
			
			visualizationUpdateCounter = 0;
		}
	}
	

	// Kontrollera om det finns några bloblys kvar
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
private void ReproduceTopHalfAndRemoveOthers()
{
    blobly[] sortedBloblys = blobly.AllInstances.OrderByDescending(b => b.Hunger).ToArray();
    int halfPopulation = sortedBloblys.Length;

    // Keep the top 70%
    var top70 = sortedBloblys.Take((int)(halfPopulation * 0.5f)).ToList();
	// remove bottom half
	foreach (var blobly in sortedBloblys.Skip(halfPopulation))
	{
		blobly.QueueFree();
	}
	
	

    // Remove all instances
    foreach (var blobly in blobly.AllInstances.ToList())
    {
        blobly.QueueFree();
    }
    blobly.AllInstances.Clear();

    Random rand = new Random();
    List<blobly> newPopulation = new List<blobly>();

    while (newPopulation.Count < 250)
    {
        foreach (var parentBlobly in top70)
        {
            if (newPopulation.Count >= 250) break;

            int index = top70.IndexOf(parentBlobly) + 1;
            double probability = 1.0 - ((index - 1) * 0.001f); // Adjust probability as needed

                    

            if (rand.NextDouble() <= probability)
            {
                blobly newBlobly = CreateNewBlobly(parentBlobly);
                    
                    newPopulation.Add(newBlobly);
                    
                
            }
        }
    }

    // Remove excess if needed
    while (newPopulation.Count > 250)
    {
        int indexToRemove = rand.Next(newPopulation.Count);
        blobly bloblyToRemove = newPopulation[indexToRemove];
        newPopulation.RemoveAt(indexToRemove);
        bloblyToRemove.QueueFree();
    }

    // Add new bloblys to the scene
    foreach (var newBlobly in newPopulation)
    {
        CallDeferred("add_child", newBlobly);
    }
}

private blobly CreateNewBlobly(blobly parentBlobly)
{
	string path = "agent/blobly.tscn";
	PackedScene packedScene = GD.Load<PackedScene>(path);
	blobly newBlobly = packedScene.Instantiate<blobly>();

	// Copy properties from parent to child
	newBlobly.Hunger = 100;
	newBlobly.Shopped_tree = 40;
	newBlobly.Wood = 150;
	newBlobly.Fishing_hooks = 100;
	newBlobly.Raw_fish = 100;
	newBlobly.CookedFish = 300;
	newBlobly.Skill_cooking = 0;
	newBlobly.Skill_chopping_tree = 0;
	newBlobly.Skill_chopping_wood = 0;
	newBlobly.Skill_fishing = 0;
	newBlobly.Skill_craft_fishing_hooks = 0;

	// Copy neural network weights
	newBlobly.NeuralNetwork.CopyWeightsFrom(parentBlobly.NeuralNetwork);

	return newBlobly;
}

}
