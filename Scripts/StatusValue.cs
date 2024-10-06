using Godot;
using System;

public class StatusValue : Label
{
    public void OnStatusUpdate(float value)
    {
        Text = $"{(int)value}";
    }

}
