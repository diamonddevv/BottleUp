using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class Pause : CanvasLayer
{
    [Export] public NodePath EncapsulatingHud;

    [ExportCategory("Buttons")]
	[Export] public NodePath Resume;
	[Export] public NodePath MainMenu;
	[Export] public NodePath Settings;
	[Export] public NodePath ReportBugs;


    private HUD _hud;

    private Button _resume;
	private Button _mainMenu;
	private Button _settings;
	private Button _reportBugs;

	public override void _Ready()
	{
		_hud = GetNode<HUD>(EncapsulatingHud);

		_resume = GetNode<Button>(Resume);
		_mainMenu = GetNode<Button>(MainMenu);
		_settings = GetNode<Button>(Settings);
		_reportBugs = GetNode<Button>(ReportBugs);

		_resume.Pressed += () =>
		{
			_hud.SetPaused(false);
		};

		_mainMenu.Pressed += () =>
		{

		};

		_settings.Pressed += () =>
		{

		};

		_reportBugs.Pressed += () =>
		{
			OS.ShellOpen(BottleUpHelper.ISSUES_GITHUB_REPOSITORY);
		};
	}

	
	public override void _Process(double delta)
	{
	}
}
