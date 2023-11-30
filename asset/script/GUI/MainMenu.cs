using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class MainMenu : CanvasLayer
{

	private Button _play;
	private Button _close;
	private TextureButton _forg;

	private RatingStars _best;

	public override void _Ready()
	{
		SaveManager.Load();
		SaveManager.Save();

		_play = GetNode<Button>("buttons/play");
		_close = GetNode<Button>("buttons/close");
		_forg = GetNode<TextureButton>("forg");
		_best = GetNode<RatingStars>("best");



        _play.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://scene/main.tscn");
		};

		_close.Pressed += () =>
		{
			GetTree().Quit(0); // close game
		};

		_forg.Pressed += () =>
		{
            OS.ShellOpen("https://www.instagram.com/novaquinn1010/");
        };

		_best.Percentage = SaveManager.saveData.BestRating;
	}


	public override void _Process(double delta)
	{
	}
}
