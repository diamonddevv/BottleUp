using BottleUp.asset.script.Game;
using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInventoryHandler : Node
{
	public const int MaxMilkUnitsCarriable = 20;

	private List<DeliverableItems.Item> _carriedItems;

	public override void _Ready()
	{
		_carriedItems = new List<DeliverableItems.Item>();
	}

	public void 
}
