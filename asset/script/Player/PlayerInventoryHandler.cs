using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static BottleUp.asset.script.Game.DeliverableItems;

public partial class PlayerInventoryHandler : Node
{
	public const int MaxMilkUnitsCarriable = 20;

	private List<EnumItem> _carriedItems;
	private int _carriedWeight;

	public override void _Ready()
	{
		_carriedItems = new List<EnumItem>();
	}

	public bool CheckWeightAndAddItem(EnumItem item)
	{
		int weight = GetByEnum(item).MilkUnitWeight;

		if (_carriedWeight + weight <= MaxMilkUnitsCarriable)
		{
			_carriedItems.Add(item);
			_carriedWeight += weight;

			$"Added: {item}; Weight now: {_carriedWeight}".Test();

			return true;
		}
		else return false;
	}

	public bool RemoveItemOfType(EnumItem item)
	{
		var v = _carriedItems.Remove(item);
		if (v)
		{
			_carriedWeight -= GetByEnum(item).MilkUnitWeight;
		}
		return v;
	}
}
