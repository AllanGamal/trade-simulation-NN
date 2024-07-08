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
	private float _shoppedTree = 0;
	private float _wood = 0;
	private float _fishingHooks = 0;
	private float _rawFish = 0;
	private float _cookedFish = 80;

	// skills
	private float _skillCooking = 0;
	private float _skill_chopping_tree = 0;
	private float _skillChoppingWood = 0;
	private float _skillFishing = 0;
	private float _skillCraftFishingHooks = 0;
	private Sprite2D sprite;
	private NeuralNetwork neuralNetwork;

	private static List<blobly> allInstances = new List<blobly>();
	private double[] outputs;
	private int score;
	private static blobly lastWinner;
	private static float res = 3f;

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
	
	public static float Res {
		get => res;
		set => res = value;
	}
	public NeuralNetwork NeuralNetwork
    {
        get => neuralNetwork;
		set => neuralNetwork = value;
    }

	public static blobly LastWinner
	{
		get => lastWinner;
		set => lastWinner = value;
	}

	public int Score
	{
		get => score;
		set => score = value;
	}

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
		neuralNetwork = new NeuralNetwork(11, 64, 16, 8, 5);
		outputs = new double[] {0,0,0,0,0};
		this.score = -1;
		this.ZIndex = 10;
		
		// change color of last winner

		if (lastWinner != null)
		{
			
			lastWinner.Scale = new Vector2(5f, 5f);
			lastWinner.ZIndex = 15;
		}
		
	}

	public blobly(NeuralNetwork neuralNetwork)
	{
		allInstances.Add(this);
		this.neuralNetwork = neuralNetwork;
		outputs = new double[] {0,0,0,0,0};
		this.score = -1;
		this.ZIndex = 10;
		
		// change color of last winner

		if (lastWinner != null)
		{
			
			lastWinner.Scale = new Vector2(5f, 5f);
			lastWinner.ZIndex = 15;
		}

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

	public static bool IsAnyBloblyWithInitialScore()
	{

		blobly[] allBloblys = allInstances.ToArray();

		if (allBloblys.Any(b => b.Score == -1))
		{
			return true;
		}
		return false;
	}

	




	private AnimationPlayer animations;
	private Vector2 targetPosition;
	private Vector2 clickPosition;

	// input for the input layer of the neural network
public double[] GetNormalizedInputs()
{
	// -1 to 1
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
		Eat(1.0f);
	}

	public void Chop_wood()
	{
		nextPosition = locations.get_position_workshop();
		Eat(1.0f);
		if (Shopped_tree < 1 || Wood > 999)
		{
			
			return;
		}
		Use_resource(ref _shoppedTree, 1);
		Get_resources(ref _wood, ref _skillChoppingWood, 7.5f);
	}

	public void Fish()
	{
		nextPosition = locations.get_position_fishingLake();
		Eat(1.0f);
		if (Fishing_hooks < 4 || Raw_fish > 999)
		{
			
			return;
		}
		Use_resource(ref _fishingHooks, 4);
		Get_resources(ref _rawFish, ref _skillFishing, 5f);
	}

	public void Craft_fishing_hooks()
	{
		nextPosition = locations.get_position_workshop();
		Eat(1.0f);
		if (Wood < 1 || Fishing_hooks > 999)
		{
			
			return;
		}
		Use_resource(ref _wood, 1);
		Get_resources(ref _fishingHooks, ref _skillCraftFishingHooks, 2.7f);

	}
	
	public void Cook()
	{
		nextPosition = locations.get_position_kitchen();
		Eat(1.0f);
		if (Raw_fish < 3 || Wood < 4 || CookedFish > 999)
		{
			return;
		}

		Use_resource(ref _rawFish, 3);
		Use_resource(ref _wood, 4);
		Get_resources(ref _cookedFish, ref _skillCooking, res);
	}

// market not yet implemented
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

	public static float GetLastWinnerValue(Func<blobly, float> selector)
	{
		if (lastWinner == null) return 0;
		return selector(lastWinner);
	}


	public static float GetAverageHunger() => GetAverageValue(b => b.Hunger);
	public static float GetAverageFishingHooks() => GetAverageValue(b => b.Fishing_hooks);
	public static float GetAverageShoppedTree() => GetAverageValue(b => b.Shopped_tree);
	public static float GetAverageWood() => GetAverageValue(b => b.Wood);
	public static float GetAverageRawFish() => GetAverageValue(b => b.Raw_fish);
	
	public static float GetAverageCookedFish() => GetAverageValue(b => b.CookedFish);  
	public static float GetAverageHunger2() => GetLastWinnerValue(b => b.Hunger);
	public static float GetAverageFishingHooks2() => GetLastWinnerValue(b => b.Fishing_hooks);
	public static float GetAverageShoppedTree2() => GetLastWinnerValue(b => b.Shopped_tree);
	public static float GetAverageWood2() => GetLastWinnerValue(b => b.Wood);
	public static float GetAverageRawFish2() => GetLastWinnerValue(b => b.Raw_fish);
	public static float GetAverageCookedFish2() => GetLastWinnerValue(b => b.CookedFish);

	

	public static float GetPopulationSize()
	{
		return blobly.AllInstances.Count(b => b.Score == -1);
	}

	private static float GetAverageOfLowestHalfForInitialScore(Func<blobly, float> selector)
	{
		blobly[] sortedBloblys = blobly.AllInstances.OrderBy(b => b.Hunger).ToArray();
		int lowestHalfPopulation = sortedBloblys.Length;

		var top50 = sortedBloblys.Take((int)(lowestHalfPopulation * 0.5f)).ToList();

		float total = top50.Sum(selector);
		return total / top50.Count;

	}

	private static float GetAverageOfHighestHalfForInitialScore(Func<blobly, float> selector)
	{

		blobly[] sortedBloblys = blobly.AllInstances.OrderByDescending(b => b.Hunger).ToArray();
		int lowestHalfPopulation = sortedBloblys.Length;

		var top50 = sortedBloblys.Take((int)(lowestHalfPopulation * 0.5f)).ToList();

		float total = top50.Sum(selector);
		return total / top50.Count;

	}

	private static float GetAverageOfLowestHalfForInitialScore2(Func<blobly, float> selector)
	{
		var filteredInstances = allInstances.Where(b => b.Score == -1).ToList();
		if (filteredInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(filteredInstances.Count * 0.5);
		var lowest10PercentInstances = filteredInstances.OrderBy(selector).Take(tenPercent);

		float total = lowest10PercentInstances.Sum(selector);
		return 0;
	}

	private static float GetAverageOfHighestHalfForInitialScore2(Func<blobly, float> selector)
	{

		var filteredInstances = allInstances.Where(b => b.Score == -1).ToList();
		if (filteredInstances.Count == 0) return 0;

		int tenPercent = (int)Math.Ceiling(filteredInstances.Count * 0.5);
		var lowest10PercentInstances = filteredInstances.OrderByDescending(selector).Take(tenPercent);

		float total = lowest10PercentInstances.Sum(selector);
		return 0;
	}

	public static float GetAverageOfCookedFishOfLowestHalfForInitialScore() => GetAverageOfLowestHalfForInitialScore(b => b.CookedFish);
	public static float GetAverageOfCookedFishOfOfHighestHalfForInitialScore() => GetAverageOfHighestHalfForInitialScore(b => b.CookedFish);
	public static float GetAverageOfRawFishOfLowestHalfForInitialScore() => GetAverageOfLowestHalfForInitialScore(b => b.Raw_fish);
	public static float GetAverageOfRawFishOfOfHighestHalfForInitialScore() => GetAverageOfHighestHalfForInitialScore(b => b.Raw_fish);
	public static float GetAverageOfHungerOfLowestHalfForInitialScore() => GetAverageOfLowestHalfForInitialScore(b => b.Hunger);
	public static float GetAverageOfHungerOfOfHighestHalfForInitialScore() => GetAverageOfHighestHalfForInitialScore(b => b.Hunger);
	public static float GetAverageOfShoppedTreeOfLowestHalfForInitialScore() => GetAverageOfLowestHalfForInitialScore(b => b.Shopped_tree);
	public static float GetAverageOfShoppedTreeOfOfHighestHalfForInitialScore() => GetAverageOfHighestHalfForInitialScore(b => b.Shopped_tree);
	public static float GetAverageOfWoodOfLowestHalfForInitialScore() => GetAverageOfLowestHalfForInitialScore(b => b.Wood);
	public static float GetAverageOfWoodOfOfHighestHalfForInitialScore() => GetAverageOfHighestHalfForInitialScore(b => b.Wood);
	public static float GetAverageOfFishingHooksOfLowestHalfForInitialScore() => GetAverageOfLowestHalfForInitialScore(b => b.Fishing_hooks);
	public static float GetAverageOfFishingHooksOfOfHighestHalfForInitialScore() => GetAverageOfHighestHalfForInitialScore(b => b.Fishing_hooks);


	public static float GetAverageOfCookedFishOfLowestHalfForInitialScore2() => GetAverageOfLowestHalfForInitialScore2(b => b.CookedFish);
	public static float GetAverageOfCookedFishOfOfHighestHalfForInitialScore2() => GetAverageOfHighestHalfForInitialScore2(b => b.CookedFish);
	public static float GetAverageOfRawFishOfLowestHalfForInitialScore2() => GetAverageOfLowestHalfForInitialScore2(b => b.Raw_fish);
	public static float GetAverageOfRawFishOfOfHighestHalfForInitialScore2() => GetAverageOfHighestHalfForInitialScore2(b => b.Raw_fish);
	public static float GetAverageOfHungerOfLowestHalfForInitialScore2() => GetAverageOfLowestHalfForInitialScore2(b => b.Hunger);
	public static float GetAverageOfHungerOfOfHighestHalfForInitialScore2() => GetAverageOfHighestHalfForInitialScore2(b => b.Hunger);
	public static float GetAverageOfShoppedTreeOfLowestHalfForInitialScore2() => GetAverageOfLowestHalfForInitialScore2(b => b.Shopped_tree);
	public static float GetAverageOfShoppedTreeOfOfHighestHalfForInitialScore2() => GetAverageOfHighestHalfForInitialScore2(b => b.Shopped_tree);
	public static float GetAverageOfWoodOfLowestHalfForInitialScore2() => GetAverageOfLowestHalfForInitialScore2(b => b.Wood);
	public static float GetAverageOfWoodOfOfHighestHalfForInitialScore2() => GetAverageOfHighestHalfForInitialScore2(b => b.Wood);
	public static float GetAverageOfFishingHooksOfLowestHalfForInitialScore2() => GetAverageOfLowestHalfForInitialScore2(b => b.Fishing_hooks);
	public static float GetAverageOfFishingHooksOfOfHighestHalfForInitialScore2() => GetAverageOfHighestHalfForInitialScore2(b => b.Fishing_hooks);


	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		ChangeColor();
		animations = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
	}
	public void PerformNeuralNetworkAction(int score)
	{
		int extra = 0;
		if (blobly.AllInstances.Count(b => b.Score == -1) == 1)
		{
			this.Hunger = 1;
			extra = 100000;
		}
		
		if (this.Hunger < 2)
		{
			if (this.Score == -1){
		this.Score = score*10000+(int)(this.Wood/15)+(int)(this.Fishing_hooks)+(int)((this.Raw_fish)*15) + extra;
			}
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

		actions[index]();
		outputs = new double[outputs.Length];

		double[] inputs = GetNormalizedInputs();
		outputs = neuralNetwork.CalculateOutputs(inputs);
		this.outputs = outputs;
	
		
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




