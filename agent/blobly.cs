using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class blobly : CharacterBody2D
{
	[Export]
	public Vector2 nextPosition = new Vector2();
	public static int Speed { get; set; } = 4200;
	public Locations locations = new Locations();

	// resources
	private float _hunger = 100;
	private float _shoppedTree = 40;
	private float _wood = 80;
	private float _fishingHooks = 100;
	private float _rawFish = 100;
	private float _cookedFish = 80;

	



	

	// skills
	private float _skillCooking = 0;
	private float _skill_chopping_tree = 0;
	private float _skillChoppingWood = 0;
	private float _skillFishing = 0;
	private float _skillCraftFishingHooks = 0;
	private Sprite2D sprite;
	private NeuralNetwork neuralNetwork;
	private static NeuralNetworkVisualizer visualizer;

	private static List<blobly> allInstances = new List<blobly>();
	private double[] outputs;
	private int score;

	public int Score
	{
		get => score;
		set => score = value;
	}

// get and set the outputs of the neural network
public double[] Outputs
{
	get => outputs;
	set => outputs = value;
}

	public static List<blobly> AllInstances
	{
		get => allInstances;
	}

	public blobly()
	{
		allInstances.Add(this);
		neuralNetwork = new NeuralNetwork(11, 16, 8, 5);
		outputs = new double[] {0,0,0,0,0};
		visualizer = new NeuralNetworkVisualizer(AllInstances[0].NeuralNetwork);
		this.score = -1;
		this.ZIndex = 10;
		
		
		//GD.Print(visualizer.NeuralNetwork.Layers.Length);
	}

	public NeuralNetwork NeuralNetwork
	{
		get => neuralNetwork;
	}
	
	public void ChangeColor()
	{
		if (sprite == null)
		{
			sprite = GetNode<Sprite2D>("Sprite2D");
		}

		float hungerRatio = (100 - Hunger) / 220.0f;

		Color targetColor;

		float ratio = hungerRatio * 2.0f;
		targetColor = new Color(0.0f + ratio, 1.0f - ratio, 0.0f); // green -> red


		sprite.Modulate = targetColor;
	}

	public static bool IsHalfPopulationAboveMinimalHunger()
	{

		blobly[] allBloblys = allInstances.ToArray();

		// if any blobly scoore that is not -1
		if (allBloblys.Any(b => b.Score == -1))
		{
			return true;
		}
		// change z-index of the blobly
		return false;

		/*
		float averageHunger = GetAverageOfLowest10Percent(b => b.Hunger);

		if (averageHunger < 15)
		{
			return false;
		}
		return true;
		*/

		/*
		blobly[] survivors = allInstances.ToArray();
		// sort by descending hunger
		survivors = survivors.OrderByDescending(b => b.Hunger).ToArray();
		int index = (survivors.Length / 2)+1;
		if (survivors[index].Hunger < 15)
		{
			return false;
		}
		return true;
		*/
	}

	




	private AnimationPlayer animations;
	private Vector2 targetPosition;
	private Vector2 clickPosition;

	// input for the input layer of the neural network
public double[] GetInputs()
{
	return new double[]
	{
		(Hunger-50)/50,
		(Shopped_tree-500)/500,
		(Wood-500)/500,
		(Fishing_hooks-500)/500,
		(Raw_fish-500)/500,
		(CookedFish-500)/500,
		(Skill_cooking-2.5)/2.5,
		(Skill_chopping_tree-2.5)/2.5,
		(Skill_chopping_wood-2.5)/2.5,
		(Skill_fishing-2.5)/2.5,
		(Skill_craft_fishing_hooks-2.5)/2.5
	};
}





	

	~blobly()
	{
		allInstances.Remove(this);
	}



	public void Eat(float m)
	{
		Hunger -= m;
		if (CookedFish > 0 && Hunger < 100)
		{
			float rest = 100 - Hunger;
			float eatAmount = Math.Min(CookedFish, rest / m);

			Hunger += eatAmount * m;
			CookedFish -= eatAmount;

			// hunger no more than 100
			Hunger = Math.Min(Hunger, 100);

		}
		if (Hunger < 0)
		{
			Hunger = 0;
		}
		
	}

	public static void Get_resources<T, U>(ref T target, ref float skill, U m) where T : struct where U : struct
	{
		dynamic targetValue = target;
		dynamic mValue = m;

		if (targetValue < 1000)
		{
			float totalIncrease = (1 + skill) * mValue;

			targetValue += totalIncrease;

			target = (T)targetValue;

			if (skill < 5)
			{
				skill += 0.001f;
			}
		} 
		
	}

	public static void Use_resource(ref float resource, int m)
	{
		resource -= m;
	}

	public void Chop_tree()
	{
		nextPosition = locations.get_position_lumberyard();
		Get_resources(ref _shoppedTree, ref _skill_chopping_tree, 1.1f);
		Eat(1.5f);
	}

	public void Chop_wood()
	{
		nextPosition = locations.get_position_workshop();
		Eat(1.3f);
		if (Shopped_tree < 1)
		{
			
			return;
		}
		Use_resource(ref _shoppedTree, 1);
		Get_resources(ref _wood, ref _skillChoppingWood, 7.5f);
	}

	public void Fish()
	{
		nextPosition = locations.get_position_fishingLake();
		Eat(1.02f);
		if (Fishing_hooks < 4)
		{
			
			return;
		}
		Use_resource(ref _fishingHooks, 4);
		Get_resources(ref _rawFish, ref _skillFishing, 5f);
	}

	public void Craft_fishing_hooks()
	{
		nextPosition = locations.get_position_workshop();
		Eat(1.01f);
		if (Wood < 1)
		{
			
			return;
		}
		Use_resource(ref _wood, 1);
		Get_resources(ref _fishingHooks, ref _skillCraftFishingHooks, 2.7f);

	}
	private static float res = 50f;
	
	
	public static float Res {
		get => res;
		set => res = value;
	}
	public void Cook()
	{
		nextPosition = locations.get_position_kitchen();
		Eat(1.04f);
		if (Raw_fish < 3 || Wood < 4)
		{
			
			return;
		}

		Use_resource(ref _rawFish, 3);
		Use_resource(ref _wood, 4);
		Get_resources(ref _cookedFish, ref _skillCooking, res);

	}

	public void GoToMarket ()
	{

		List<Action> actions = new List<Action>
		{
			Chop_tree,
			Chop_wood,
			Fish,
			Craft_fishing_hooks,
			Cook,
			
		};

		Random rand = new Random();
		actions[rand.Next(actions.Count)]();
		
	}

	public float Shopped_tree
	{
		get => _shoppedTree;
		set => _shoppedTree = value;
	}
	public float Wood
	{
		get => _wood;
		set => _wood = value;
	}
	public float Fishing_hooks
	{
		get => _fishingHooks;
		set => _fishingHooks = value;
	}
	public float Raw_fish
	{
		get => _rawFish;
		set => _rawFish = value;
	}
	public float CookedFish
	{
		get => _cookedFish;
		set => _cookedFish = value;
	}
	public float Skill_cooking
	{
		get => _skillCooking;
		set => _skillCooking = value;
	}
	public float Skill_chopping_tree
	{
		get => _skill_chopping_tree;
		set => _skill_chopping_tree = value;
	}
	public float Skill_chopping_wood
	{
		get => _skillChoppingWood;
		set => _skillChoppingWood = value;
	}
	public float Skill_fishing
	{
		get => _skillFishing;
		set => _skillFishing = value;
	}
	public float Skill_craft_fishing_hooks
	{
		get => _skillCraftFishingHooks;
		set => _skillCraftFishingHooks = value;
	}

	public float Hunger
	{
		get => _hunger;
		set
		{
			{
				_hunger = value;
				ChangeColor(); // update color when hunger changes
			}
		}
	}

	public static float GetAverageValue(Func<blobly, float> selector)
	{
		if (allInstances.Count == 0) return 0;
		return allInstances.Sum(selector) / allInstances.Count;
	}

	public static float GetAverageValue2 (Func<blobly, float> selector)
	{
		var instancesWithScoreMinusOne = allInstances.Where(b => b.Score == -1);
		if (instancesWithScoreMinusOne.Count() == 0) return 0;
		return instancesWithScoreMinusOne.Sum(selector) / instancesWithScoreMinusOne.Count();
	
	}


	public static float GetAverageHunger() => GetAverageValue(b => b.Hunger);
	public static float GetAverageFishingHooks() => GetAverageValue(b => b.Fishing_hooks);
	public static float GetAverageShoppedTree() => GetAverageValue(b => b.Shopped_tree);
	public static float GetAverageWood() => GetAverageValue(b => b.Wood);
	public static float GetAverageRawFish() => GetAverageValue(b => b.Raw_fish);
	
	public static float GetAverageCookedFish() => GetAverageValue(b => b.CookedFish);  
	public static float GetAverageHunger2() => GetAverageValue2(b => b.Hunger);
	public static float GetAverageFishingHooks2() => GetAverageValue2(b => b.Fishing_hooks);
	public static float GetAverageShoppedTree2() => GetAverageValue2(b => b.Shopped_tree);
	public static float GetAverageWood2() => GetAverageValue2(b => b.Wood);
	public static float GetAverageRawFish2() => GetAverageValue2(b => b.Raw_fish);
	public static float GetAverageCookedFish2() => GetAverageValue2(b => b.CookedFish);

	

	public static float GetPopulationSize()
	{
		return allInstances.Count;
	}

	private static float GetAverageOfLowest10Percent(Func<blobly, float> selector)
	{
		blobly[] sortedBloblys = blobly.AllInstances.OrderBy(b => b.Hunger).ToArray();
		int lowestHalfPopulation = sortedBloblys.Length;

		var top50 = sortedBloblys.Take((int)(lowestHalfPopulation * 0.5f)).ToList();

		float total = top50.Sum(selector);
		return total / top50.Count;


		/*

		if (allInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(allInstances.Count * 0.5);
		var lowest10PercentInstances = allInstances.OrderBy(selector).Take(tenPercent);

		float total = lowest10PercentInstances.Sum(selector);
		return total / tenPercent;
		*/
	}

	private static float GetAverageOfHighest10Percent(Func<blobly, float> selector)
	{

		blobly[] sortedBloblys = blobly.AllInstances.OrderByDescending(b => b.Hunger).ToArray();
		int lowestHalfPopulation = sortedBloblys.Length;

		var top50 = sortedBloblys.Take((int)(lowestHalfPopulation * 0.5f)).ToList();

		float total = top50.Sum(selector);
		return total / top50.Count;

		/*
		if (allInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(allInstances.Count * 0.5);
		var highest10PercentInstances = allInstances.OrderByDescending(selector).Take(tenPercent);

		float total = highest10PercentInstances.Sum(selector);
		return total / tenPercent;
		*/
	}

	private static float GetAverageOfLowest10Percent2(Func<blobly, float> selector)
	{
		var filteredInstances = allInstances.Where(b => b.Score == -1).ToList();
		if (filteredInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(filteredInstances.Count * 0.5);
		var lowest10PercentInstances = filteredInstances.OrderBy(selector).Take(tenPercent);

		float total = lowest10PercentInstances.Sum(selector);
		return 0;
	}

	private static float GetAverageOfHighest10Percent2(Func<blobly, float> selector)
	{

		var filteredInstances = allInstances.Where(b => b.Score == -1).ToList();
		if (filteredInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(filteredInstances.Count * 0.5);
		var lowest10PercentInstances = filteredInstances.OrderByDescending(selector).Take(tenPercent);

		float total = lowest10PercentInstances.Sum(selector);
		return 0;
	}

	public static float GetAverageOfCookedFishOfTheLowest10Percent() => GetAverageOfLowest10Percent(b => b.CookedFish);
	public static float GetAverageOfCookedFishOfTheHighest10Percent() => GetAverageOfHighest10Percent(b => b.CookedFish);
	public static float GetAverageOfRawFishOfTheLowest10Percent() => GetAverageOfLowest10Percent(b => b.Raw_fish);
	public static float GetAverageOfRawFishOfTheHighest10Percent() => GetAverageOfHighest10Percent(b => b.Raw_fish);
	public static float GetAverageOfHungerOfTheLowest10Percent() => GetAverageOfLowest10Percent(b => b.Hunger);
	public static float GetAverageOfHungerOfTheHighest10Percent() => GetAverageOfHighest10Percent(b => b.Hunger);
	public static float GetAverageOfShoppedTreeOfTheLowest10Percent() => GetAverageOfLowest10Percent(b => b.Shopped_tree);
	public static float GetAverageOfShoppedTreeOfTheHighest10Percent() => GetAverageOfHighest10Percent(b => b.Shopped_tree);
	public static float GetAverageOfWoodOfTheLowest10Percent() => GetAverageOfLowest10Percent(b => b.Wood);
	public static float GetAverageOfWoodOfTheHighest10Percent() => GetAverageOfHighest10Percent(b => b.Wood);
	public static float GetAverageOfFishingHooksOfTheLowest10Percent() => GetAverageOfLowest10Percent(b => b.Fishing_hooks);
	public static float GetAverageOfFishingHooksOfTheHighest10Percent() => GetAverageOfHighest10Percent(b => b.Fishing_hooks);


	public static float GetAverageOfCookedFishOfTheLowest10Percent2() => GetAverageOfLowest10Percent2(b => b.CookedFish);
	public static float GetAverageOfCookedFishOfTheHighest10Percent2() => GetAverageOfHighest10Percent2(b => b.CookedFish);
	public static float GetAverageOfRawFishOfTheLowest10Percent2() => GetAverageOfLowest10Percent2(b => b.Raw_fish);
	public static float GetAverageOfRawFishOfTheHighest10Percent2() => GetAverageOfHighest10Percent2(b => b.Raw_fish);
	public static float GetAverageOfHungerOfTheLowest10Percent2() => GetAverageOfLowest10Percent2(b => b.Hunger);
	public static float GetAverageOfHungerOfTheHighest10Percent2() => GetAverageOfHighest10Percent2(b => b.Hunger);
	public static float GetAverageOfShoppedTreeOfTheLowest10Percent2() => GetAverageOfLowest10Percent2(b => b.Shopped_tree);
	public static float GetAverageOfShoppedTreeOfTheHighest10Percent2() => GetAverageOfHighest10Percent2(b => b.Shopped_tree);
	public static float GetAverageOfWoodOfTheLowest10Percent2() => GetAverageOfLowest10Percent2(b => b.Wood);
	public static float GetAverageOfWoodOfTheHighest10Percent2() => GetAverageOfHighest10Percent2(b => b.Wood);
	public static float GetAverageOfFishingHooksOfTheLowest10Percent2() => GetAverageOfLowest10Percent2(b => b.Fishing_hooks);
	public static float GetAverageOfFishingHooksOfTheHighest10Percent2() => GetAverageOfHighest10Percent2(b => b.Fishing_hooks);


	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		ChangeColor();
		animations = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
	}
	public void PerformRandomAction(int score)
	{
		
		
		if (this.Hunger < 2)
		{
		this.Score = score;
		int scory = score + 10;
		// change opacity of the blobly and make it orange
		this.Modulate = new Color(1f, 1f, 1f, 0.08f);
		// make it smaller
		this.Scale = new Vector2(0.5f, 0.5f);
		this.ZIndex = 9;
		return;
		}



		
		List<Action> actions = new List<Action>
		{
			Chop_tree,
			Chop_wood,
			Fish,
			Craft_fishing_hooks,
			Cook,
		};

		clickPosition = nextPosition;
		double[] outputs = this.outputs;
		int index = Array.IndexOf(outputs, outputs.Max());

		// if highest output is GoToMarket, choose the second highest output
	
		actions[index]();
		// clear the outputs
		outputs = new double[outputs.Length];

		// Uppdatera neural network inputs och outputs här om det behövs
		double[] inputs = GetInputs();
		outputs = neuralNetwork.CalculateOutputs(inputs);
		this.outputs = outputs;
		// index of the highest output
		//SetOutputs(outputs);

		// Uppdatera position
		
	}

	private void UpdateAnimation()
	{
		if (Velocity.Length() == 0)
		{
			animations.Stop();
		}
		else
		{
			string direction = "Down";
			if (Velocity.X < 0) direction = "Left";
			else if (Velocity.X > 0) direction = "Right";
			else if (Velocity.Y < 0) direction = "Up";

			// animate the direction of the sprite
			animations.Play("walk" + direction);
		}
	}

	public static blobly getHighestRankedBlobly() {
		blobly[] survivors = allInstances.ToArray();
		// sort by descending hunger
		survivors = survivors.OrderByDescending(b => b.Hunger).ToArray();
		return survivors[0];
	}

	
	private void GoThere(double delta)
	{
		targetPosition = (clickPosition - Position).Normalized();
		Velocity = targetPosition * Speed;
		MoveAndSlide();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("left_click"))
		{
			List<Action> actions = new List<Action>
			{
				Chop_tree,
				Chop_wood,
				Fish,
				Craft_fishing_hooks,
				Cook
			};

			// perform random action when clicked

			Random rand = new Random();
			actions[rand.Next(actions.Count)]();
			actions[rand.Next(actions.Count)]();
			actions[rand.Next(actions.Count)]();
			actions[rand.Next(actions.Count)]();
			actions[rand.Next(actions.Count)]();
			Random randy = new Random();
			
			
			

			double[] inputs = GetInputs();
		double[] outputs = neuralNetwork.CalculateOutputs(inputs);
		
		//SetOutputs(outputs);
			//GD.Print("Outputs: " + string.Join(", ", outputs));
			
			
						

			clickPosition = nextPosition;

		}

		if (Position.DistanceTo(clickPosition) > 100)
		{
			GoThere(delta);
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		UpdateAnimation();

	}
}




