using Godot;
using System;

public class StatusBar : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private float _maxSize = 200.0f;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        RectSize = new Vector2(_maxSize, 40.0f);
    }

    public void OnStatusUpdate(float value, float maxValue)
    {
        RectSize = new Vector2((value / maxValue) * _maxSize, 40.0f);
    }

}
