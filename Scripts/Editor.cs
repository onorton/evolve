using Godot;
using System;

public class Editor : Control
{

    private RichTextLabel _message;

    public override void _Ready()
    {
        _message = GetNode<RichTextLabel>("PanelContainer/VBoxContainer/Message");
    }

    public void OnContinue()
    {
        GetTree().Paused = false;
        Visible = false;
    }

    public void OnEvolved()
    {
        Visible = true;
    }

    public override void _Process(float delta)
    {
        var children = GetNode("PanelContainer/VBoxContainer/Upgrades").GetChildren();
        if (children.Count == 0)
        {
            _message.Text = "You are able to evolve!\n\nYou have all available mutations.";
        }
    }

    public void OnDnaChanged(float value)
    {
        var children = GetNode("PanelContainer/VBoxContainer/Upgrades").GetChildren();

        foreach (var child in children)
        {
            if (child is Upgrade upgrade)
            {
                upgrade.OnDnaChanged(value);
            }
        }
    }
}
