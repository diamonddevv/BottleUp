using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class Map : TileMap
{
	[Export] public NodePath Player { get; set; }

	private Player _player;

	public override void _Ready()
	{
		if (Player != null) _player = GetNode<Player>(Player);
	}

	
	public override void _Process(double delta)
	{
	}

	public Player GetPlayer() => _player;
}
