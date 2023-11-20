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
	[Export] public bool UseInventoryLabel { get; set; } = true;
	[Export] public bool ShowDescription { get; set; } = false;
	[Export] public int Count { get; set; } = 0;
	[Export] public Vector2 IconScaleModifier { get; set; } = new Vector2(1, 1);
	

    private void SetItem(DeliverableItems.EnumItem item)
    {
        _deliverableItem = item;
		var data = DeliverableItems.Items[(int)_deliverableItem];

		if (data.IsMilkBottle)
		{
            if (_bottle != null) _bottle.Show();
			if (_milk != null) if (_milk.Material as ShaderMaterial != null) (_milk.Material as ShaderMaterial).SetShaderParameter(SHADER_COLOR_PARAM, data.MilkColor);
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
	private Sprite2D _milk;
	private Sprite2D _item;

	private Label _label;

	public override void _Ready()
	{
		_bottle = GetNode<Sprite2D>("bottle");
        _milk = _bottle.GetNode<Sprite2D>("milk");
		_item = GetNode<Sprite2D>("item");

		_label = GetNode<Label>("label");
	}

	

    public override void _Process(double delta)
    {
		if (_label != null)
		{
			_label.Visible = UseInventoryLabel;
			if (UseInventoryLabel)
			{
				_label.Text = FormatInventoryLabel(ShowDescription);
			}
		}

		if (_bottle != null && _item != null)
		{
            _bottle.Scale = IconScaleModifier;
            _item.Scale = IconScaleModifier;
        }
    }

	private string FormatInventoryLabel(bool desc) => 
		$"{Count}x {DeliverableItems.GetByEnum(Item).Name} : {DeliverableItems.GetByEnum(Item).MilkUnitWeight * Count}mu" 
		+ (desc ? $"\n{DeliverableItems.GetByEnum(Item).Description}" : "");

	public string GetName() => DeliverableItems.Items[(int)_deliverableItem].Name;
	public string GetDescription() => DeliverableItems.Items[(int)_deliverableItem].Description;
	public int GetWeight() => DeliverableItems.Items[(int)_deliverableItem].MilkUnitWeight;


	public void UniqueifyMilkShader() => _milk.Material = _milk.Material.Duplicate() as ShaderMaterial;
	public void UniqueifyBottleTexture() => _bottle.Texture = _bottle.Texture.Duplicate() as Texture2D;
    public void UniqueifyItemTexture() => _item.Texture = _item.Texture.Duplicate() as Texture2D;
}
