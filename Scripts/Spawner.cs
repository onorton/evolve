using Godot;
using System;

public class Spawner : Node
{

    [Export]
    float _maxDistance = 2000.0f;

    [Export]
    float _minDistance = 100.0f;

    // From Player's current location

    private Player _player;

    [Export]
    float _spawnFrequency = 2.0f;

    [Export]
    float _initialNumber = 10.0f;

    private Random _random;


    [Export]
    private PackedScene _objectToSpawn;


    public override void _Ready()
    {
        _random = new Random();
        var timer = GetNode<Timer>("Timer");
        timer.WaitTime = _spawnFrequency;
        _player = GetNode<Player>("/root/Root/Player");
        for (var i = 0; i < _initialNumber; i++)
        {
            Spawn();
        }
    }


    private void Spawn()
    {

        var x = _random.NextDouble() * 2 * _maxDistance - _maxDistance + _player.Position.x;
        if (Math.Abs(x - _player.Position.x) < _minDistance)
        {
            if (x > _player.Position.x)
            {
                x += _minDistance;
            }
            else
            {
                x -= _minDistance;

            }
        }

        var y = _random.NextDouble() * 2 * _maxDistance - _maxDistance + _player.Position.y;
        if (Math.Abs(y - _player.Position.y) < _minDistance)
        {
            if (y > _player.Position.y)
            {
                y += _minDistance;
            }
            else
            {
                y -= _minDistance;

            }
        }

        var objectInstance = _objectToSpawn.Instance() as Node2D;
        objectInstance.Position = new Vector2((float)x, (float)y);
        AddChild(objectInstance);
    }
}
