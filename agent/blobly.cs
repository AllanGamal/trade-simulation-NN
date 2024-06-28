using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class blobly : CharacterBody2D
{
	[Export]
	public Vector2 nextPosition = new Vector2();
	public static int Speed { get; set; } = 3200;
	public Locations locations = new Locations();

	// resources
	private float _hunger = 100;
	private float _shoppedTree = 40;
	private float _wood = 100;
	private float _fishingHooks = 100;
	private float _rawFish = 100;
	private float _cookedFish = 500;

	

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

	public static List<blobly> AllInstances
	{
		get => allInstances;
	}

	public blobly()
	{
		allInstances.Add(this);
		neuralNetwork = new NeuralNetwork(11, 8, 4, 6);
		visualizer = new NeuralNetworkVisualizer(AllInstances[0].NeuralNetwork);
		
		
		GD.Print(visualizer.NeuralNetwork.Layers.Length);
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

public void SetOutputs(double[] outputs)
{
	// take the highest output
	//outputs.OrderByDescending(o => o).First();
	

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
				skill += 0.008f;
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
		Get_resources(ref _shoppedTree, ref _skill_chopping_tree, 0.5f);
		Eat(2.1f);
	}

	public void Chop_wood()
	{
		nextPosition = locations.get_position_workshop();
		if (Shopped_tree < 1)
		{
			Chop_tree();
			return;
		}
		Use_resource(ref _shoppedTree, 1);
		Get_resources(ref _wood, ref _skillChoppingWood, 2);
		Eat(1.6f);
	}

	public void Fish()
	{
		nextPosition = locations.get_position_fishingLake();
		if (Fishing_hooks < 3)
		{
			Craft_fishing_hooks();
			return;
		}
		Use_resource(ref _fishingHooks, 3);
		Get_resources(ref _rawFish, ref _skillFishing, 1);
		Eat(1.3f);
	}

	public void Craft_fishing_hooks()
	{
		nextPosition = locations.get_position_workshop();
		if (Wood < 1)
		{
			Chop_wood();
			return;
		}
		Use_resource(ref _wood, 1);
		Get_resources(ref _fishingHooks, ref _skillCraftFishingHooks, 1);
		Eat(1.1f);

	}
	public void Cook()
	{
		nextPosition = locations.get_position_kitchen();
		if (Raw_fish < 6 || Wood < 9)
		{
			if (Raw_fish < 6)
			{
				Fish();
			}
			if (Wood < 8)
			{
				Chop_wood();
			}
			return;
		}

		Use_resource(ref _rawFish, 6);
		Use_resource(ref _wood, 9);
		Get_resources(ref _cookedFish, ref _skillCooking, 5f);
		Eat(1.2f);

	}

	public void GoToMarket ()
	{
		
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

	public static float GetAverageHunger() => GetAverageValue(b => b.Hunger);
	public static float GetAverageFishingHooks() => GetAverageValue(b => b.Fishing_hooks);
	public static float GetAverageShoppedTree() => GetAverageValue(b => b.Shopped_tree);
	public static float GetAverageWood() => GetAverageValue(b => b.Wood);
	public static float GetAverageRawFish() => GetAverageValue(b => b.Raw_fish);
	public static float GetAverageCookedFish() => GetAverageValue(b => b.CookedFish);

	public static float GetPopulationSize()
	{
		return allInstances.Count;
	}

	private static float GetAverageOfLowest10Percent(Func<blobly, float> selector)
	{
		if (allInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var lowest10PercentInstances = allInstances.OrderBy(selector).Take(tenPercent);

		float total = lowest10PercentInstances.Sum(selector);
		return total / tenPercent;
	}

	private static float GetAverageOfHighest10Percent(Func<blobly, float> selector)
	{
		if (allInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var highest10PercentInstances = allInstances.OrderByDescending(selector).Take(tenPercent);

		float total = highest10PercentInstances.Sum(selector);
		return total / tenPercent;
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


	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		ChangeColor();
		animations = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
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
		
		SetOutputs(outputs);
			//GD.Print("Outputs: " + string.Join(", ", outputs));
			
			
						

			clickPosition = nextPosition;

		}

		if (Position.DistanceTo(clickPosition) > 25)
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




