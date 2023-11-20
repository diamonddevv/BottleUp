using BottleUp.asset.script.Util;
using Godot;
using System;

[Tool]
public partial class RatingStars : Control
{
	private int _stars;
	private double _percent;

	[ExportCategory("Stars")]
	[Export] public double Percentage
	{
		get => _percent;
		set 
		{
			_percent = value;
			SetPercentagePerStar(value);
		}
	}
	[Export] public int StarCount
    {
        get => _stars;
        set
        {
            _stars = value;
            UpdateStars(value);
        }
    }
    [Export] public TextureProgressBar Star;

	public BottleUpHelper.Rating Rating { get; set; }


	private Node2D _root;
	
	public override void _Ready()
	{
		_root = GetNode<Node2D>("root");
	}

	public override void _Process(double delta)
	{
	}

	private void UpdateStars(int count)
	{
		foreach (var child in _root.GetChildren()) child.QueueFree();

		Star.Visible = false;
		float width = Star.Size.X;
		for (int i = 0; i < count; i++)
		{
			TextureProgressBar star = Star.Duplicate(4) as TextureProgressBar;
			_root.AddChild(star);

			star.Position = new Vector2(0 + width * i, 0);
			star.Visible = true;

			star.MinValue = 0;
			star.MaxValue = 100;
			star.Step = 1;
			star.Value = 0;
		}
	}

	private void SetPercentagePerStar(double totalPercentage)
	{
		double fullPerStar = 100 / StarCount;
		double filledStars = totalPercentage / fullPerStar;
		foreach (TextureProgressBar star in _root.GetChildren())
		{
			if (filledStars > 1)
			{
				filledStars -= 1;
				star.Value = 100;
			} else if (filledStars > 0)
			{
				filledStars = 0;
				star.Value = 100 * filledStars;
			} else if (filledStars <= 0)
			{
				star.Value = 0;
			}
		}
	}
}
