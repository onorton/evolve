using Godot;
using System;

public class Upgrade : PanelContainer
{
    [Export]
    private int _price;

    [Export]
    private string _name;
    [Export]
    private string _description;

    public override void _Ready()
    {
        GetNode<RichTextLabel>("VBoxContainer/Title").Text = _name;
        GetNode<RichTextLabel>("VBoxContainer/Description").Text = _description;
        GetNode<Label>("VBoxContainer/HBoxContainer/Price").Text = _price.ToString();

    }

    public void OnPurchase()
    {

        var player = GetNode<Player>("/root/Root/Player");
        player.AddUpgrade(_name, _price);
        QueueFree();
    }

    public void OnDnaChanged(float value)
    {
        GetNode<Button>("VBoxContainer/HBoxContainer/Buy").Disabled = value < _price;
    }


}
