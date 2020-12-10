using Godot;

public class ForceBasedCharacterController
{
    // Force based character controller for a side scrolling character.

    // TODO LATER: Jumping.
    // TODO LATER: Seperate deceleration when changing directions.

    // Configuration
    private float _maxSpeed;
    private float _timeToMaxSpeed;  // Time to go from 0 - max speed.
    private float _timeToTurn;      // Time to change directions from max speed.
    private float _timeToStop;      // Time to go from max speed - 0.

    private float _jumpHeight;
    private float _jumpDuration;    // Time to reach apex of the jump.
    private float _fallDuration;

    // State
    private float _accelerationConstant;
    private float _decelerationConstant;
    private float _frictionConstant;

    private float _jumpVelocity;
    private float _jumpGravity;
    private float _fallGravity;

    private float _bufferedHorizontalAccleration;
    private float _bufferedJumpVelocity;
    private Vector2 _velocity;

    // References
    private KinematicBody2D _kb;

    float time = 0f;
    public ForceBasedCharacterController(KinematicBody2D kb, float maxSpeed, float timeToMaxSpeed, float timeToTurn, float timeToStop, float jumpHeight, float jumpDuration, float fallDuration)
    {
        _kb = kb;
        _maxSpeed = maxSpeed;
        _timeToMaxSpeed = timeToMaxSpeed;
        _timeToTurn = timeToTurn;
        _timeToStop = timeToStop;

        _jumpHeight = jumpHeight;
        _jumpDuration = jumpDuration;
        _fallDuration = fallDuration;

        CalculateAcceleration(ref _accelerationConstant, 0f, _maxSpeed, _timeToMaxSpeed);   // acceleration.
        CalculateAcceleration(ref _decelerationConstant, _maxSpeed, 0f, _timeToTurn);       // deceleration.
        CalculateAcceleration(ref _frictionConstant, _maxSpeed, 0f, _timeToStop);           // friction.

        CalculateJump(ref _jumpGravity, ref _jumpVelocity, _jumpHeight, _jumpDuration);
        CalculateFallGravity(ref _fallGravity, _jumpHeight, _fallDuration);
    }

    public void ProcessPhysics(float delta)
    {
        time += delta;
        // Applying acceleration from buffered acceleration.
        int accelerationDirection = Mathf.Sign(_bufferedHorizontalAccleration);
        if (accelerationDirection != Mathf.Sign(_velocity.x))
            _bufferedHorizontalAccleration = accelerationDirection * _decelerationConstant;

        _velocity.x += _bufferedHorizontalAccleration * delta;
        _velocity.x = Mathf.Clamp(_velocity.x, -_maxSpeed, _maxSpeed); // Clamp horizontal velocity to "_maxSpeed".

        if (_bufferedJumpVelocity > 0f)
        {
            _velocity.y = -_bufferedJumpVelocity;
            GD.Print($"Jumped: {time}");
        }

        // Applying friction.
        if (_bufferedHorizontalAccleration == 0f)
            CalculateCounterForce(ref _velocity, _frictionConstant * delta);

        // Applying gravity.
        float gravityThisFrame;
        if (_velocity.y < 0f)
            gravityThisFrame = _jumpGravity;
        else
            gravityThisFrame = _fallGravity;

        if (_velocity.y < 0f && _velocity.y + gravityThisFrame * delta >= 0f)
            GD.Print($"Reached Apex of Jump: {time}");

        _velocity.y += gravityThisFrame * delta;

        // Resetting buffered values for next frame.
        _bufferedHorizontalAccleration = 0f;
        _bufferedJumpVelocity = 0f;

        // Applying computated velocity to KinematicBody.
        bool airborne = !_kb.IsOnFloor();
        _velocity = _kb.MoveAndSlide(_velocity, Vector2.Up);

        if (_kb.IsOnFloor() && airborne)
            GD.Print($"Landed: {time}");
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

    private void CalculateAcceleration(ref float acceleration, float startVelocity, float targetVelocity, float time)
    {
        float deltaVelocity = Mathf.Abs(targetVelocity - startVelocity);
        acceleration = deltaVelocity / time;
    }

    private void CalculateJump(ref float gravity, ref float initialVelocity, float height, float time)
    {
        gravity = (2 * height) / (time * time); // positive gravity in godot.
        initialVelocity = (2f * height) / time;
    }

    private void CalculateFallGravity(ref float gravity, float height, float time)
    {
        gravity = (2 * height) / (time * time); // positive gravity in godot.
    }

    public void Move(int direction)
    {
        _bufferedHorizontalAccleration = direction * _accelerationConstant;
    }

    public void Jump()
    {
        _bufferedJumpVelocity = _jumpVelocity;
    }
}