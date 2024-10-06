using Godot;
using System;

public class Victory : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void OnVictory()
    {
        Visible = true;
    }


    public void OnQuit()
    {
        GetTree().Quit();
    }

    public void OnContinue()
    {
        Visible = false;
        GetTree().Paused = false;
    }

}
