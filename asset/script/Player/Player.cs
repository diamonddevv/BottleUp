using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class Player : CharacterBody2D
{

    [ExportCategory("Controls")]
    [Export] public Key Accelerate { get; set; } = Key.W;
    [Export] public Key Brake { get; set; } = Key.S;
    [Export] public Key LeftTurn { get; set; } = Key.A;
    [Export] public Key RightTurn { get; set; } = Key.D;
    [Export] public Key Handbrake { get; set; } = Key.Space;

	[ExportCategory("Movement/Physics")]
	[Export] public float ForwardDegreeOffset { get; set; } = 90;
	[Export] public float TurnSpeed { get; set; } = 2.5f;
	[Export] public float BaseAcceleration { get; set; } = .5f;
	[Export] public float MaxSpeed { get; set; } = 15;
	[Export] public float Friction { get; set; } = 1.005f;
	[Export] public float CameraSlide { get; set; } = 18f;

	[ExportCategory("Other Nodes")]
	[Export] public NodePath Camera;

	// other nodes
	private Camera2D _camera;

	// children
	private StackedSprite _sprite;
	private CollisionShape2D _shape;

	private float _turnMultiplier;
    private float _speed;
	private float _accelInputTick;
	private float _lastAccelInputTick;
	private float _lastTurnInputTick;
	private float _turnInputTick;
	private bool _lastHandbrakeInputTick;
	private bool _handbrakeInputTick;
	private float _twist;
	private Vector2 _cameraSlide;
	private Vector2 _maxCameraSlideVec;

	public override void _Ready()
	{
		_camera = GetNode<Camera2D>(Camera);

		_sprite = GetNode<StackedSprite>("sprite");
		_shape = GetNode<CollisionShape2D>("shape");

		_maxCameraSlideVec = BottleUpMath.Uniform(CameraSlide);
	}

	
	public override void _Process(double delta)
	{
		ControlTick();
		ApplyMovement(delta);

		_cameraSlide = Position + (BottleUpMath.DegToVec(RotationDegrees + ForwardDegreeOffset) * BottleUpMath.Lerp(0, CameraSlide, _speed / MaxSpeed));
		_camera.Position = _cameraSlide;
	}

    #region Movement Controller

	public void ControlTick()
	{
		_lastAccelInputTick = _accelInputTick;
		_lastTurnInputTick = _turnInputTick;
		_lastHandbrakeInputTick = _handbrakeInputTick;

        _accelInputTick = 0;
        _turnInputTick = 0;
		_handbrakeInputTick = false;

		if (Input.IsKeyPressed(Accelerate)) _accelInputTick += 1;
		if (Input.IsKeyPressed(Brake)) _accelInputTick -= 1;

		if (Input.IsKeyPressed(LeftTurn)) _turnInputTick += 1;
		if (Input.IsKeyPressed(RightTurn)) _turnInputTick -= 1;

		if (Input.IsKeyPressed(Handbrake)) _handbrakeInputTick = true;
	}
	
	public void ApplyMovement(double delta)
	{
		_turnMultiplier = 1;

		if (_accelInputTick != 0 && _accelInputTick == _lastAccelInputTick)
		{
			if (_speed < MaxSpeed && !_handbrakeInputTick) _speed += BaseAcceleration;
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
        RotationDegrees += rotForce * (_speed / MaxSpeed) * _turnMultiplier;

        _sprite.SetRotation(RotationDegrees + twistAdd);

		//todo : slow on grass collision

		Velocity += BottleUpMath.DegToVec(RotationDegrees + ForwardDegreeOffset) * _accelInputTick * (_speed / braker) * -1; // Apply Speed
		if (_speed != 0) Velocity /= Friction + .75f/_speed; // Apply Friction; allows player to stop moving when not inputting controls

		MoveAndSlide(); // do collisions if required
	}

    #endregion
}
