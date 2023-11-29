using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static BottleUp.asset.script.Util.BottleUpHelper;
using static MainGameManager;

public partial class Player : CharacterBody2D
{

	[ExportCategory("Controls")]
	[Export] public Key Accelerate { get; set; } = Key.W;
	[Export] public Key Brake { get; set; } = Key.S;
	[Export] public Key LeftTurn { get; set; } = Key.A;
	[Export] public Key RightTurn { get; set; } = Key.D;
	[Export] public Key Handbrake { get; set; } = Key.Space;
	[Export] public Key Boost { get; set; } = Key.Shift;
	[Export] public Key Interact { get; set; } = Key.E;
	[Export] public Key FullMap { get; set; } = Key.M;

	[ExportCategory("Movement/Physics")]
	[Export] public float ForwardDegreeOffset { get; set; } = 90;
	[Export] public float TurnSpeed { get; set; } = 2.5f;
	[Export] public float BaseAcceleration { get; set; } = .5f;
	[Export] public float MaxSpeed { get; set; } = 15;
	[Export] public float BoostSpeedMultiplier { get; set; } = 1.5f;
	[Export] public float BoostMaxTimeSeconds { get; set; } = 2f;
	[Export] public float BoostRegainTimeSeconds { get; set; } = 5f;
	[Export] public float Friction { get; set; } = 1.005f;
	[Export] public float GrassFriction { get; set; } = 1.15f;
	[Export] public float CameraSlide { get; set; } = 18f;
	[Export] public float SquareArbMeterInteractDistanceThreshold { get; set; } = 5f;

	[ExportCategory("Misc")]
	[Export] public Vector2 DefaultCameraZoomFactor = new Vector2(4, 4);
	[Export] public Vector2 FullMapZoomFactor = new Vector2(.1f, .1f);
	[Export] public float FullMapTimeToTransition = .75f;
	[Export] public Vector2 FullmapTargetPos = new Vector2(0, -1000f);

    [ExportCategory("Other Nodes")]
	[Export] public Camera2D Camera;
	[Export] public HUD Hud;

	// children
	private StackedSprite _sprite;
	private Area2D _grassCollider;
	private PlayerInventoryHandler _inventory;
	private AudioStreamPlayer2D _engineStreamPlayer;
	private AudioStreamPlayer2D _hornStreamPlayer;
	private AudioStreamPlayer2D _handbrakeStreamPlayer;

	private float _turnMultiplier;
	private float _speed;
	private float _accelInputTick;
	private float _lastAccelInputTick;
	private float _lastTurnInputTick;
	private float _turnInputTick;
	private bool _lastHandbrakeInputTick;
	private bool _handbrakeInputTick;
	private bool _boostInputTick;
	private bool _lastInteractInputTick;
	private bool _interactInputTick;
    private bool _lastFullmapInputTick;
    private bool _fullmapInputTick;
    private float _twist;
	private Vector2 _cameraSlide;
	private Vector2 _maxCameraSlideVec;
	private bool _onGrass;
	private float _boostTime;
	private float _mapTransitionTime;
	private bool _mapTransitionState;
	private bool _isFullMap;
	private Vector2 _fullMapTargetPos;
    private List<DeliveryRequest> _completedDeliveries;
    private Rating _rating;
	private Dictionary<Poi, bool> _interactables;

