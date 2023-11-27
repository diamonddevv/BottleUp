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
			SetPoiSprites();
		}
	} 
	[Export] public NodePath Map { get; set; }


    [Export] public Texture2D HouseSpritestacks;
    [Export] public Texture2D DepotSpritestacks;

    [Signal]
    public delegate void PoiDeliveryMadeEventHandler();

    [Signal]
    public delegate void PoiDepotInteractedEventHandler();


    private float _sqrDistToPlayer;
	private StackedSprite _sprite;
	private Node2D _entrance;
	private Map _map;
    private Player _player;

	private MainGameManager.DeliveryRequest? _delivery;
	
	public override void _Ready()
	{
		_sprite = GetNode<StackedSprite>("sprite");
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
        if (float.IsNaN(_sqrDistToPlayer)) return false;
        if (_sqrDistToPlayer <= _player.SquareArbMeterInteractDistanceThreshold / BottleUpMath.SQUARED_PIXELS_TO_ARB_METERS)
        {
            if (PointOfInterestType == PoiType.Depot || _delivery != null) _player.SetCanInteract(this, true);

            if (_player.GetIsInteracting())
            {
				return true;
            }

            return false;
        }
        _player.SetCanInteract(this, false);

        return false;
    }

    public void DoDepotPoiProcess(double delta)
    {
        if (CheckInteraction())
        {
            EmitSignal(SignalName.PoiDepotInteracted);
        }
    }

    public void DoDestPoiProcess(double delta)
    {
        if (CheckInteraction())
        {
            if (_delivery.HasValue)
            {
                var v = _delivery.Value;
                // Deliver Item
                EmitSignal(SignalName.PoiDeliveryMade);
                v.Made = true;
            }
			
        }
    }

	public void SetPoiSprites()
	{
        if (_sprite != null && HouseSpritestacks != null && DepotSpritestacks != null)
        {
            if (PointOfInterestType == PoiType.Depot)
            {
                _sprite.Texture = DepotSpritestacks;
                _sprite.Hframes = 9;
            }

            if (PointOfInterestType == PoiType.Destination)
            {
                _sprite.Texture = HouseSpritestacks;
                _sprite.Hframes = 8;
            }

            _sprite.DrawSprites();
        }
    }

	public MainGameManager.DeliveryRequest? GetDelivery() => _delivery;
    public void SetDelivery(MainGameManager.DeliveryRequest? d) => _delivery = d;

    public enum PoiType
	{
		Depot, Destination
	}

}
