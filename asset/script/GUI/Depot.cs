using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.Json;

public partial class Depot : CanvasLayer
{
	public HUD Hud { get; set; }

	[Export] public int ItemsPerHBox;

	private Minimap _map;
	private ColorRect _mapFullOverlay;
	private Button _leaveButton;
	private Button _dumpButton;
	private Label _scaleWeightLabel;
	private Label _reqs;
	private VBoxContainer _items;
	private VBoxContainer _deliveries;

	private List<DeliverableItems.EnumItem> _itemsToAdd;
	private int _weightAdded;
	private bool _setMapPlayer;
	private Vector2 _oldMapPos;
	private Vector2 _unfillScreenOffset;
	private bool _isMapFull;

	public override void _Ready()
	{
		_map = GetNode<Minimap>("minimap");
		_mapFullOverlay = GetNode<ColorRect>("mapFullOverlay");
		_leaveButton = GetNode<Button>("leaveButton");
		_dumpButton = GetNode<Button>("dumpButton");
		_items = GetNode<VBoxContainer>("items");
		_deliveries = GetNode<VBoxContainer>("deliveries");
        _scaleWeightLabel = GetNode<Label>("scale/weight/numeral");
        _reqs = GetNode<Label>("reqs");

        _leaveButton.Pressed += () =>
		{
			
			foreach (var item in _itemsToAdd)
			{
                Hud.GetPlayer().GetInventory().CheckWeightAndAddItem(item);
            }

			_itemsToAdd.Clear();

			Hud.SetInDepot(false);
		};

		_dumpButton.Pressed += () =>
		{
			Hud.GetPlayer().GetInventory().Clear();
		};

		_map.MapFullClicked += (state) =>
		{
			_isMapFull = state;

			if (state)
			{
				_map.Position = (GetWindow().GetVisibleRect().Size / 2).Multiply(1, 1.5f);

				_mapFullOverlay.Visible = true;
				_deliveries.Visible = true;
				_reqs.Visible = true;

				Hud.GameManager.ActiveRequestsUpdated += () =>
				{
					if (_isMapFull)
					{
						BuildDeliveries(Hud.GameManager);
					}
				};

				BuildDeliveries(Hud.GameManager, false);

				foreach (var hbox in _items.GetChildren())
				{
					foreach (var c in hbox.GetChildren())
					{
                        if (c is DepotItemButton dib)
                        {
                            dib.SetDisabled(true);
                        }
                    }
				}

            } else
			{
				_map.Position = CalculateReturnPos();

                _mapFullOverlay.Visible = false;
                _deliveries.Visible = false;
                _reqs.Visible = false;

                foreach (var hbox in _items.GetChildren())
                {
                    foreach (var c in hbox.GetChildren())
                    {
                        if (c is DepotItemButton dib)
                        {
                            dib.SetDisabled(false);
                        }
                    }
                }
            }
        };

		_unfillScreenOffset = new Vector2(20, -20);


		GetWindow().SizeChanged += () => { if (!_isMapFull) _map.Position = CalculateReturnPos(); };

		int hboxCount = 0;
		HBoxContainer hbox = new HBoxContainer();
		_items.AddChild(hbox);
		foreach (var itemType in DeliverableItems.Items)
		{
			var enumItem = itemType.Enum;
			DepotItemButton button = ResourceLoader.Load<PackedScene>("res://asset/ui/depot_item_button.tscn").Instantiate<DepotItemButton>();

            hbox.AddChild(button);

            button.Item.UniqueifyBottleTexture();
            button.Item.UniqueifyItemTexture();
            button.Item.UniqueifyMilkShader();

            button.Item.Item = enumItem;
            button.Item.UseInventoryLabel = true;
			button.Item.ShowDescription = true;


            button.AddButton.Pressed += () =>
			{
				if (_weightAdded + itemType.MilkUnitWeight <= 20)
				{
					button.Item.Count += 1;
					_weightAdded += itemType.MilkUnitWeight;
					_itemsToAdd.Add(enumItem);
				}
			};
            button.SubtractButton.Pressed += () =>
            {
				if (button.Item.Count > 0)
				{
                    button.Item.Count -= 1;
                    _weightAdded -= itemType.MilkUnitWeight;
                    _itemsToAdd.Remove(enumItem);
                }
            };

            button.Visible = true;

			hboxCount += 1;
			if (hboxCount >= ItemsPerHBox)
			{
                hbox = new HBoxContainer();
                _items.AddChild(hbox);
				hboxCount = 0;
			}

		}


    }

	private void BuildDeliveries(MainGameManager manager, bool update = true)
    {
		if (!update) foreach (var child in _deliveries.GetChildren()) child.QueueFree();

		foreach (var delivery in manager.GetActiveRequests())
		{
			if (update)
			{
				bool skip = false;
				foreach (var child in _deliveries.GetChildren())
				{
					if (child is DeliveryHighlight h)
					{
						if (h.Request.Value.Equals(delivery.Value))
						{
							skip = true;
							break;
						}
					}
				}
				if (skip) continue;
			}

			DeliveryHighlight highlight = ResourceLoader.Load<PackedScene>("res://asset/ui/delivery_highlight.tscn").Instantiate<DeliveryHighlight>();


			_deliveries.AddChild(highlight);
			highlight.Show();

			highlight.Minimap = _map;
			highlight.Poi = delivery.Key;
			highlight.Request = delivery.Value;
		}
    }

    public void _Opened()
	{
		_map.SetFull(false);
		_map.Position = CalculateReturnPos();

		_itemsToAdd = new List<DeliverableItems.EnumItem>();
		_weightAdded = 0;

        foreach (var hbox in _items.GetChildren())
        {
            foreach (var c in hbox.GetChildren())
            {
                if (c is DepotItemButton dib)
                {
					dib.Item.Count = 0;
                }
            }
        }
    }

	public override void _Process(double delta)
	{
		if (_scaleWeightLabel != null) _scaleWeightLabel.Text = $"+{_weightAdded}";
		if (_reqs != null) _reqs.Text = Hud.GetRequestsLabel().Text;
    }

	public void SetMapPlayer(Player p) => _map.SetPlayer(p);
	public void SetMapMapScene(PackedScene p) => _map.SetMapScene(p);
	public void SetMapGameManager(MainGameManager p) => _map.GameManager = p;

	private Vector2 CalculateReturnPos() => GetWindow().GetVisibleRect().Size.Multiply(1, 0).Add(-250 / 2, 250 / 2) - _unfillScreenOffset;
}
