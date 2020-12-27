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

    private bool _isGrounded;

    private float _jumpVelocity;
    private float _jumpGravity;
    private float _fallGravity;

    private bool _jumpThisTick;

    // private float _bufferedHorizontalAccleration;
    private bool _accelerateThisFrame;
    private int _accelerationDir;

    private Vector2 _currentVelocity;

    private Vector2 _previousVelocity;

    // References
    private KinematicBody2D _kb;
    
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

        // _accelerationConstant = CalculateAccelerationConstant(0f, _maxSpeed, _timeToMaxSpeed);
        // _decelerationConstant = CalculateAccelerationConstant(_maxSpeed, 0f, _timeToTurn);

        _jumpGravity = CalculateGravityConstant(_jumpHeight, _jumpDuration);
        _fallGravity = CalculateGravityConstant(_jumpHeight, _fallDuration);
        _jumpVelocity = CalculateJumpVelocity(_jumpHeight, jumpDuration);

        _accelerateThisFrame = false;
        _accelerationConstant = _maxSpeed / (_timeToMaxSpeed * _timeToMaxSpeed);
        _decelerationConstant = _maxSpeed / (_timeToTurn * _timeToTurn);
        _frictionConstant = CalculateAccelerationConstant(_maxSpeed, 0f, _timeToStop);
    }

    private float CalculateT(float y, float a)
    {
        return Mathf.Sqrt(y / a);
    }

    public void ProcessPhysics(float delta)
    {
        _isGrounded = _kb.IsOnFloor();

        // Acceleration
        if (_accelerateThisFrame)
        {
            int velocityDir = Mathf.Sign(_currentVelocity.x);

            if (velocityDir != 0 && velocityDir != _accelerationDir)   
                Decelerate(ref _currentVelocity.x, _accelerationDir, delta);
            else
                Accelerate(ref _currentVelocity.x, _accelerationDir, delta);
        }

        // If the player isn't accelerating, apply friction.
        if (!_accelerateThisFrame)
            _currentVelocity.x += CalculateCounterAcceleration(_previousVelocity.x, _frictionConstant, delta) * delta;

        // Execute a jump if a jump has been buffered.
        if (_jumpThisTick)
            _currentVelocity.y = -_jumpVelocity;

        // Determine which gravity to apply.
        float gravityThisFrame = _currentVelocity.y < 0f ? _jumpGravity : _fallGravity;
        float verticalAcceleration = gravityThisFrame;

        // INTEGRATE NEW VELOCITY
        _currentVelocity.y += verticalAcceleration * delta;
        _currentVelocity.x = Mathf.Clamp(_currentVelocity.x, -_maxSpeed, _maxSpeed);

        // Applying computated velocity to KinematicBody.
        bool airborne = !_kb.IsOnFloor();
        _currentVelocity = _kb.MoveAndSlide(_currentVelocity, Vector2.Up);

        #region Debug logs
        // HORIZONTAL MOVEMENT RELATED DEBUG MESSAGES.
        //if (_previousVelocity.x == 0f && _bufferedHorizontalAccleration != 0f)
        //    GD.Print($"Started accelerating: {debugTime}");

        //if (_previousVelocity.x != 0f &&_bufferedHorizontalAccleration == 0f)
        //    GD.Print($"Stopped accelerating: {debugTime}");

        //if (Mathf.Abs(_previousVelocity.x) > 0f && _currentVelocity.x == 0f)
        //    GD.Print($"Stopped moving: {debugTime}");

        // JUMP RELATED DEBUG MESSAGES.
        //if (_jumpThisTick)
        //    GD.Print($"Jumped: {debugTime}");

        //if (_previousVelocity.y < 0f && _currentVelocity.y >= 0f)
        //    GD.Print($"Reached apex of jump: {debugTime}");

        //if (_kb.IsOnFloor() && airborne)
        //    GD.Print($"Landed: {debugTime}");
        #endregion

        _previousVelocity = _currentVelocity;

        // Resetting buffered values for next frame.
        _accelerateThisFrame = false;
        _accelerationDir = 0;
        _jumpThisTick = false;
    }

    private void Accelerate(ref float velocity, int direction, float delta)
    {
        float t = CalculateT(velocity, direction * _accelerationConstant) + delta;
        velocity = direction * _accelerationConstant * (t * t);
    }

    private void Decelerate(ref float velocity, int direction, float delta)
    {
        int velocityDir = Mathf.Sign(velocity);
        float t = CalculateT(velocity, velocityDir * _decelerationConstant) - delta;
        velocity = velocityDir * _decelerationConstant * (t * t);

        if (t <= 0.075f)
        {
            velocity = 0f;
            Accelerate(ref velocity, direction, delta);
        }
    }

    // IDK WHY I WROTE THIS FUNCTION, BUT LEAVE IT HERE.
    private float CalculateCounterAcceleration(float currentVelocity, float counterAcceleration, float delta)
    {
        int currentDirection = Mathf.Sign(currentVelocity);
        float acceleration = -currentDirection * counterAcceleration;

        if (Mathf.Sign(currentVelocity + acceleration * delta) != currentDirection)
            acceleration = (0f - currentVelocity) / delta;

        return acceleration;
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

    private float CalculateAccelerationConstant(float startVelocity, float targetVelocity, float time)
    {
        float deltaSpeed = Mathf.Abs(targetVelocity - startVelocity);
        return deltaSpeed / time;
    }

    private float CalculateGravityConstant(float height, float time)
    {
        return (2f * height) / (time * time);
    }

    // Calculates the initial velocity required in terms of gravity.
    private float CalculateJumpVelocity(float height, float time)
    {
        return (2f * height) / time;
    }

    public void Move(int direction)
    {
        _accelerationDir = direction;
        _accelerateThisFrame = direction != 0;
    }

    public void Jump()
    {
        _jumpThisTick = true;
    }

    public float GetHorizontalVelocity => _currentVelocity.x;
    public float GetVerticalVelocity => _currentVelocity.y;
    public Vector2 GetVelocity => _currentVelocity;
    public bool IsGrounded => _isGrounded;
}