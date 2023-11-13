using Godot;
using System;

public partial class HUD : CanvasLayer
{

	[ExportCategory("UI Controls")]
	[Export] public Key Pause { get; set; } = Key.Escape;


	//
	private CanvasLayer _pauseOverlay;

	private bool _lastPauseControlTick;
	private bool _pauseControlTick;
	private bool _isPaused;
	
	public override void _Ready()
	{
		_pauseOverlay = GetNode<CanvasLayer>("pause");
	}

	
	public override void _Process(double delta)
	{
		ControlTick();
		CheckPause();

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
}
