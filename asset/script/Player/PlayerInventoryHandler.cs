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

	private List<(float damage, EnumItem item)> _carriedItems;
	private int _carriedWeight;

	public override void _Ready()
	{
		_carriedItems = new List<(float damage, EnumItem item)>();
	}

	public bool CheckWeightAndAddItem(EnumItem item, int count = 1, float damage = 1)
	{
		int weight = GetByEnum(item).MilkUnitWeight;

		if (_carriedWeight + (weight * count) <= MaxMilkUnitsCarriable)
		{
			for (int i = 0; i < count; i++) _carriedItems.Add((damage, item));
			_carriedWeight += weight * count;

			$"Added: {item} x {count}; Weight now: {_carriedWeight}".Test();

			EmitSignal(SignalName.InventoryUpdated);

			return true;
		}
		else return false;
	}

	public bool RemoveItemOfType(EnumItem item, int count = 1)
	{
		bool removed = false;
		int realRemoved = 0;
		for (int i = 0; i < count; i++)
		{
			bool v = false;
			float dmg = 0;
			foreach (var e in _carriedItems)
			{
				if (e.item == item)
				{
					v = true;
					dmg = e.damage;
					break;
				}
			}
			if (v)
			{
				_carriedItems.Remove((dmg, item));
			}


			if (v)
			{
				realRemoved += 1;
				_carriedWeight -= GetByEnum(item).MilkUnitWeight;
            }

            EmitSignal(SignalName.InventoryUpdated);

            if (!removed && v) removed = true;
		}
        $"Removed: {item} x {realRemoved}; Weight now: {_carriedWeight}".Test();
        return removed;
	}

	public Dictionary<(float damage, EnumItem item), int> GetCarriedItemsWithCount()
	{
		var dict = new Dictionary<(float damage, EnumItem item), int>();
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

	public void Damage(float f)
	{
		List<(float damage, EnumItem item)> items = new List<(float damage, EnumItem item)>();
		foreach (var i in _carriedItems)
		{
			items.Add((i.damage - f, i.item));
		}
		_carriedItems = items;
	}

	public void Clear()
	{
		var clonedItems = new List<(float damage, EnumItem item)>();

		foreach (var item in _carriedItems) clonedItems.Add(item);

		foreach (var item in clonedItems)
		{
            RemoveItemOfType(item.item);
        }
	}

	public int GetWeight() => _carriedWeight;

	public float GetIntactness(EnumItem item)
	{
        foreach (var e in _carriedItems)
        {
            if (e.item == item)
            {
                return e.damage;
            }
        }
		return -1;
    }
}
