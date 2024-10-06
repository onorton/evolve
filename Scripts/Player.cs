using Godot;
using System;

public class Player : KinematicBody2D
{

    [Signal]
    public delegate void HealthUpdateEventHandler(float health, float maxHealth);
    [Signal]
    public delegate void EnergyUpdateEventHandler(float energy, float maxEnergy);
    [Signal]
    public delegate void DnaUpdateEventHandler(float dna);
    [Signal]
    public delegate void EvolvedEventHandler();
    [Signal]
    public delegate void VictoryEventHandler();
    [Signal]
    public delegate void GameOverEventHandler(int generations);
    private PackedScene _foodScene = GD.Load<PackedScene>("res://Scenes/Food.tscn");
    private PackedScene _dnaScene = GD.Load<PackedScene>("res://Scenes/DNA.tscn");


    private float _health;
    private float _maxHealth = 100;
    private float _energy = 100;
    private float _maxEnergy = 100;
    private float _speed = 100;

    private float _acquiredDnaCurrentGeneration = 0.0f;
    private float _dnaForAGeneration = 100.0f;
    private float _dna = 0.0f;

    private Camera2D _camera;


    // Rads/s
    private float _turnSpeed = 3f;

    private Timer _immunityTimer;
    private bool _immune;

    private Vector2 _direction = Vector2.Up;
    private Vector2 _intendedDirection = Vector2.Zero;
    private bool _dead = false;


    private bool _moving = false;

    private float _baseEnergyConsumptionRate = 2.0f;

    private float _baseEnergyProductionRate = 0.0f;
    private float _movementEnergyConsumptionRate = 5.0f;

