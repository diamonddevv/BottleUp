using Godot;
using System;

public partial class DepotItemButton : Control
{
	[Export] public DeliverableItem Item { get; set; }
	[Export] public TextureButton AddButton { get; set; }
	[Export] public TextureButton SubtractButton { get; set; }


	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}
