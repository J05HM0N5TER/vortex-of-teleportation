using Godot;

public class PhysicsBody2D : KinematicBody2D
{
    protected Vector2 _gravity;
    protected Vector2 _velocity;

    private KinematicCollision2D _collision;
    private KinematicCollision2D _collisionPreviousFrame;

    public override void _PhysicsProcess(float delta)
    {
        Vector2 acceleration = _gravity;
        _velocity += acceleration * delta;
        _collision = MoveAndCollide(_velocity * delta);

        if (_collisionPreviousFrame == null && _collision != null)
            OnCollisionEnter(_collision);

        _collisionPreviousFrame = _collision;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }

    public void SetGravity(Vector2 gravity)
    {
        _gravity = gravity;
    }

    protected virtual void OnCollisionEnter(KinematicCollision2D collision)
    {
        _velocity = Vector2.Zero;
        GD.Print($"Entered a collision.");
    }

    public Vector2 GetVelocity => _velocity;
}
