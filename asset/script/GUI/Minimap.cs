using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[Tool]
public partial class Minimap : Control
{
	[ExportCategory("Minimap Options")]
	[Export] public Vector2 CameraZoom = BottleUpMath.Uniform(8f);
	[Export] public bool RotateEverything = false; // Rotates the entire map, rather than the player
	[Export] public bool DisplayClosestDestinationDistance = true;

	[ExportCategory("Minimap Icons")]
	[Export] public NodePath Player;

    [Export] public NodePath CompassPivot;
    [Export] public NodePath North;
	[Export] public NodePath East;
	[Export] public NodePath South;
	[Export] public NodePath West;

    [Export] public NodePath Depot;
    [Export] public NodePath Destination;

    private SubViewportContainer _container;
    private TextureRect _frame;
    private SubViewport _view;
	private Camera2D _cam;
	private Label _closestDestLabel;
	private Sprite2D _navArrow;

	private Sprite2D _playerIcon;

	private Node2D _compassPivot;
	private Sprite2D _north;
	private Sprite2D _east;
	private Sprite2D _south;
	private Sprite2D _west;

    private Sprite2D _depot;
    private Sprite2D _destinationIcon;
    private List<(Vector2 pos, Sprite2D icon)> _destinations;

    private Map _map;
	private Player _player;


	private int _minimapNavArrowDistIndex;
	private Vector2 _minimapDepotPosition;

	public override void _Ready()
	{
		_container = GetNode<SubViewportContainer>("container");
        _frame = GetNode<TextureRect>("container/frame");
        _view = GetNode<SubViewport>("container/view");
        _cam = GetNode<Camera2D>("container/view/cam");
        _closestDestLabel = GetNode<Label>("closestDestLabel");
		_navArrow = GetNode<Sprite2D>("navArrow");


        _playerIcon = GetNode<Sprite2D>(Player);

		_compassPivot = GetNode<Node2D>(CompassPivot);
		_north = GetNode<Sprite2D>(North);
		_east = GetNode<Sprite2D>(East);
		_south = GetNode<Sprite2D>(South);
		_west = GetNode<Sprite2D>(West);

        _depot = GetNode<Sprite2D>(Depot);
        _destinationIcon = GetNode<Sprite2D>(Destination);

        _cam.CustomViewport = _view;
        _cam.Position = Vector2.Zero;

        _player = null;
		_map = null;

		_frame.Size = _container.Size;
		_view.Size = _container.Size.RoundInts();
		_closestDestLabel.Position = _frame.Position + _frame.Size.Multiply(0, .6f) - _closestDestLabel.Size.Multiply(.5f, 0);
		_navArrow.Position = _frame.Position + _frame.Size.Multiply(0, .8f);

		_navArrow.Scale = BottleUpMath.Uniform(3);
    }

	
	public override void _Process(double delta)
	{
        _closestDestLabel.Visible = DisplayClosestDestinationDistance;

		if (!Engine.IsEditorHint())
		{
			(Vector2 pos, Sprite2D icon) closestDest = (-Vector2.Inf, null);
			float closestDestDist = float.NaN;

			_cam.Position = _player.Position;
			_cam.Zoom = Vector2.One / CameraZoom;

			_playerIcon.Position = _player.Position;

			_cam.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;
			_playerIcon.Rotation = _player.Rotation;

			_compassPivot.Position = Vector2.Zero + _frame.Size.Multiply(.5f, .5f);
			_compassPivot.Rotation = (RotateEverything ? 1 : 0) * -_player.Rotation;

			_north.Position = Vector2.Zero - _frame.Size.Multiply(0, .5f);
			_east.Position = Vector2.Zero + _frame.Size.Multiply(.5f, 0);
			_south.Position = Vector2.Zero - _frame.Size.Multiply(0, -.5f);
			_west.Position = Vector2.Zero + _frame.Size.Multiply(-.5f, 0);

			_north.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;
			_east.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;
			_south.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;
			_west.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;

			_depot.Position = _minimapDepotPosition;
			_depot.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;


			var scale = _map.Scale;
			if (_destinations != null)
			{
				foreach (var t in _destinations)
				{
					t.icon.Position = t.pos;
					t.icon.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;

					var dist = _player.Position.DistanceSquaredTo(t.pos * scale);
					if (float.IsNaN(closestDestDist) || dist < closestDestDist)
					{
						closestDest = t;
						closestDestDist = dist;
					}
				}
			}

            _closestDestLabel.Text = FormatLabelText(closestDestDist);

			if (float.IsNaN(closestDestDist))
			{
				_minimapNavArrowDistIndex = 3;
            } 
			else if (closestDestDist >= BottleUpMath.ArbMetersToSqrPx(70))
			{
				_minimapNavArrowDistIndex = 2;
			}
            else if (closestDestDist >= BottleUpMath.ArbMetersToSqrPx(30))
            {
                _minimapNavArrowDistIndex = 1;
            } else
			{
				_minimapNavArrowDistIndex = 0;
			}

			(_navArrow.Texture as AtlasTexture).Region = new Rect2(16 * _minimapNavArrowDistIndex, 32, 16, 16);
			var vec = ((closestDest.pos * scale) - _player.Position).Normalized();
			_navArrow.Rotation = Mathf.Atan2(vec.Y, vec.X);

        }
    }

