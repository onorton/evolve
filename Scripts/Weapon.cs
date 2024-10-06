using Godot;
using System;
using System.Diagnostics;

public class Weapon : Node2D
{

    private float _damage = 5.0f;

    private void OnBodyEntered(Node node)
    {
        if (node is Enemy enemy)
        {
            GD.Print("Hit enemy");
            enemy.TakeDamage(_damage);
            GD.Print((GetNode<CollisionPolygon2D>("Spike").GlobalPosition - GlobalPosition).Normalized());
            enemy.ApplyVelocity((GetNode<CollisionPolygon2D>("Spike").GlobalPosition - GlobalPosition).Normalized(), 0.1f);
        }

        GetNode<SoundManager>("/root/Root/Camera2D/SoundManager").OnPlaySoundEffect("slash");

    }




}
