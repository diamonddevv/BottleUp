using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class HUD : CanvasLayer
{

	[ExportCategory("UI Controls")]
	[Export] public Key Pause { get; set; } = Key.Escape;

	[ExportCategory("Minimap")]
	[Export] public NodePath Player;
	[Export] public PackedScene MapScene;

	[ExportCategory("Misc.")]
	[Export] public NodePath InGameTimer;


    //
    private CanvasLayer _pauseOverlay;
    private Minimap _minimap;
    private Label _timerLabel;
    private Timer _timer;
    private Player _player;
    private PackedScene _mapScene;

	private bool _lastPauseControlTick;
	private bool _pauseControlTick;
	private bool _isPaused;
	
	public override void _Ready()
	{
		_pauseOverlay = GetNode<CanvasLayer>("pause");
		_minimap = GetNode<Minimap>("ingame/margin/minimap");
        _timerLabel = GetNode<Label>("ingame/timer");

		_timer = GetNode<Timer>(InGameTimer);
		_player = GetNode<Player>(Player);

		_minimap.SetMapScene(MapScene);
		_minimap.SetPlayer(_player);
	}

	
	public override void _Process(double delta)
	{
		ControlTick();
		CheckPause();

		if (_timer != null) _timerLabel.Text = BottleUpHelper.FormatTime(_timer.TimeLeft);

        GetTree().Paused = _isPaused;
        PauseTick(delta);
	}

	public void ControlTick()
	{
		_lastPauseControlTick = _pauseControlTick;

		_pauseControlTick = false;

		if (Input.IsKeyPressed(Pause)) _pauseControlTick = true;
	}
	public void CheckPause()
	{
		if (_pauseControlTick && !_lastPauseControlTick)
		{
			_isPaused = !_isPaused;
		}
	}


	public void PauseTick(double delta)
	{
		_pauseOverlay.Visible = _isPaused;

        if (_isPaused)
		{
			
		}
	}

	public void SetPaused(bool b) => _isPaused = b;
	public bool GetPaused() => _isPaused;

	public Player GetPlayer() => _player;
}
