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
	[Export] public MainGameManager GameManager;
	[Export] public NodePath InGameTimer;
	[Export] public bool DisplayFPS = false;


    //
    private CanvasLayer _pauseOverlay;
    private Depot _depotGui;
    private Minimap _minimap;
    private Label _timerLabel;
    private Label _fpsLabel;
    private Label _reqsLabel;
    private Label _interactLabel;
    private Timer _timer;
    private Player _player;
    private PackedScene _mapScene;

	private bool _lastPauseControlTick;
	private bool _pauseControlTick;
	private bool _isPaused;
	private bool _isInDepot;
	
	public override void _Ready()
	{
		_pauseOverlay = GetNode<CanvasLayer>("pause");
		_depotGui = GetNode<Depot>("depot");

		_minimap = GetNode<Minimap>("ingame/margin/minimap");
        _timerLabel = GetNode<Label>("ingame/timer");
        _fpsLabel = GetNode<Label>("ingame/fps");
		_reqsLabel = GetNode<Label>("ingame/reqs");
		_interactLabel = GetNode<Label>("ingame/interact");

		_timer = GetNode<Timer>(InGameTimer);
		_player = GetNode<Player>(Player);

		_minimap.SetMapScene(MapScene);
		_minimap.SetPlayer(_player);
		_minimap.GameManager = GameManager;

        _depotGui.Hud = this;
        _depotGui.SetMapPlayer(_player);
        _depotGui.SetMapMapScene(MapScene);
        _depotGui.SetMapGameManager(GameManager);
    }

	
	public override void _Process(double delta)
	{
		ControlTick();
		CheckPause();

		if (_player.GetInteraction(out var poi))
		{
			_interactLabel.Visible = true;
            if (poi.PointOfInterestType == Poi.PoiType.Depot)
			{
				_interactLabel.Text = "Press 'E' to Enter Depot";
			} else
			{
				_interactLabel.Text = "Press 'E' to Deliver";
			}
		} else
		{
			_interactLabel.Visible = false;
		}

		if (_timer != null) _timerLabel.Text = BottleUpHelper.FormatTime(_timer.TimeLeft);

		_fpsLabel.Visible = DisplayFPS;
		if (DisplayFPS) _fpsLabel.Text = $"FPS: {BottleUpHelper.GetFramerate()}";

        _reqsLabel.Text = $"Unfulfilled Deliveries: {GameManager.GetActiveRequests().Count}\nCompleted Deliveries: {_player.GetCompletedDeliveries().Count}";

        GetTree().Paused = _isPaused;

		DepotTick(delta);
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

    public void DepotTick(double delta)
    {
        _depotGui.Visible = _isInDepot;

        if (_isInDepot)
        {
        }
    }

	public void OnDepotInteracted(Poi depot)
	{
		SetInDepot(true);
		_depotGui._Opened();
	}

    public void SetPaused(bool b) => _isPaused = b;
	public bool GetPaused() => _isPaused;

    public void SetInDepot(bool b) => _isInDepot = b;
    public bool GetInDepot() => _isInDepot;

    public Player GetPlayer() => _player;
	public Label GetRequestsLabel() => _reqsLabel;
	public Minimap GetMinimap() => _minimap;
	public Timer GetTimer() => _timer;
}
