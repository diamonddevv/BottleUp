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


		_icon.MouseEntered += () => _isMouseOver = true;
		_icon.MouseExited += () => _isMouseOver = false;
	}

	
	public override void _Process(double delta)
	{
		if (Request != null)
		{
			if (_isMouseOver)
			{
				if (Poi != null && Minimap != null)
				{
					// highlight poi
					AnimatedSprite2D mapPoi = Minimap.GetDestPoiIcon(Poi);
					mapPoi.Play(Minimap.SELECTED);
					(mapPoi.Material as ShaderMaterial).SetShaderParameter(Minimap.SHADER_ENABLED, true);
				}
			}
		}
	}
}
