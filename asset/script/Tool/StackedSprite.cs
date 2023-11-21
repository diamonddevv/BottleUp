using BottleUp.asset.script.Util;
using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

[Tool]
public partial class StackedSprite : Sprite2D
{
    private bool _showSprites = true;
    private float _separate = 1;
    private float _displayRotation = 0;
    [Export] public bool ShowSprites
    {
        get => _showSprites;
        set => SetShowSprites(value);
    }

    [Export(PropertyHint.Range, "0,10,0.1")] public float Separate
    {
        get => _separate;
        set {
            _separate = value;
            DrawSprites();
        }
    }

    [Export(PropertyHint.Range, "0,360,0.1")] public float DisplayRotation
    {
        get => _displayRotation;
        set => SetDispRotation(value);
    }

    public override void _Ready()
	{
		DrawSprites();
	}

    
    public override void _Process(double delta)
	{
	}

    private void SetShowSprites(bool show)
    {
        _showSprites = show;
        if (show) DrawSprites();
        else ClearSprites();
    }

    private void SetDispRotation(float rot)
    {
        _displayRotation = rot;
        if (Engine.IsEditorHint())
        {
            SetRotation(rot);
        }
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
            next.Position = new Vector2(0, -i * Separate);

            AddChild(next);
        }

        RegionEnabled = true;
        RegionRect = new Rect2(0, 0, 0, 0); // hide the base sprite

        SetRotation(DisplayRotation);
    }

    private void ClearSprites()
    {
        foreach (var c in GetChildren()) c.QueueFree();
    }

    public void SetRotation(float degs)
    {
        if (GetParent() is Node2D n)
        {
            var v = n.Rotation;
            Rotation = -v;
        }

        foreach (Sprite2D c in GetChildren()) c.RotationDegrees = degs;
    }
}
