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

	[ExportCategory("Movement/Physics")]
	[Export] public float ForwardDegreeOffset { get; set; } = -90;
	[Export] public float BaseTurnSpeed { get; set; } = 2;
	[Export] public float BaseAcceleration { get; set; } = 2;
	[Export] public float MaxSpeed { get; set; } = 30;
	[Export] public float Friction { get; set; } = 1.05f;


	private float _speed;

	private float _accelInputTick;
	private float _lastAccelInputTick;
	private float _turnInputTick;

	public override void _Ready()
	{
		Scale = BottleUpMath.Uniform(2);
	}

	
	public override void _Process(double delta)
	{
		ControlTick();
		ApplyMovement();
	}

    #region Movement Controller

	public void ControlTick()
	{
		_lastAccelInputTick = _accelInputTick;

        _accelInputTick = 0;
        _turnInputTick = 0;

		if (Input.IsKeyPressed(Accelerate)) _accelInputTick += 1;
		if (Input.IsKeyPressed(Brake)) _accelInputTick -= 1;

		if (Input.IsKeyPressed(LeftTurn)) _turnInputTick += 1;
		if (Input.IsKeyPressed(RightTurn)) _turnInputTick -= 1;
	}
	
	public void ApplyMovement()
	{
		if (_accelInputTick != 0 && _accelInputTick == _lastAccelInputTick)
		{
			if (_speed < MaxSpeed) _speed += BaseAcceleration;
		} else
		{
            if (_speed > 0) _speed -= BaseAcceleration;
			if (_speed < 0) _speed = 0;
		}

		

		var braker = _accelInputTick < 0 ? 5 : 1;
		var rotForce = BaseTurnSpeed * _speed * .05f;

        RotationDegrees += _turnInputTick * rotForce.Test() * -1;

		Velocity += BottleUpMath.DegToVec(RotationDegrees + ForwardDegreeOffset) * _accelInputTick * (_speed / braker) * -1; // Apply Speed
		Velocity /= Friction; // Apply Friction; allows player to stop moving when not inputting controls

		MoveAndSlide(); // do collisions if required
	}

    #endregion
}
