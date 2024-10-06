using Godot;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : Node
{
    private AudioStreamPlayer2D _mainMusic;
    private AudioStreamPlayer2D _combatMusic;
    private AudioStreamPlayer2D _soundEffectPlayer;

    private Player _player;

    private bool _enemyInRangeOfPlayer = false;

    public override void _Ready()
    {
        _mainMusic = GetNode<AudioStreamPlayer2D>("MainMusic");
        _combatMusic = GetNode<AudioStreamPlayer2D>("CombatMusic");
        _soundEffectPlayer = GetNode<AudioStreamPlayer2D>("SoundEffectPlayer");

        _combatMusic.VolumeDb = -80.0f;
        _player = GetNode<Player>("/root/Root/Player");
    }

    public override void _Process(float delta)
    {
        var enemies = new List<Enemy>();

        foreach (var enemy in GetTree().GetNodesInGroup("Enemies"))
        {

            if (enemy is Enemy e)
            {
                enemies.Add(e);
            }
        }

        if (enemies.Any(enemy => enemy.Position.DistanceTo(_player.Position) < 500.0f && !enemy.Herbivore) && !_enemyInRangeOfPlayer)
        {
            _enemyInRangeOfPlayer = true;
            OnCombatStart();

        }
        if (!enemies.Any(enemy => enemy.Position.DistanceTo(_player.Position) < 500.0f && !enemy.Herbivore) && _enemyInRangeOfPlayer)
        {
            _enemyInRangeOfPlayer = false;
            OnCombatEnd();
        }
    }




    public void OnCombatStart()
    {
        var tween = GetTree().CreateTween();
        _combatMusic.Play();
        tween.SetParallel();
        tween.TweenProperty(_mainMusic, "volume_db", -80.0f, 5.0f);
        tween.TweenProperty(_combatMusic, "volume_db", 0.0f, 5.0f);
        tween.Play();

    }

    public void OnCombatEnd()
    {
        var tween = GetTree().CreateTween();
        tween.SetParallel();
        tween.TweenProperty(_mainMusic, "volume_db", 0.0f, 2.0f);
        tween.TweenProperty(_combatMusic, "volume_db", -80.0f, 2.0f);

    }

    public void OnPlaySoundEffect(string soundName)
    {
        _soundEffectPlayer.Stream = GD.Load<AudioStream>($"res://Audio/{soundName}.wav");
        _soundEffectPlayer.Play();
    }

    public void OnMuteToggle(bool mute)
    {
        AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), mute);
    }



}