    public override void _Ready()
	{

		_sprite = GetNode<StackedSprite>("sprite");
		_grassCollider = GetNode<Area2D>("grassCollider");
		_inventory = GetNode<PlayerInventoryHandler>("inventoryHandler");
		_engineStreamPlayer = GetNode<AudioStreamPlayer2D>("engine");
		_hornStreamPlayer = GetNode<AudioStreamPlayer2D>("horn");
		_handbrakeStreamPlayer = GetNode<AudioStreamPlayer2D>("handbrake");

		_maxCameraSlideVec = BottleUpMath.Uniform(CameraSlide);


        Camera.Zoom = DefaultCameraZoomFactor;

        // Add GrassCollider Properties
        _grassCollider.BodyEntered += (body) =>
		{
			if (body is Map map)
			{
				if (((byte)map.TileSet.GetPhysicsLayerCollisionMask(0)).IsBitSet(0))
				{
					_onGrass = true;
				}
			}
		};

		_grassCollider.BodyExited += (body) =>
		{

			if (body is Map map)
			{
				if (((byte)map.TileSet.GetPhysicsLayerCollisionMask(0)).IsBitSet(0))
				{
					_onGrass = false;
				}
			}
		};

		Hud.GameManager.DeliveryMade += () =>
		{
			HonkHorn(GD.Randf());
		};

        _interactables = new Dictionary<Poi, bool>();
        _completedDeliveries = new List<DeliveryRequest>();
	}

	
	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		ControlTick();
		ApplyMovement(delta);

        EngineSoundTick();
        // HandbrakeSoundTick(_handbrakeInputTick, _turnInputTick); // doesnt sound right. future me problem

        _cameraSlide = Position + (BottleUpMath.DegToVec(RotationDegrees + ForwardDegreeOffset) * BottleUpMath.Lerp(0, CameraSlide, _speed / MaxSpeed));

        if (!_isFullMap) Camera.Position = _cameraSlide;
		else Camera.Position = _fullMapTargetPos;

		if (_fullmapInputTick && !_lastFullmapInputTick)
		{
			_mapTransitionState = true;
			_isFullMap = !_isFullMap;
			_mapTransitionTime = 0;
		}