    private int _generations = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _immunityTimer = GetNode<Timer>("Immunity");
        _immune = false;
        _health = _maxHealth;
        _camera = GetNode<Camera2D>("/root/Root/Camera2D");
    }

    public override async void _Process(float delta)
    {
        if (_dead)
        {
            return;
        }

        // GD.Print($"Health: {_health}, Energy: {_energy}");

        if (_acquiredDnaCurrentGeneration >= _dnaForAGeneration)
        {
            GD.Print("Evolving...");
            _acquiredDnaCurrentGeneration = 0.0f;
            _generations += 1;
            if (_generations == 5)
            {
                EmitSignal(nameof(VictoryEventHandler));
            }
            else
            {
                EmitSignal(nameof(EvolvedEventHandler));
            }
            _health = _maxHealth;
            _energy = _maxEnergy;
            GetTree().Paused = true;
        }

        _baseEnergyProductionRate = 1.0f;


        var intendedAngle = Vector2.Up.AngleTo(_intendedDirection);
        var currentAngle = Vector2.Up.AngleTo(_direction);

        var maxAngleInFrame = _turnSpeed * delta;

        var newAngle = 0.0f;


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

        var energyConsumed = delta * (_baseEnergyConsumptionRate - _baseEnergyProductionRate);

        if (_moving)
        {
            energyConsumed += delta * _movementEnergyConsumptionRate;
        }


        if (_energy <= 0.0f)
        {
            _health -= energyConsumed;
            _health = Mathf.Max(_health, 0.0f);
        }
        else
        {
            _energy -= energyConsumed;
            _energy = Mathf.Clamp(_energy, 0.0f, _maxEnergy);
        }

        // Recover lost health if energy is left
        if (_health < _maxHealth && _energy > 0.0f)
        {
            _health = Mathf.Min(_health + delta * 5.0f, _maxHealth);
        }


        if (_health <= 0.0f)
        {
            // Spawn food and DNA
            var consumableInstance = _foodScene.Instance() as Node2D;
            consumableInstance.Position = Position;
            GetTree().Root.GetChild(0).AddChild(consumableInstance);
            consumableInstance = _dnaScene.Instance() as Node2D;
            consumableInstance.Position = Position;
            GetTree().Root.GetChild(0).AddChild(consumableInstance);

            Visible = false;
            GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
            _dead = true;
            GD.Print("GAME OVER!");
            GetNode<SoundManager>("/root/Root/Camera2D/SoundManager").OnPlaySoundEffect("game_over");
            await ToSignal(GetTree().CreateTimer(1), "timeout");
            GetTree().Paused = true;
            EmitSignal(nameof(GameOverEventHandler), _generations);
        }

        EmitSignal(nameof(HealthUpdateEventHandler), _health, _maxHealth);
        EmitSignal(nameof(EnergyUpdateEventHandler), _energy, _maxEnergy);

    }


    public override void _Input(InputEvent @event)
    {
        if (_dead)
        {
            return;
        }

        if (@event is InputEventMouse mouseEvent)
        {
            _intendedDirection = (mouseEvent.Position + _camera.Position - Position).Normalized();
        }

        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Scancode == (int)KeyList.W)
            {
                if (keyEvent.IsPressed())
                {
                    _moving = true;
                }
                else if (keyEvent.IsReleased())
                {
                    _moving = false;
                }
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_dead)
        {
            return;
        }

        if (_moving)
        {

            var collision = MoveAndCollide(_direction * delta * _speed, infiniteInertia: false);


            if (collision != null)
            {
                var body = collision.Collider as PhysicsBody2D;
                if (body.IsInGroup("Consumables"))
                {
                    GetNode<SoundManager>("/root/Root/Camera2D/SoundManager").OnPlaySoundEffect("eat");
                    var energy = (body as Consumable).Energy;
                    _energy += energy;
                    _acquiredDnaCurrentGeneration += (body as Consumable).Dna;
                    _dna += (body as Consumable).Dna;
                    EmitSignal(nameof(DnaUpdateEventHandler), _dna);
                    EmitSignal(nameof(EnergyUpdateEventHandler), _energy, _maxEnergy);
                    body.QueueFree();
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!_immune)
        {
            GetNode<AnimationPlayer>("Sprite/AnimationPlayer").Play("flash_anim");

            GetNode<SoundManager>("/root/Root/Camera2D/SoundManager").OnPlaySoundEffect("damage");
            _health -= damage;
            EmitSignal(nameof(HealthUpdateEventHandler), _health, _maxHealth);
            _immunityTimer.Start();
            _immune = true;
        }
    }

    public void OnImmunityTimeout()
    {
        _immune = false;
    }

    public void AddUpgrade(string name, float cost)
    {
        _dna -= cost;
        EmitSignal(nameof(DnaUpdateEventHandler), _dna);

        switch (name)
        {
            case "Flagellum":
                {
                    GetNode<AnimatedSprite>("Flagellum").Visible = true;
                    _speed *= 1.5f;
                    _turnSpeed *= 1.5f;
                    _movementEnergyConsumptionRate *= 1.5f;
                    _maxHealth += 20;
                    break;
                }
            case "Spike":
                {
                    GD.Print(GetNode("Spike").GetType());
                    GetNode<Weapon>("Spike").Visible = false;
                    GetNode<CollisionPolygon2D>("Spike/Spike").Disabled = true;
                    GetNode<Weapon>("SecondSpike").Visible = true;
                    GetNode<CollisionPolygon2D>("SecondSpike/Spike").Disabled = false;
                    GetNode<Weapon>("ThirdSpike").Visible = true;
                    GetNode<CollisionPolygon2D>("ThirdSpike/Spike").Disabled = false;

                    _maxHealth += 20;

                    _baseEnergyConsumptionRate += 0.5f;
                    break;
                }
            case "Chloroplasts":
                {
                    GetNode<Sprite>("Sprite").Texture = GD.Load<Texture>("res://Sprites/player_green.png");
                    _baseEnergyProductionRate += 1.0f;
                    break;
                }
            default:
                throw new Exception($"{name} is not a valid upgrade");
        }

    }
}
