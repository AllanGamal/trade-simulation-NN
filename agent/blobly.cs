using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class blobly : CharacterBody2D
{
	[Export]
	public Vector2 nextPosition = new Vector2();
	public static int Speed { get; set; } = 1200;
	public Locations locations = new Locations();

	// resources
	private float _hunger = 100;
	private int _shoppedTree = 40;
	private int _wood = 100;
	private int _fishingHooks = 100;
	private int _rawFish = 100;
	private float _cookedFish = 500;

	// skills
	private float _skillCooking = 0;
	private float _skill_chopping_tree = 0;
	private float _skillChoppingWood = 0;
	private float _skillFishing = 0;
	private float _skillCraftFishingHooks = 0;
	private Sprite2D sprite;


	// change color of the blobly based on its hunger
// Assuming 'Hunger' is a value between 0 and 1
// and there's a method or update loop calling this method repeatedly
public void ChangeColor()
{
  if (sprite == null)
  {
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

  // Define color based on hunger level (0 to 1)
  
  float hungerRatio = (100-Hunger) / 220.0f;

  Color targetColor;

  // Green to White (0 to 0.5)
  
	 float ratio = hungerRatio * 2.0f; // Stretch range for green and white
	targetColor = new Color(0.0f + ratio, 1.0f - ratio, 0.0f); // Green -> White
  
 

  sprite.Modulate = targetColor;
}




	private AnimationPlayer animations;
	private Vector2 targetPosition;
	private Vector2 clickPosition;

	private static List<blobly> allInstances = new List<blobly>();

	

	public blobly()
	{
		allInstances.Add(this);
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

			// Säkerställ att Hunger inte överstiger 100
			Hunger = Math.Min(Hunger, 100);

		}
	}

	public static void Get_resources(ref int target, ref float skill, int m)
	{
		if (target < 1000)
		{
			target += 1 * (1 + (int)Math.Floor(skill)) * m;
			if (skill < 5)
			{
				skill += 0.001f;
			}
		}
	}

	public static void Get_resources(ref float target, ref float skill, int m)
	{
		if (target < 1000)
		{
			target += 1 * (1 + skill) * m;
			if (skill < 5)
			{
				skill += 0.001f;
			}
		}
	}

	public static void Get_resources(ref float target, ref float skill, float m)
	{
		if (target < 1000)
		{
			float increase = 1 * (1 + skill) * m;
			target += increase;

			if (skill < 5)
			{
				skill += 0.001f;
			}
		}
	}

	public static void Use_resource(ref int resource, int m)
	{
		resource -= m;
	}

	// Användningsexempel:
	public void Chop_tree()
	{
		nextPosition = locations.get_position_lumberyard();
		Get_resources(ref _shoppedTree, ref _skill_chopping_tree, 1);
		Eat(1.8f);
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
		Eat(1.4f);
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
		Eat(1.1f);
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
		Eat(1.1f);

	}

	public int Shopped_tree
	{
		get => _shoppedTree;
		set => _shoppedTree = value;
	}
	public int Wood
	{
		get => _wood;
		set => _wood = value;
	}
	public int Fishing_hooks
	{
		get => _fishingHooks;
		set => _fishingHooks = value;
	}
	public int Raw_fish
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
		set {{
		_hunger = value;
		ChangeColor(); // Update color when hunger changes
	}}
	}

	public static float GetAverageHunger()
	{
		if (allInstances.Count == 0)
			return 0;

		float totalHunger = allInstances.Sum(b => b.Hunger);
		return totalHunger / allInstances.Count;
	}

	public static float GetAverageFishingHooks()
	{
		if (allInstances.Count == 0)
			return 0;

		float totalFishingHooks = allInstances.Sum(b => b.Fishing_hooks);
		return totalFishingHooks / allInstances.Count;
	}

	public static float GetAverageShoppedTree()
	{
		if (allInstances.Count == 0)
			return 0;

		float totalShoppedTree = allInstances.Sum(b => b.Shopped_tree);
		return totalShoppedTree / allInstances.Count;
	}

	public static float GetAverageWood()
	{
		if (allInstances.Count == 0)
			return 0;

		float totalWood = allInstances.Sum(b => b.Wood);
		return totalWood / allInstances.Count;
	}

	public static float GetAverageRawFish()
	{
		if (allInstances.Count == 0)
			return 0;

		float totalRawFish = allInstances.Sum(b => b.Raw_fish);
		return totalRawFish / allInstances.Count;
	}

	public static float GetAverageCookedFish()
	{
		if (allInstances.Count == 0)
			return 0;

		float totalCookedFish = allInstances.Sum(b => b.CookedFish);
		return totalCookedFish / allInstances.Count;
	}

	public static float GetPopulationSize()
	{
		return allInstances.Count;
	}

	public static float GetAverageOfCookedFishOfTheLowest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int lowest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var lowest10PercentInstances = allInstances.OrderBy(b => b.CookedFish).Take(lowest10Percent);
		float totalCookedFish = lowest10PercentInstances.Sum(b => b.CookedFish);
		return totalCookedFish / lowest10Percent;
	}

	public static float GetAverageOfCookedFishOfTheHighest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int highest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var highest10PercentInstances = allInstances.OrderByDescending(b => b.CookedFish).Take(highest10Percent);
		float totalCookedFish = highest10PercentInstances.Sum(b => b.CookedFish);
		return totalCookedFish / highest10Percent;
	}

	public static float GetAverageOfRawFishOfTheLowest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int lowest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var lowest10PercentInstances = allInstances.OrderBy(b => b.Raw_fish).Take(lowest10Percent);
		float totalRawFish = lowest10PercentInstances.Sum(b => b.Raw_fish);
		return totalRawFish / lowest10Percent;
	}

	public static float GetAverageOfRawFishOfTheHighest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int highest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var highest10PercentInstances = allInstances.OrderByDescending(b => b.Raw_fish).Take(highest10Percent);
		float totalRawFish = highest10PercentInstances.Sum(b => b.Raw_fish);
		return totalRawFish / highest10Percent;
	}

	/*
	private float _hunger = 100;
	private int _shoppedTree = 40;
	private int _wood = 100;
	private int _fishingHooks = 100;*/

	public static float GetAverageOfHungerOfTheLowest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int lowest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var lowest10PercentInstances = allInstances.OrderBy(b => b.Hunger).Take(lowest10Percent);
		float totalHunger = lowest10PercentInstances.Sum(b => b.Hunger);
		return totalHunger / lowest10Percent;
	}

	public static float GetAverageOfHungerOfTheHighest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int highest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var highest10PercentInstances = allInstances.OrderByDescending(b => b.Hunger).Take(highest10Percent);
		float totalHunger = highest10PercentInstances.Sum(b => b.Hunger);
		return totalHunger / highest10Percent;
	}

	public static float GetAverageOfShoppedTreeOfTheLowest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int lowest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var lowest10PercentInstances = allInstances.OrderBy(b => b.Shopped_tree).Take(lowest10Percent);
		float totalShoppedTree = lowest10PercentInstances.Sum(b => b.Shopped_tree);
		return totalShoppedTree / lowest10Percent;
	}

	public static float GetAverageOfShoppedTreeOfTheHighest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int highest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var highest10PercentInstances = allInstances.OrderByDescending(b => b.Shopped_tree).Take(highest10Percent);
		float totalShoppedTree = highest10PercentInstances.Sum(b => b.Shopped_tree);
		return totalShoppedTree / highest10Percent;
	}

	public static float GetAverageOfWoodOfTheLowest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int lowest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var lowest10PercentInstances = allInstances.OrderBy(b => b.Wood).Take(lowest10Percent);
		float totalWood = lowest10PercentInstances.Sum(b => b.Wood);
		return totalWood / lowest10Percent;
	}

	public static float GetAverageOfWoodOfTheHighest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int highest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var highest10PercentInstances = allInstances.OrderByDescending(b => b.Wood).Take(highest10Percent);
		float totalWood = highest10PercentInstances.Sum(b => b.Wood);
		return totalWood / highest10Percent;
	}

	public static float GetAverageOfFishingHooksOfTheLowest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int lowest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var lowest10PercentInstances = allInstances.OrderBy(b => b.Fishing_hooks).Take(lowest10Percent);
		float totalFishingHooks = lowest10PercentInstances.Sum(b => b.Fishing_hooks);
		return totalFishingHooks / lowest10Percent;
	}

	public static float GetAverageOfFishingHooksOfTheHighest10Percent()
	{
		if (allInstances.Count == 0)
			return 0;

		int highest10Percent = (int)Math.Ceiling(allInstances.Count * 0.1);
		var highest10PercentInstances = allInstances.OrderByDescending(b => b.Fishing_hooks).Take(highest10Percent);
		float totalFishingHooks = highest10PercentInstances.Sum(b => b.Fishing_hooks);
		return totalFishingHooks / highest10Percent;
	}



	
	
	




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

			// You might want to play the animation here
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
			//clickPosition = GetGlobalMousePosition();
			// list of functions to get random positions
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




			clickPosition = nextPosition;
			// print the position
			//GD.Print(clickPosition);
			// print the wood
		
			

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
