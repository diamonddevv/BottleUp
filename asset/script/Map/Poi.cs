using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Poi : StaticBody2D
{
	[Export] public PoiType PointOfInterestType { get; set; }
	[Export] public NodePath Map { get; set; }


	private float _sqrDistToPlayer;
	private Sprite2D _sprite;
	private Map _map;
	private Player _player;
	
	public override void _Ready()
	{
		_sprite = GetNode<Sprite2D>("sprite");
		_map = GetNode<Map>(Map);
	}

    public override void _Process(double delta)
	{

		if (_player == null)
		{
			_player = _map.GetPlayer();
		}
		else
		{
			_sqrDistToPlayer = (Position*_map.Scale).DistanceSquaredTo(_player.Position);
		}
	}

    public enum PoiType
	{
		Depot, Destination
	}
}
