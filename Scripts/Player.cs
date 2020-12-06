using System;
using Godot;

public class Player : KinematicBody2D
{
    [Export] private float MoveSpeed;
    [Export] private float jumpImpulse;
    [Export] private float Gravity;

    private float _horizontalInput = 0f;
    private bool _jump;
    private VelocityBasedCharacterController _cc;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _cc = new VelocityBasedCharacterController(this, MoveSpeed, jumpImpulse, Gravity);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _horizontalInput = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        if (Input.IsActionJustPressed("jump"))
            _jump = true;
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        _cc.Move((int)_horizontalInput);

        if (_jump)
        {
            _cc.Jump();
            _jump = false;
        }

        _cc.ProcessPhysics(delta);
    }
}