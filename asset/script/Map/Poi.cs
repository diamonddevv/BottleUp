using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Security.Cryptography.X509Certificates;
using static BottleUp.asset.script.Game.DeliverableItems;
using static BottleUp.asset.script.Util.BottleUpHelper;

public partial class Poi : StaticBody2D
{
	private PoiType _poiType = PoiType.Destination;

    [Export] public PoiType PointOfInterestType 
	{
		get => _poiType;
		set
		{
			_poiType = value;
			
			if (_poiType == PoiType.Depot)
			{
				_destSprites.Hide();
				_depotSprite.Show();
			} else if (_poiType == PoiType.Destination)
			{
				_destSprites.Show();
				_depotSprite.Hide();
			}
		}
	} 
	[Export] public NodePath Map { get; set; }


    [Signal]
    public delegate void PoiDeliveryMadeEventHandler();


    private float _sqrDistToPlayer;
	private Sprite2D _destSprites;
	private Sprite2D _depotSprite;
	private Node2D _entrance;
	private Map _map;
	private Player _player;

	private MainGameManager.DeliveryRequest _delivery;
	
	public override void _Ready()
	{
		_destSprites = GetNode<Sprite2D>("destSprite");
		//_depotSprite = GetNode<Sprite2D>("depotSprite");
		_entrance = GetNode<Node2D>("entrance");
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
			// Update Player Distance
			_sqrDistToPlayer = ((Position + _entrance.Position) * _map.Scale).DistanceSquaredTo(_player.Position);
		}

		if (PointOfInterestType == PoiType.Depot) DoDepotPoiProcess(delta);
		if (PointOfInterestType == PoiType.Destination) DoDestPoiProcess(delta);
	}

	public bool CheckInteraction()
	{
        if (_sqrDistToPlayer <= _player.SquareInteractDistanceThreshold)
        {
            if (_player.GetIsInteracting())
            {
				return true;
            }
        }
		return false;
    }

    public void DoDepotPoiProcess(double delta)
    {
        if (CheckInteraction())
        {
			"asd".Test();
			// Open Depot UI
        }
    }

    public void DoDestPoiProcess(double delta)
    {
        if (CheckInteraction())
        {
			// Deliver Item
			EmitSignal(SignalName.PoiDeliveryMade);

			_delivery.Made = true;
        }
    }

	public MainGameManager.DeliveryRequest GetDelivery() => _delivery;

    public enum PoiType
	{
		Depot, Destination
	}

}
