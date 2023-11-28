using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class Map : TileMap
{
	private MainGameManager _mainGameManager;

	[Export] public NodePath Player { get; set; }

    [Signal]
    public delegate void DeliveryMadeEventHandler(Poi pointOfInterest);

    [Signal]
    public delegate void DepotInteractedEventHandler(Poi depot);


	public MainGameManager GameManager
	{
		get => _mainGameManager;
		set
		{
			_mainGameManager = value;
			foreach (var child in GetChildren())
			{
				if (child is Poi poi)
				{
					poi.GameManager = value;
				}
			}
		}
	}

    private Player _player;
	private List<Poi> _destinations;
	private Poi _depot;

	public override void _Ready()
	{
		_destinations = new List<Poi>();

		if (Player != null) _player = GetNode<Player>(Player);

		foreach (var child in GetChildren())
		{
			if (child is Poi poi)
			{
				if (poi.PointOfInterestType == Poi.PoiType.Destination) _destinations.Add(poi);
				if (poi.PointOfInterestType == Poi.PoiType.Depot) _depot = poi; // there should only be one depot

				poi.PoiDeliveryMade += () => EmitSignal(SignalName.DeliveryMade, poi);
				poi.PoiDepotInteracted += () => EmitSignal(SignalName.DepotInteracted, poi);
				poi.SetPoiSprites();
			}
		}
	}

	
	public override void _Process(double delta)
	{
	}

	public Player GetPlayer() => _player;
	public List<Poi> GetDestinations() => _destinations;
	public Poi GetDepot() => _depot;
}
