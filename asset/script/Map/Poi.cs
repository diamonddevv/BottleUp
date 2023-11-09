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
			// Update Player Distance
			_sqrDistToPlayer = (Position*_map.Scale).DistanceSquaredTo(_player.Position);
		}

		if (PointOfInterestType == PoiType.Depot) (this as DepotPoi).DoPoiProcess(delta);
		if (PointOfInterestType == PoiType.Destination) (this as DeliveryDestinationPoi).DoPoiProcess(delta);
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

	public virtual void DoPoiProcess(double delta)
	{	
	}

    public enum PoiType
	{
		Depot, Destination
	}


    public partial class DepotPoi : Poi
    {
        public override void DoPoiProcess(double delta)
        {
            if (CheckInteraction())
			{

			}
        }
    }

    public partial class DeliveryDestinationPoi : Poi
	{
        public override void DoPoiProcess(double delta)
        {
            if (CheckInteraction())
			{

			}
        }

		public struct DeliveryRequest
		{

		}
    }
}
