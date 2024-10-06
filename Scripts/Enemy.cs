using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class Enemy : KinematicBody2D
{
    private PackedScene _food = GD.Load<PackedScene>("res://Scenes/Food.tscn");
    private PackedScene _dna = GD.Load<PackedScene>("res://Scenes/DNA.tscn");

    private Player _player;
    private float _distanceToChasePlayer = 500.0f;

    [Export]
    private float _speed = 100.0f;
    private Vector2 _direction;
    private Vector2 _intendedDirection;

    private Timer _switchDirectionTimer;

    private Random _random;

    private bool _touchingPlayer;

    [Export]
    private float _health = 20.0f;

    [Export]
    public bool Herbivore = false;

    // Rads/s
    private float _turnSpeed = 1.5f;

    [Export]
    private float _dnaAmount = 20.0f;

    private Vector2 _externalForces = new Vector2(0.0f, 0.0f);


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _player = GetNode<Player>("/root/Root/Player");
        _switchDirectionTimer = GetNode<Timer>("Timer");
        _random = new Random();
        _direction = Vector2.Up;
    }

    public override void _Process(float delta)
    {

        if (_player.Position.DistanceTo(Position) < _distanceToChasePlayer && !Herbivore)
        {
            // Turn towards 
            _intendedDirection = Position.DirectionTo(_player.Position);
        }
        else
        {
            if (_switchDirectionTimer.TimeLeft == 0.0f)
            {
                _intendedDirection = new Vector2(((float)_random.NextDouble()) * 2.0f - 1.0f, ((float)_random.NextDouble()) * 2.0f - 1.0f).Normalized();
                _switchDirectionTimer.Start();
            }
        }

        var intendedAngle = Vector2.Up.AngleTo(_intendedDirection);
        var currentAngle = Vector2.Up.AngleTo(_direction);

        var maxAngleInFrame = _turnSpeed * delta;

        // Depending on the direction, angle can be more than Pi radians. Adjust accordingly so the shortest distance is taken
        if (Mathf.Abs(intendedAngle - currentAngle) > Math.PI)
        {
            if (currentAngle >= 0.0f)
            {
                currentAngle -= 2 * (float)Math.PI;
            }
            else
            {
                currentAngle += 2 * (float)Math.PI;
            }
        }

        var newAngle = 0.0f;

        if (Mathf.Abs(intendedAngle - currentAngle) < maxAngleInFrame)
        {
            newAngle = intendedAngle;
        }
        else
        {
            if (intendedAngle < currentAngle)
            {
                newAngle = currentAngle - maxAngleInFrame;
            }
            else
            {
                newAngle = currentAngle + maxAngleInFrame;
            }
        }

        _direction = _direction.Rotated(newAngle - currentAngle);
        Rotation = newAngle;


        if (_health <= 0.0f)
        {
            // Death


            // Spawn food and DNA
            QueueFree();
            var consumableInstance = _food.Instance() as Consumable;
            consumableInstance.Position = Position + new Vector2((float)(_random.NextDouble() * 2.0f - 1.0f) * 20.0f, (float)(_random.NextDouble() * 2.0f - 1.0f) * 20.0f);
            GetTree().Root.GetChild(0).AddChild(consumableInstance);
            consumableInstance = _food.Instance() as Consumable;
            consumableInstance.Position = Position + new Vector2((float)(_random.NextDouble() * 2.0f - 1.0f) * 20.0f, (float)(_random.NextDouble() * 2.0f - 1.0f) * 20.0f);
            GetTree().Root.GetChild(0).AddChild(consumableInstance);
            consumableInstance = _dna.Instance() as Consumable;
            consumableInstance.Dna = _dnaAmount;
            consumableInstance.Position = Position + new Vector2((float)(_random.NextDouble() * 2.0f - 1.0f) * 20.0f, (float)(_random.NextDouble() * 2.0f - 1.0f) * 20.0f);
            GetTree().Root.GetChild(0).AddChild(consumableInstance);


        }

        if (_touchingPlayer)
        {
            _player.TakeDamage(20.0f);
        }
    }

    public override void _PhysicsProcess(float delta)
    {

        var collision = MoveAndCollide(_direction * delta * _speed, infiniteInertia: false);

        if (_externalForces != Vector2.Zero)
        {
            collision = MoveAndCollide(_externalForces * 5 * delta * _speed, infiniteInertia: false);
        }

        if (collision != null)
        {
            if (collision.Collider is Player && !Herbivore)
            {
                _touchingPlayer = true;
            }
            else
            {
                _touchingPlayer = false;
            }

            if (collision.Collider is RigidBody2D body)
            {
                if (body.IsInGroup("Food") && Herbivore)
                {
                    body.QueueFree();
                }
            }

        }
        else
        {
            _touchingPlayer = false;
        }
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        GD.Print("Enemy took damage");
        GetNode<AnimationPlayer>("Sprite/AnimationPlayer").Play("flash_anim");

    }

    public void ApplyVelocity(Vector2 velocity, float time)
    {
        _externalForces = velocity;
        GetTree().CreateTimer(time).Connect("timeout", this, "ForceStoppedApplying");

    }

    public void ForceStoppedApplying()
    {
        _externalForces = Vector2.Zero;
    }


}


