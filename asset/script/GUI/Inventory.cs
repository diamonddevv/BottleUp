using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Xml;

public partial class Inventory : Control
{
	// The header is written badly because i like the trope of video game characters being really bad at writing idk

	[ExportCategory("Nodes")]
	[Export] public NodePath ParentHud;
	[Export] public NodePath ContentsRoot;
	[Export] public PackedScene InventoryItemScene;


	private HUD _hud;
	private Node2D _contentsRoot;
	private PlayerInventoryHandler _playerInv;

	public override void _Ready()
	{
		_hud = GetNode<HUD>(ParentHud);
        _contentsRoot = GetNode<Node2D>(ContentsRoot);
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
		foreach (var v in _playerInv.GetCarriedItemsWithCount())
		{
			var item = InventoryItemScene.Instantiate<DeliverableItem>();
			_contentsRoot.AddChild(item);
			item.Position = item.Position + Vector2.One.Multiply(-8f, index * 4f);
			item.Scale = BottleUpMath.Uniform(.38f);

			item.Item = v.Key;
			item.Count = v.Value;

			index++;
		}
	}
}
