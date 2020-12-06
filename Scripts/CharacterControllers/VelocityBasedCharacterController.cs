using Godot;
using System;

public class VelocityBasedCharacterController
{
    private float _moveSpeed;
    private float _jumpImpulse;
    private float _gravity;

    private int _currentDirection = 0;
    private float _bufferedJumpAcceleration = 0f;
    private Vector2 _velocity = new Vector2(0f, 0f);

    private KinematicBody2D _kb;

    public VelocityBasedCharacterController(KinematicBody2D kb, float moveSpeed, float jumpImpulse, float gravity)
    {
        _kb = kb;
        _moveSpeed = moveSpeed;
        _jumpImpulse = jumpImpulse;
        _gravity = gravity;
    }

    public void ProcessPhysics(float delta)
    {
        // Applying horizontal motion
        _velocity.x = _currentDirection * _moveSpeed;

        if (_bufferedJumpAcceleration > 0f)
            _velocity.y = -_bufferedJumpAcceleration / delta;

        _velocity.y += _gravity;
        _bufferedJumpAcceleration = 0f;
        _currentDirection = 0;
        
        _kb.MoveAndSlide(_velocity, Vector2.Up);
    }

    public void Move(int direction)
    {
        _currentDirection = direction;
    }

    public void Jump()
    {
        if (_kb.IsOnFloor())
            _bufferedJumpAcceleration = _jumpImpulse;
    }
}