using Godot;

public class ForceBasedCharacterController
{
    // Force based character controller for a side scrolling character.

    // TODO LATER: Jumping.
    // TODO LATER: Seperate deceleration when changing directions.

    private float _maxSpeed;
    private float _acceleration;
    private float _friction;
    private float _gravity;

    private float _bufferedHorizontalAccleration;
    private Vector2 _velocity;

    private KinematicBody2D _kb;

    public ForceBasedCharacterController(KinematicBody2D kb, float maxSpeed, float acceleration, float friction, float gravity)
    {
        _kb = kb;
        _maxSpeed = maxSpeed;
        _acceleration = acceleration;
        _friction = friction;
        _gravity = gravity;
    }

    public void ProcessPhysics(float delta)
    {
        // Applying acceleration from buffered acceleration.
        _velocity.x += _bufferedHorizontalAccleration * delta;
        _velocity.x = Mathf.Clamp(_velocity.x, -_maxSpeed, _maxSpeed); // Clamp horizontal velocity to "_maxSpeed".

        // Applying friction.
        float currentDirection = Mathf.Sign(_velocity.x);
        float preFrictionHorizontalVelocity = _velocity.x;
        float postFrictionHorizontalVelocity = _velocity.x + -currentDirection * _friction * delta;

        _velocity.x = postFrictionHorizontalVelocity;
        if (Mathf.Sign(preFrictionHorizontalVelocity) != Mathf.Sign(postFrictionHorizontalVelocity))
            _velocity.x = 0f;

        // Applying gravity.
        _velocity.y += _gravity * delta;

        // Resetting buffered values for next frame.
        _bufferedHorizontalAccleration = 0f;

        // Applying computated velocity to KinematicBody.
        _velocity = _kb.MoveAndSlide(_velocity, Vector2.Up);
    }

    public void Move(int direction)
    {
        _bufferedHorizontalAccleration = direction * (_acceleration + _friction);
    }

    public void Jump()
    {
        GD.Print("'Jump()' still in development!");
    }
}