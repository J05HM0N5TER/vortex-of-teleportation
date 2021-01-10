using Godot;

public abstract class Throwable : PhysicsBody2D
{
    public virtual void Throw(Vector2 initialPosition, Vector2 initialVelocity, float gravity)
    {
        Position = initialPosition;
        SetVelocity(initialVelocity);
        SetGravity(Vector2.Down * gravity);
    }
}