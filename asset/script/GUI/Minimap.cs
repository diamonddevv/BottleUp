using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

[Tool]
public partial class Minimap : Control
{
    public const string SHADER_ENABLED = "_enabled";

    public const string NO_REQUEST = "unreq";
	public const string REQUEST = "req";
	public const string SELECTED = "sel";

	[ExportCategory("Minimap Options")]
	[Export] public Vector2 CameraZoom = BottleUpMath.Uniform(8f);
	[Export] public bool RotateEverything = true; // Rotates the entire map, rather than the player
	[Export] public bool DisplayClosestDestinationDistance = true;
	[Export] public bool FullClickable = false;

	[ExportCategory("Minimap Icons")]
	[Export] public NodePath Player;

    [Export] public NodePath CompassPivot;
    [Export] public NodePath North;
	[Export] public NodePath East;
	[Export] public NodePath South;
	[Export] public NodePath West;

    [Export] public NodePath Depot;
    [Export] public NodePath Destination;


    [Signal]
    public delegate void MapFullClickedEventHandler(bool isNowFull);

    public MainGameManager GameManager { get; set; }

    private SubViewportContainer _container;
    private TextureRect _frame;
    private SubViewport _view;
	private Camera2D _cam;
	private Label _closestDestLabel;
	private Label _clickToFullLabel;
	private Sprite2D _navArrow;

	private Sprite2D _playerIcon;

	private Node2D _compassPivot;
	private Sprite2D _north;
	private Sprite2D _east;
	private Sprite2D _south;
	private Sprite2D _west;

    private Sprite2D _depot;
    private AnimatedSprite2D _destinationIcon;
    private List<(Vector2 pos, AnimatedSprite2D icon)> _destinations;

    private Map _map;
	private Player _player;

	private int _minimapNavArrowDistIndex;
	private Vector2 _minimapDepotPosition;

	private bool _isFull;
	private bool _isMouseOver;
	private bool _clickTick;
	private bool _lastClickTick;

    public override void _Ready()
	{
		_container = GetNode<SubViewportContainer>("container");
        _frame = GetNode<TextureRect>("container/frame");
        _view = GetNode<SubViewport>("container/view");
        _cam = GetNode<Camera2D>("container/view/cam");
        _closestDestLabel = GetNode<Label>("closestDestLabel");
        _clickToFullLabel = GetNode<Label>("clickToFullLabel");
		_navArrow = GetNode<Sprite2D>("navArrow");


        _playerIcon = GetNode<Sprite2D>(Player);

		_compassPivot = GetNode<Node2D>(CompassPivot);
		_north = GetNode<Sprite2D>(North);
		_east = GetNode<Sprite2D>(East);
		_south = GetNode<Sprite2D>(South);
		_west = GetNode<Sprite2D>(West);

        _depot = GetNode<Sprite2D>(Depot);
        _destinationIcon = GetNode<AnimatedSprite2D>(Destination);

        _cam.CustomViewport = _view;
        _cam.Position = Vector2.Zero;

        _player = null;
		_map = null;

		_frame.Size = _container.Size;
		_view.Size = _container.Size.RoundInts();
		_closestDestLabel.Position = _frame.Position + _frame.Size.Multiply(0, .6f) - _closestDestLabel.Size.Multiply(.5f, 0);
		_navArrow.Position = _frame.Position + _frame.Size.Multiply(0, .82f);

		_navArrow.Scale = BottleUpMath.Uniform(3);

		MouseEntered += () => _isMouseOver = true;
		MouseExited += () => _isMouseOver = false;
    }

	
	public override void _Process(double delta)
	{
		_lastClickTick = _clickTick;
		_clickTick = _isMouseOver && Input.IsMouseButtonPressed(MouseButton.Left);

		if (_clickToFullLabel != null) _clickToFullLabel.Visible = FullClickable;

		if (_clickTick && !_lastClickTick && FullClickable)
		{
            _isFull = !_isFull;
            EmitSignal(SignalName.MapFullClicked, _isFull);
        }

        _closestDestLabel.Visible = DisplayClosestDestinationDistance;
		_navArrow.Visible = DisplayClosestDestinationDistance;

		_frame.Visible = !_isFull;

		if (_isFull)
		{
			var v = GetViewport().GetVisibleRect().Size.Multiply(.75f, 1.3f).RoundInts();
			_view.Size = v;
		} else
		{
			_view.Size = new Vector2I(250, 250);
		}

		if (!Engine.IsEditorHint())
		{
			(Vector2 pos, AnimatedSprite2D icon) closestDest = (-Vector2.Inf, null);
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
					bool hasReq = GameManager.GetActiveRequests().Any(kvp => kvp.Key.Position == t.pos);

                    t.icon.Position = t.pos;
                    t.icon.Rotation = (RotateEverything ? 1 : 0) * _player.Rotation;

                    if (hasReq)
					{
						t.icon.Play(REQUEST);

						var dist = _playerIcon.Position.DistanceSquaredTo(t.pos * scale);
						if (float.IsNaN(closestDestDist) || dist < closestDestDist)
						{
							closestDest = t;
							closestDestDist = dist;
						}
					} else
					{
						t.icon.Play(NO_REQUEST);
					}

                    (t.icon.Material as ShaderMaterial).SetShaderParameter(SHADER_ENABLED, false);
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

			_navArrow.Rotation = CompassDirRad(_playerIcon.Position, closestDest.pos * scale);
        }
    }

	public void SetMapScene(PackedScene mapScene)
	{

		_destinations = new List<(Vector2 pos, AnimatedSprite2D icon)>();

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
						_destinations.Add((poi.Position, _destinationIcon.Duplicate() as AnimatedSprite2D));
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
			t.icon.Material = t.icon.Material.Duplicate() as Material;
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
            arbMeters = (float)Math.Round(arbMeters, 1);
            d = $"{arbMeters}m";
		}

		return $"Closest Destination: \n{d}";
	}

    public static float CompassDirRad(Vector2 from, Vector2 to)
    {
        // i thought +90 meant adding a right angle then i remembered this is not degrees this is radians and that is not a right angle
        // tldr; i dont know how this works
		return from.DirectionTo(to).Angle() + 90;
    }


	public void SetFull(bool b) => _isFull = b;
	public SubViewport GetView() => _view;
	public Map GetMap() => _map;

	public AnimatedSprite2D GetDestPoiIcon(Poi poi)
	{
		foreach (var t in _destinations)
		{
			if (t.pos == poi.Position)
			{
				return t.icon;
			}
		}
		return null;
	}
}
