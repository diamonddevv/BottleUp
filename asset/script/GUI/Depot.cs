using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;

public partial class Depot : CanvasLayer
{
	public HUD Hud { get; set; }

	[Export] private Control ItemButton;

	private Minimap _map;
	private Button _leaveButton;
	private Label _scaleWeightLabel;
	private VBoxContainer _items;

	private List<DeliverableItems.EnumItem> _itemsToAdd;
	private int _weightAdded;
	private bool _setMapPlayer;
	private Vector2 _oldMapPos;
	private Vector2 _unfillScreenOffset;
	private bool _isMapFull;

	public override void _Ready()
	{
		_map = GetNode<Minimap>("minimap");
		_leaveButton = GetNode<Button>("leaveButton");
		_items = GetNode<VBoxContainer>("items");
        _scaleWeightLabel = GetNode<Label>("scale/weight/numeral");

        _leaveButton.Pressed += () =>
		{
			Hud.SetInDepot(false);
		};

		_map.MapFullClicked += (state) =>
		{
			_isMapFull = state;

			if (state)
			{
				_map.Position = (GetWindow().GetVisibleRect().Size / 2).Multiply(1, 1.5f);
            } else
			{
				_map.Position = CalculateReturnPos();
            }
        };

		_unfillScreenOffset = new Vector2(20, -20);


		GetWindow().SizeChanged += () => { if (!_isMapFull) _map.Position = CalculateReturnPos(); };

		foreach (var itemType in DeliverableItems.Items)
		{
			var enumItem = itemType.Enum;
            DepotItemButton clonedItemButton = ItemButton.Duplicate(4) as DepotItemButton;
            _items.AddChild(clonedItemButton);

            clonedItemButton.Item.UniqueifyBottleTexture();
            clonedItemButton.Item.UniqueifyItemTexture();
            clonedItemButton.Item.UniqueifyMilkShader();

            clonedItemButton.Item.Item = enumItem;
			clonedItemButton.Item.UseInventoryLabel = true;
			clonedItemButton.Item.ShowDescription = true;


			clonedItemButton.AddButton.Pressed += () =>
			{
				clonedItemButton.Item.Count += 1;
				_weightAdded += itemType.MilkUnitWeight;
				_itemsToAdd.Add(enumItem);
			};
            clonedItemButton.SubtractButton.Pressed += () =>
            {
				if (clonedItemButton.Item.Count > 0)
				{
                    clonedItemButton.Item.Count -= 1;
                    _weightAdded -= itemType.MilkUnitWeight;
                    _itemsToAdd.Remove(enumItem);
                }
            };
			clonedItemButton.Visible = true;

		}

	}

	public void _Opened()
	{
		_map.SetFull(false);
		_map.Position = CalculateReturnPos();

		_itemsToAdd = new List<DeliverableItems.EnumItem>();
		_weightAdded = 0;
	}

	public override void _Process(double delta)
	{
		if (_scaleWeightLabel != null) _scaleWeightLabel.Text = $"+{_weightAdded}";
	}

	public void SetMapPlayer(Player p) => _map.SetPlayer(p);
	public void SetMapMapScene(PackedScene p) => _map.SetMapScene(p);
	public void SetMapGameManager(MainGameManager p) => _map.GameManager = p;

	private Vector2 CalculateReturnPos() => GetWindow().GetVisibleRect().Size.Multiply(1, 0).Add(-250 / 2, 250 / 2) - _unfillScreenOffset;
}
