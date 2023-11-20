using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class Map : TileMap
{
	[Export] public NodePath Player { get; set; }

    [Signal]
    public delegate void DeliveryMadeEventHandler(Poi pointOfInterest);


    private Player _player;

	public override void _Ready()
	{
		if (Player != null) _player = GetNode<Player>(Player);

		foreach (var child in GetChildren())
		{
			if (child is Poi poi)
			{
				poi.PoiDeliveryMade += () => EmitSignal(SignalName.DeliveryMade, poi);
			}
		}
	}

	
	public override void _Process(double delta)
	{
	}

	public Player GetPlayer() => _player;
}
