using Godot;

public class ForceBasedCharacterController
{
    // Force based character controller for a side scrolling character.

    // TODO LATER: Jumping.
    // TODO LATER: Seperate deceleration when changing directions.

    private float _maxSpeed;
    private float _acceleration;
    private float _gravity;

    private float _friction;
    private float _deceleration;

    private float _bufferedHorizontalAccleration;
    private Vector2 _velocity;

    private KinematicBody2D _kb;

    public ForceBasedCharacterController(KinematicBody2D kb, float maxSpeed, float acceleration, float gravity, float friction, float deceleration)
    {
        _kb = kb;
        _maxSpeed = maxSpeed;
        _acceleration = acceleration;
        _gravity = gravity;
        _friction = friction;
        _deceleration = deceleration;
    }

    public void ProcessPhysics(float delta)
    {
        // Applying acceleration from buffered acceleration.
        _velocity.x += _bufferedHorizontalAccleration * delta;
        _velocity.x = Mathf.Clamp(_velocity.x, -_maxSpeed, _maxSpeed); // Clamp horizontal velocity to "_maxSpeed".

        // Applying friction.
        if (_bufferedHorizontalAccleration == 0f)
            CalculateCounterForce(ref _velocity, _friction * delta);
        else if (Mathf.Sign(_bufferedHorizontalAccleration) != Mathf.Sign(_velocity.x))
            CalculateCounterForce(ref _velocity, _deceleration * delta);

        // Applying gravity.
        _velocity.y += _gravity * delta;

        // Resetting buffered values for next frame.
        _bufferedHorizontalAccleration = 0f;

        // Applying computated velocity to KinematicBody.
        _velocity = _kb.MoveAndSlide(_velocity, Vector2.Up);
    }

    private void CalculateCounterForce(ref Vector2 velocity, float counterForce)
    {
        float currentDirection = Mathf.Sign(velocity.x);
        float preForceHorizontalVelocity = velocity.x;
        float postForceHorizontalVelocity = velocity.x + -currentDirection * counterForce;

        velocity.x = postForceHorizontalVelocity;
        if (Mathf.Sign(preForceHorizontalVelocity) != Mathf.Sign(postForceHorizontalVelocity))
            velocity.x = 0f;
    }

    public void Move(int direction)
    {
        _bufferedHorizontalAccleration = direction * _acceleration;
    }

    public void Jump()
    {
        GD.Print("'Jump()' still in development!");
    }
}