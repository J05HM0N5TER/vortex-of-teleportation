using System;
using Godot;

public class Player : KinematicBody2D
{
    [Export] private float MaxSpeed;
    [Export] private float TimeToMaxSpeed;  // Time to go from 0 - max speed.
    [Export] private float TimeToTurn;      // Time to change directions from max speed.
    [Export] private float TimeToStop;      // Time to go from max speed - 0.

    [Export] private float JumpHeight;      //config     
    [Export] private float JumpDuration;    //config
    [Export] private float FallDuration;

    private float _horizontalInput = 0f;
    private bool _jumpBuffer;
    private ForceBasedCharacterController _cc;

    private AnimationTree _animTree;
    private AnimatedSprite _sprite;

    public override void _Ready()
    {
        _cc = new ForceBasedCharacterController(this, MaxSpeed, TimeToMaxSpeed, TimeToTurn, TimeToStop, JumpHeight, JumpDuration, FallDuration);
        _animTree = GetNode<AnimationTree>("AnimationTree");
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public override void _Process(float delta)
    {
        _horizontalInput = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

        if (Input.IsActionJustPressed("jump"))
            _jumpBuffer = true;

        _animTree.Set("parameters/movement/current", _cc.GetHorizontalVelocity != 0f);

        if (_cc.GetVelocity.x != 0f)
            _sprite.FlipH = _cc.GetVelocity.x < 0f;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        _cc.Move((int)_horizontalInput);

        if (_jumpBuffer)
        {
            _cc.Jump();
            _jumpBuffer = false;
        }

        _cc.ProcessPhysics(delta);
    }
}