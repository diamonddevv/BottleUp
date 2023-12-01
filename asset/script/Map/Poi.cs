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

    public MainGameManager GameManager { get; set; }

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
	private GpuParticles2D _deliveryParticles;
	private Node2D _entrance;
	private Map _map;
    private Player _player;

	private MainGameManager.DeliveryRequest? _delivery;
	
	public override void _Ready()
	{
		_sprite = GetNode<StackedSprite>("sprite");
		_entrance = GetNode<Node2D>("entrance");
		_deliveryParticles = GetNode<GpuParticles2D>("deliveryParticles");
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
            if ((PointOfInterestType == PoiType.Depot || _delivery != null) && IsDeliveryPossibleWithInventoryContents(_player.GetInventory())) 
                _player.SetCanInteract(this, true);

            if (_player.GetIsInteracting())
            {
                bool destReqMet = PointOfInterestType == PoiType.Depot ? true : IsDeliveryPossibleWithInventoryContents(_player.GetInventory());
				if (destReqMet) return true;
            }

            return false;
        }
        _player.SetCanInteract(this, false);

        return false;
    }

    public bool IsDeliveryPossibleWithInventoryContents(PlayerInventoryHandler inventory)
    {
        if (PointOfInterestType == PoiType.Depot) return true;

        if (_delivery.HasValue)
        {
            var v = _delivery.Value;
            int matches = v.Items.Count;
            foreach (var item in v.Items)
            {
                int count = item.count;
                EnumItem enumItem = item.item;
                foreach (var invItem in inventory.GetCarriedItemsWithCount())
                {
                    if (invItem.Key.item == enumItem)
                    {
                        if (invItem.Value <= count)
                        {
                            matches -= 1;

                            if (matches <= 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public void SubtractRequestItems(PlayerInventoryHandler inventory)
    {
        if (_delivery.HasValue)
        {
            var v = GameManager.GetActiveRequests()[this];
            foreach (var item in v.Items)
            {
                int count = item.count;
                EnumItem enumItem = item.item;
                inventory.RemoveItemOfType(enumItem, count);
            }
        }
    }

    public void SetRequestItemIntactness(PlayerInventoryHandler inventory)
    {
        if (_delivery.HasValue)
        {
            _delivery.Value.SetIntactness(inventory);
        }
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
                // Deliver Item; Assume delivery is possible here

                _deliveryParticles.Emitting = true;
                SetRequestItemIntactness(_player.GetInventory());

                v.SetRatings(_player.Hud.GetTimer().TimeLeft);
                v.Made = true;

                SubtractRequestItems(_player.GetInventory());
                EmitSignal(SignalName.PoiDeliveryMade);
                GameManager.GetActiveRequests().Remove(this);
                GameManager.MarkRequestUpdate();

                

                CheckInteraction();

                _delivery = null;
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
