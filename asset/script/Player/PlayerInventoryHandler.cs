using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static BottleUp.asset.script.Game.DeliverableItems;

public partial class PlayerInventoryHandler : Node
{
    [Signal]
    public delegate void InventoryUpdatedEventHandler();

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

			EmitSignal(SignalName.InventoryUpdated);

			return true;
		}
		else return false;
	}

	public bool RemoveItemOfType(EnumItem item)
	{
		var v = _carriedItems.Remove(item);
        EmitSignal(SignalName.InventoryUpdated);
        if (v)
		{
			_carriedWeight -= GetByEnum(item).MilkUnitWeight;
		}
		return v;
	}

	public Dictionary<EnumItem, int> GetCarriedItemsWithCount()
	{
		var dict = new Dictionary<EnumItem, int>();
		foreach (var item in _carriedItems)
		{
			if (dict.ContainsKey(item))
			{
				dict[item] += 1;
			} else
			{
				dict.Add(item, 1);
			}
		}
		return dict;
	}
}
