using Godot;
using System;

public class FadeIn : ColorRect
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Visible = true;
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "color", new Color(0.0f, 0.0f, 0.0f, 0.0f), 4.0f);

    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //      
    //  }
}
