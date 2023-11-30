using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class Finish : CanvasLayer
{
	private int _starsPercent;


	public int StarsPercentage
	{
		get => _starsPercent; 
		set
		{
            _starsPercent = value;

			SaveManager.saveData.BestRating = _starsPercent;
			SaveManager.Save();
        }
	}

	private RatingStars _stars;
	private Button _mainMenuButton;
	private Button _playAgainButton;

	public override void _Ready()
	{
		_stars = GetNode<RatingStars>("stars");
		_mainMenuButton = GetNode<Button>("buttons/toMain");
		_playAgainButton = GetNode<Button>("buttons/again");


		_mainMenuButton.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://scene/mainMenu.tscn");
            QueueFree();
        };

		_playAgainButton.Pressed += () =>
		{
            GetTree().ChangeSceneToFile("res://scene/main.tscn");
			QueueFree();
        };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_stars.Percentage = StarsPercentage;
	}
}
