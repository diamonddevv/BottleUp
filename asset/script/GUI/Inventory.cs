using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Xml;

public partial class Inventory : Control
{
	// The header is written badly because i like the trope of video game characters being really bad at writing idk

	[Export] public int MaxDisplayableItems = 6;
	[Export] public string MoreItemsText = "(& more)";

	[ExportCategory("Nodes")]
	[Export] public NodePath ParentHud;
	[Export] public NodePath ContentsRoot;
	[Export] public NodePath TotalWeightLabel;
	[Export] public PackedScene InventoryItemScene;


	private HUD _hud;
	private Node2D _contentsRoot;
	private PlayerInventoryHandler _playerInv;
	private Label _totalWeightLabel;

	public override void _Ready()
	{
		_hud = GetNode<HUD>(ParentHud);
        _contentsRoot = GetNode<Node2D>(ContentsRoot);
        _totalWeightLabel = GetNode<Label>(TotalWeightLabel);
	}

	
	public override void _Process(double delta)
	{
        if (_playerInv == null && _hud.GetPlayer() != null)
		{
            _playerInv = _hud.GetPlayer().GetInventory();
			_playerInv.InventoryUpdated += Update;
        }
    }

	public void Update()
	{
		foreach (var child in _contentsRoot.GetChildren()) child.QueueFree();

		int index = 0;
		int count = _playerInv.GetCarriedItemsWithCount().Count;

		bool overflow = count > MaxDisplayableItems;

		foreach (var v in _playerInv.GetCarriedItemsWithCount())
		{
			if (overflow)
			{
                if (index >= MaxDisplayableItems - 1)
				{
                    Label label = new Label();
					_contentsRoot.AddChild(label);
					label.Position = Vector2.One.Multiply(-10f, index * 3.8f);
					label.Scale = BottleUpMath.Uniform(.1f);
					label.Text = MoreItemsText;
					if (_totalWeightLabel != null)
					{
						label.Theme = _totalWeightLabel.Theme;
						label.LabelSettings = _totalWeightLabel.LabelSettings;
					}

					break;
                }
			}

			var item = InventoryItemScene.Instantiate<DeliverableItem>();
			_contentsRoot.AddChild(item);

			item.UniqueifyBottleTexture();
			item.UniqueifyMilkShader();
			item.UniqueifyItemTexture();

			item.Position = item.Position + Vector2.One.Multiply(-10f, index * 3.8f);
			item.Scale = BottleUpMath.Uniform(.45f);

			item.IconScaleModifier = BottleUpMath.Uniform(.65f);

			item.Item = v.Key.item;
			item.Count = v.Value;

			index++;
		}

        if (_totalWeightLabel != null)
        {
            _totalWeightLabel.Text = FormatTotalWeightLabel(_playerInv.GetWeight());
        }
    }

	public static string FormatTotalWeightLabel(int mu) => $"Total Weight:\n{mu}mu";
}
