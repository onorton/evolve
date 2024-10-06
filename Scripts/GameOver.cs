using Godot;
using System;

public class GameOver : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void OnGameOver(int generations)
    {
        var description = GetNode<RichTextLabel>("PanelContainer/VBoxContainer/Description");
        if (generations == 1)
        {
            description.Text = "A valiant effort! You managed to survive 1 generation.";
        }
        else
        {
            description.Text = $"A valiant effort! You managed to survive {generations} generations.";
        }
        Visible = true;
    }


    public void OnQuit()
    {
        GetTree().Quit();
    }

    public void OnTryAgain()
    {
        GetTree().ReloadCurrentScene();
        GetTree().Paused = false;
    }

}
