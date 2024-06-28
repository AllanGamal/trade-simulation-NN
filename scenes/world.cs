using Godot;
using System;

public partial class world : Node2D
{
	private chart cookedFishChart;
	private chart rawFishChart;
	private chart hungerChart;
	private chart shoppedTreeChart;
	private chart woodChart;
	private chart fishingHooksChart;
	private chart populationSizeChart;

	private NeuralNetworkVisualizer neuralNetworkVisualizer;


	// spawn a blobly at a random location
	public void spawn_blobly()
	{
		string path = "agent/blobly.tscn";
		PackedScene packedScene = GD.Load<PackedScene>(path);

		for (int i = 0; i < 100; i++)
		{
			blobly blobly = packedScene.Instantiate<blobly>();
			
			CallDeferred("add_child", blobly);
		}
	}
	

	public override void _Ready()
	{
		spawn_blobly();
		neuralNetworkVisualizer = new NeuralNetworkVisualizer(blobly.AllInstances[0].NeuralNetwork);
		AddChild(neuralNetworkVisualizer);
		
		
		



		hungerChart = InstantiateThreeCharts(new Vector2(1281, 1), "Hunger",
	blobly.GetAverageOfHungerOfTheHighest10Percent, blobly.GetAverageHunger, blobly.GetAverageOfHungerOfTheLowest10Percent);
		fishingHooksChart = InstantiateThreeCharts(new Vector2(1601, 1), "Fishing Hooks",
			blobly.GetAverageOfFishingHooksOfTheHighest10Percent, blobly.GetAverageFishingHooks, blobly.GetAverageOfFishingHooksOfTheLowest10Percent);
		cookedFishChart = InstantiateThreeCharts(new Vector2(1281, 350), "Cooked Fish",
			blobly.GetAverageOfCookedFishOfTheHighest10Percent, blobly.GetAverageCookedFish, blobly.GetAverageOfCookedFishOfTheLowest10Percent);
		rawFishChart = InstantiateThreeCharts(new Vector2(1601, 350), "Raw Fish",
			blobly.GetAverageOfRawFishOfTheHighest10Percent, blobly.GetAverageRawFish, blobly.GetAverageOfRawFishOfTheLowest10Percent);
		shoppedTreeChart = InstantiateThreeCharts(new Vector2(1281, 700), "Shopped Trees",
			blobly.GetAverageOfShoppedTreeOfTheHighest10Percent, blobly.GetAverageShoppedTree, blobly.GetAverageOfShoppedTreeOfTheLowest10Percent);
		woodChart = InstantiateThreeCharts(new Vector2(1601, 700), "Wood",
			blobly.GetAverageOfWoodOfTheHighest10Percent, blobly.GetAverageWood, blobly.GetAverageOfWoodOfTheLowest10Percent);

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("left_click"))
		{
			GD.Print("Left click pressed");
			AddChild(neuralNetworkVisualizer);
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

	private void OnTimerTimeout()
	{
		UpdateAllGraphs();
	}
}