	public void SetMapScene(PackedScene mapScene)
	{

		_destinations = new List<(Vector2 pos, Sprite2D icon)>();

        _map = mapScene.Instantiate<Map>();
        _view.AddChild(_map);

		foreach (var child in _map.GetChildren())
		{
			if (!(child is TileMap))
			{
				if (child is Poi poi)
				{
					if (poi.PointOfInterestType == Poi.PoiType.Depot)
					{
						_minimapDepotPosition = poi.Position;
					}

					if (poi.PointOfInterestType == Poi.PoiType.Destination)
					{
						_destinations.Add((poi.Position, _destinationIcon.Duplicate() as Sprite2D));
					}
				}

				child.QueueFree();
			}
		}
		_map.Position = Vector2.Zero;

        AddIcons();
    }
	public void SetPlayer(Player player) => _player = player;

	public void AddIcons()
	{
		// player icon
		_playerIcon.ZIndex = 5;
		_playerIcon.Scale *= 10;
		_playerIcon.Show();

		// Cardinal Directions
		_compassPivot.Show();

        // N
        _north.ZIndex = 8;
        _north.Scale *= 2;

        // E
        _east.ZIndex = 8;
        _east.Scale *= 2;

        // S
        _south.ZIndex = 8;
        _south.Scale *= 2;

        // W
        _west.ZIndex = 8;
        _west.Scale *= 2;

		// POIs
		// Depot
		_depot.GetParent().RemoveChild(_depot);
		_map.AddChild(_depot);
        _depot.ZIndex = 10;
		_depot.Scale *= 3.5f;
		_depot.TextureFilter = TextureFilterEnum.Nearest;
		_depot.Show();

        // Destination
		foreach (var t in _destinations)
		{
            _map.AddChild(t.icon);
            t.icon.ZIndex = 10;
            t.icon.Scale *= 3.5f;
            t.icon.TextureFilter = TextureFilterEnum.Nearest;
			t.icon.Show();
        }
    }

	public static string FormatLabelText(float dist)
	{
		string d = "";
		if (float.IsNaN(dist))
		{
			d = "None!";
		} else
		{
			float arbMeters = BottleUpMath.SqrPxToArbMeters(dist);
			if (arbMeters < 100)
			{
				arbMeters = (float)Math.Round(arbMeters, 2);
			}
            d = $"{arbMeters}m";
		}

		return $"Closest Destination: \n{d}";
	}
}
