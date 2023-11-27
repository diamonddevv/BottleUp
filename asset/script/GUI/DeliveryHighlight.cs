using BottleUp.asset.script.Game;
using BottleUp.asset.script.Util;
using Godot;
using System;

public partial class DeliveryHighlight : Control
{
	public MainGameManager.DeliveryRequest? Request { get; set; }
	public Poi Poi { get; set; }
	public Minimap Minimap { get; set; }


	private Label _label;
	private TextureRect _icon;
	private bool _isMouseOver;


	public override void _Ready()
	{
		_label = GetNode<Label>("text");
		_icon = GetNode<TextureRect>("icon");

		_icon.Material = _icon.Material.Duplicate() as Material;

		_icon.MouseEntered += () => _isMouseOver = true;
		_icon.MouseExited += () => _isMouseOver = false;
	}

	
	public override void _Process(double delta)
	{
		if (Request != null)
		{
			_label.Text = ConstructLabel();


			if (_isMouseOver)
			{
				if (Poi != null && Minimap != null)
				{
					// highlight poi
					AnimatedSprite2D mapPoi = Minimap.GetDestPoiIcon(Poi);
					mapPoi.Play(Minimap.SELECTED);
					(mapPoi.Material as ShaderMaterial).SetShaderParameter(Minimap.SHADER_ENABLED, true);
					(_icon.Material as ShaderMaterial).SetShaderParameter(Minimap.SHADER_ENABLED, true);
				}
			} else
			{
                (_icon.Material as ShaderMaterial).SetShaderParameter(Minimap.SHADER_ENABLED, false);
            }
		}

    }

    private string ConstructLabel()
    {
		string s = "";
		bool first = true;

		foreach (var item in Request.Value.Items)
		{
			var i = DeliverableItems.GetByEnum(item.item);

			if (!first) s += "\n";
			s += $"{i.MilkUnitWeight * item.count}mu {i.Name}";
			first = false;
		}

		return s;
    }
}
