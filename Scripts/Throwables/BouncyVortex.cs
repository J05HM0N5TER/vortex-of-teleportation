using Godot;

public class BouncyVortex : Throwable
{
    [Export] protected int _maxBounces;
    [Export] protected float _energyLoss; // A value between 0 - 1 | 0% - 100%

    private int _accumulatedBounces; // Amount of bounces occured since thrown.

    public override void Throw(Vector2 initialPosition, Vector2 initialVelocity, float gravity)
    {
        base.Throw(initialPosition, initialVelocity, gravity);
        _accumulatedBounces = 0;

        a = 1f / Mathf.Log(_maxBounces + 1);
    }

    float a;

    protected override void OnCollisionEnter(KinematicCollision2D collision)
    {
        _accumulatedBounces++;
        float mult = 1f - a * Mathf.Log(_accumulatedBounces + 1);

        GD.Print(mult);
        
        Vector2 collisionNormal = collision.Normal;
        Vector2 newVelocity = (_velocity - 2 * _velocity.Dot(collisionNormal) * collisionNormal) * mult;
        SetVelocity(newVelocity);
    }
}