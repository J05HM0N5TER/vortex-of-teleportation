using System;
using Godot;

public class Player : KinematicBody2D
{
    #region Locomotion
    [Export] private float MaxSpeed;
    [Export] private float TimeToMaxSpeed;  // Time to go from 0 - max speed.
    [Export] private float TimeToTurn;      // Time to change directions from max speed.
    [Export] private float TimeToStop;      // Time to go from max speed - 0.

    [Export] private float JumpHeight;      //config     
    [Export] private float JumpDuration;    //config
    [Export] private float FallDuration;

    [Export] private float JumpCoyoteTime;
    [Export] private float LedgeCoyoteTime;

    private float _horizontalInput = 0f;
    private bool _jumpBuffer;
    private ForceBasedCharacterController _cc;
    #endregion

    #region Throwing
    [Export] private NodePath throwablePath;
    [Export] private float ThrowOffset;
    [Export] private float ThrowSpeed;
    [Export] private float ThrowGravity;

    private Vector2 _aim;
    private ThrowingBehaviour _throwingBehaviour;
    private Throwable _throwable;
    #endregion;

    private AnimationTree _animTree;
    private AnimatedSprite _sprite;

    public override void _Ready()
    {
        _cc = new ForceBasedCharacterController(this, MaxSpeed, TimeToMaxSpeed, TimeToTurn, TimeToStop, JumpHeight, JumpDuration, FallDuration, JumpCoyoteTime, LedgeCoyoteTime);
        _animTree = GetNode<AnimationTree>("AnimationTree");
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");

        _throwingBehaviour = new ThrowingBehaviour(ThrowOffset, ThrowSpeed, ThrowGravity);
        _throwable = GetNode<Throwable>(throwablePath);
    }

    public override void _Process(float delta)
    {
        _horizontalInput = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        _aim = (GetGlobalMousePosition() - Position).Normalized();

        if (Input.IsActionJustPressed("jump"))
            _jumpBuffer = true;

        if (Input.IsActionJustPressed("throw"))
            Throw(_aim);

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

    public void Throw(Vector2 direction)
    {
        _throwingBehaviour.Throw(Position, direction, _throwable);
    }
}