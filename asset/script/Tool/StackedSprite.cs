using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

[Tool]
public partial class StackedSprite : Sprite2D
{
    private bool _showSprites = true;
    [Export] public bool ShowSprites
    {
        get => _showSprites;
        set => SetShowSprites(value);
    }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DrawSprites();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    private void SetShowSprites(bool show)
    {
        _showSprites = show;
        if (show) DrawSprites();
        else ClearSprites();
    }


    private void DrawSprites()
    {
        ClearSprites();
        for (int i = 0; i < Hframes; i++)
        {
            var next = new Sprite2D();
            next.Texture = Texture;
            next.Hframes = Hframes;
            next.Frame = i;
            next.Position = new Vector2(0, -i);

            AddChild(next);
        }

        RegionEnabled = true;
        RegionRect = new Rect2(0, 0, 0, 0); // hide the base sprite
    }

    private void ClearSprites()
    {
        foreach (var c in GetChildren()) c.QueueFree();
    }

    public void SetRotation(float degs)
    {
        var v = (GetParent() as Node2D).Rotation;
        Rotation = -v;
        foreach (Sprite2D c in GetChildren()) c.RotationDegrees = degs;
    }
}
