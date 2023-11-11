using BottleUp.asset.script.Game;
using Godot;
using System;

[Tool]
public partial class DeliverableItem : Node2D
{
	private const string SHADER_COLOR_PARAM = "color";

	private DeliverableItems.EnumItem _deliverableItem;

	[Export] public DeliverableItems.EnumItem Item
	{
		get => _deliverableItem;
		set => SetItem(value);
	}


    private void SetItem(DeliverableItems.EnumItem item)
    {
        _deliverableItem = item;
		var data = DeliverableItems.Items[(int)_deliverableItem];

		if (data.IsMilkBottle)
		{
            if (_bottle != null) _bottle.Show();
            if (_milkTintShader != null) _milkTintShader.SetShaderParameter(SHADER_COLOR_PARAM, data.MilkColor);
        }
        else
		{
			if (_bottle != null) _bottle.Hide();
        }

		int x = 1 + Mathf.CeilToInt(data.TextureIndex / 2);
		int y = data.TextureIndex % 2;
		if (_item != null) (_item.Texture as AtlasTexture).Region = new Rect2(x*16,y*16,16,16);
    }


    private Sprite2D _bottle;
	private ShaderMaterial _milkTintShader;
	private Sprite2D _item;
	public override void _Ready()
	{
		_bottle = GetNode<Sprite2D>("bottle");
        _milkTintShader = _bottle.GetNode<Sprite2D>("milk").Material as ShaderMaterial;
		_item = GetNode<Sprite2D>("item");
	}

    public override void _Process(double delta)
    {
    }


	public string GetName() => DeliverableItems.Items[(int)_deliverableItem].Name;
	public string GetDescription() => DeliverableItems.Items[(int)_deliverableItem].Description;
	public int GetWeight() => DeliverableItems.Items[(int)_deliverableItem].MilkUnitWeight;
}