        if (_mapTransitionState)
		{
			var pos = _isFullMap ? Position : FullmapTargetPos;
			var newPos = _isFullMap ? FullmapTargetPos : Position;
			var weight = Mathf.Clamp(_mapTransitionTime / FullMapTimeToTransition, 0, 1);

            Camera.Zoom = Camera.Zoom.Lerp(_isFullMap ? FullMapZoomFactor : DefaultCameraZoomFactor, weight);
			_fullMapTargetPos = pos.Lerp(newPos, weight);

            _mapTransitionTime += (float)delta;

			if (weight >= 1)
			{
				_mapTransitionState = false;
			}
		}
        
    }

    private Vector2 CalculateReturnPos() => GetWindow().GetVisibleRect().Size.Multiply(1, 0).Add(-250 / 2, 250 / 2) - new Vector2(20, -20);

    public void ControlTick()
	{
		_lastAccelInputTick = _accelInputTick;
		_lastTurnInputTick = _turnInputTick;
		_lastHandbrakeInputTick = _handbrakeInputTick;
		_lastInteractInputTick = _interactInputTick;
		_lastFullmapInputTick = _fullmapInputTick;

		_accelInputTick = 0;
		_turnInputTick = 0;
		_handbrakeInputTick = false;
		_boostInputTick = false;
		_interactInputTick = false;
		_fullmapInputTick = false;


		if (!Hud.GetInDepot()) // if you're allowed to move
		{

			if (Input.IsKeyPressed(Accelerate)) _accelInputTick += 1;
			if (Input.IsKeyPressed(Brake)) _accelInputTick -= 1;

			if (Input.IsKeyPressed(LeftTurn)) _turnInputTick += 1;
			if (Input.IsKeyPressed(RightTurn)) _turnInputTick -= 1;

			if (Input.IsKeyPressed(Handbrake)) _handbrakeInputTick = true;
			if (Input.IsKeyPressed(Boost)) _boostInputTick = true;

			if (Input.IsKeyPressed(Interact)) _interactInputTick = true;
			if (Input.IsKeyPressed(FullMap)) _fullmapInputTick = true;

		}
	}

	#region Movement Controller

	public void ApplyMovement(double delta)
	{
		var frameMaxSpeed = MaxSpeed;
		if (_boostInputTick && _boostTime > 0)
		{
			frameMaxSpeed = MaxSpeed * BoostSpeedMultiplier;
			_boostTime -= (float)delta;
		} else
		{
			if (!_boostInputTick && _boostTime < BoostMaxTimeSeconds) _boostTime += (float)delta / (BoostRegainTimeSeconds / BoostMaxTimeSeconds);
		}

		_turnMultiplier = 1;

		if (_accelInputTick != 0 && _accelInputTick == _lastAccelInputTick)
		{
			if (_speed < frameMaxSpeed && !_handbrakeInputTick) _speed += BaseAcceleration;
			if (_speed > frameMaxSpeed) _speed -= BaseAcceleration;
		} else
		{
			if (_speed > 0) _speed -= BaseAcceleration;
			if (_speed < 0) _speed = 0;
		}

		if (_handbrakeInputTick)
		{
			if (BottleUpMath.IsOfPercentageThreshold(_speed, MaxSpeed, 0.75f))
			{
                GetInventory().Damage(0.05f * _speed / 4);
            }

			_speed -= _speed / 100 * 1.5f;
			_turnMultiplier *= 1.45f;
		}

		var braker = _accelInputTick < 0 ? 5 : 1;
		var rotForce = _turnInputTick * TurnSpeed * -1;

		var twistAdd = (_turnInputTick * -20) + (_handbrakeInputTick ? _turnInputTick * -15 : 0);
		RotationDegrees += rotForce * (_speed / frameMaxSpeed) * _turnMultiplier;

		_sprite.SetRotation(RotationDegrees + twistAdd);


		Velocity += BottleUpMath.DegToVec(RotationDegrees + ForwardDegreeOffset) * _accelInputTick * (_speed / braker) * -1; // Apply Speed
		if (_speed != 0) Velocity /= Friction + .75f/_speed; // Apply Friction; allows player to stop moving when not inputting controls

		// do collisions if required
		DoCollisions(delta);
	}

	public void DoCollisions(double delta)
	{
		if (_onGrass && _speed != 0) Velocity /= GrassFriction + .25f / _speed;

		bool collided = MoveAndSlide();

		if (collided)
		{
			GetInventory().Damage(0.05f * _speed / 2);
        }
	}

	#endregion

	#region Getters/Setters
	public bool GetIsInteracting() => _interactInputTick && !_lastInteractInputTick;
	public PlayerInventoryHandler GetInventory() => _inventory;

	public List<DeliveryRequest> GetCompletedDeliveries() => _completedDeliveries;
	public Rating GetRating() => _rating;


	public void SetRating(Rating rating) => _rating = rating;
	#endregion

	#region Audio Handlers

	public void EngineSoundTick()
	{
		if (!_engineStreamPlayer.Playing) _engineStreamPlayer.Play();

		_engineStreamPlayer.PitchScale = .25f + (_speed / 25);
	}

	public void HonkHorn(float pitchShift)
	{
        _hornStreamPlayer.PitchScale = 1 + pitchShift;
        _hornStreamPlayer.Play();
    }

    public void HandbrakeSoundTick(bool handbraking, float turnInput)
    {
		if (_speed > 0) _handbrakeStreamPlayer.PitchScale = _speed / 5f;
		_handbrakeStreamPlayer.Playing = handbraking;
		if (_speed <= 10) _handbrakeStreamPlayer.Stop();
    }

    #endregion

    public void SetCanInteract(Poi poi, bool close)
	{
		if (_interactables.ContainsKey(poi))
		{
			_interactables[poi] = close;
		}
		else
		{
			_interactables.Add(poi, close);
		}
	}

	public bool GetInteraction(out Poi poi)
	{
		poi = null;

		if (_interactables.Any(kv => kv.Value))
		{
			poi = _interactables.First(kv => kv.Value).Key;
			return true;
		}
		else return false;
	}
}
