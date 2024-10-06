using Godot;
using System;

public class Consumable : RigidBody2D
{

    [Export]
    public float Energy = 10.0f;

    [Export]
    public float Dna = 10.0f;

    // // Called every frame. 'delta' is the elapsed time since the previous frame.
    // public override void _PhysicsProcess(float delta)
    // {

    //     LinearVelocity = new Vector2(1.0f, 1.20f) * 10;

    // }
}
