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
	[Export] public float Speed { get; set; } = 10;
	[Export] public float Friction { get; set; } = 1.05f;


	private Vector2 _inputTick;

	public override void _Ready()
	{
	}

	
	public override void _Process(double delta)
	{
		ControlTick();
		ApplyMovement();
	}

    #region Movement Controller

	public void ControlTick()
	{
		_inputTick = Vector2.Zero;

		if (Input.IsKeyPressed(Accelerate)) _inputTick.Y += 1;
		if (Input.IsKeyPressed(Brake)) _inputTick.Y -= 1;
		if (Input.IsKeyPressed(LeftTurn)) _inputTick.X += 1;
		if (Input.IsKeyPressed(RightTurn)) _inputTick.X -= 1;
	}
	
	public void ApplyMovement()
	{
		Velocity += _inputTick * Speed * -1; // Apply Speed
		Velocity /= Friction; // Apply Friction; allows player to stop moving when not inputting controls

		MoveAndSlide(); // do collisions if required
	}

    #endregion
}
