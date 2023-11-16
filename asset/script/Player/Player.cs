using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Linq;

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
	[Export] public float SquareInteractDistanceThreshold { get; set; } = 8e3f;

	[ExportCategory("Misc")]
	[Export] public Vector2 DefaultCameraZoomFactor = new Vector2(4, 4);
	[Export] public Vector2 FullMapZoomFactor = new Vector2(.1f, .1f);
	[Export] public float FullMapTimeToTransition = .5f;

	[ExportCategory("Other Nodes")]
	[Export] public NodePath Camera;

	// other nodes
	private Camera2D _camera;

	// children
	private StackedSprite _sprite;
	private Area2D _grassCollider;
	private PlayerInventoryHandler _inventory;

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

	public override void _Ready()
	{
		_camera = GetNode<Camera2D>(Camera);

		_sprite = GetNode<StackedSprite>("sprite");
		_grassCollider = GetNode<Area2D>("grassCollider");
		_inventory = GetNode<PlayerInventoryHandler>("inventoryHandler");

		_maxCameraSlideVec = BottleUpMath.Uniform(CameraSlide);


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
	}

	
	public override void _Process(double delta)
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		ControlTick();
		ApplyMovement(delta);

		_cameraSlide = Position + (BottleUpMath.DegToVec(RotationDegrees + ForwardDegreeOffset) * BottleUpMath.Lerp(0, CameraSlide, _speed / MaxSpeed));
		_camera.Position = _cameraSlide;

		if (_fullmapInputTick && !_lastFullmapInputTick)
		{
			_mapTransitionState = true;
			_isFullMap = !_isFullMap;
			_mapTransitionTime = 0;
		}

        if (_mapTransitionState)
		{
			_camera.Zoom = _camera.Zoom.Lerp(_isFullMap ? FullMapZoomFactor : DefaultCameraZoomFactor, Mathf.Clamp(_mapTransitionTime / FullMapTimeToTransition, 0, 1));

			_mapTransitionTime += (float)delta;
		}

    }

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

		if (Input.IsKeyPressed(Accelerate)) _accelInputTick += 1;
		if (Input.IsKeyPressed(Brake)) _accelInputTick -= 1;

		if (Input.IsKeyPressed(LeftTurn)) _turnInputTick += 1;
		if (Input.IsKeyPressed(RightTurn)) _turnInputTick -= 1;

		if (Input.IsKeyPressed(Handbrake)) _handbrakeInputTick = true;
		if (Input.IsKeyPressed(Boost)) _boostInputTick = true;

		if (Input.IsKeyPressed(Interact)) _interactInputTick = true;
		if (Input.IsKeyPressed(FullMap)) _fullmapInputTick = true;
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

		MoveAndSlide();
	}

	#endregion

	#region Getters
	public bool GetIsInteracting() => _interactInputTick && !_lastInteractInputTick;
	public PlayerInventoryHandler GetInventory() => _inventory;
	#endregion

}
