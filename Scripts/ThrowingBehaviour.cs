using Godot;

public class ThrowingBehaviour
{
    private float _offset;
    private float _speed;
    private float _gravity;

    public ThrowingBehaviour(float offset, float speed, float gravity)
    {
        _offset = offset;
        _speed = speed;
        _gravity = gravity;
    }

    public void Throw(Vector2 throwerPosition, Vector2 direction, Throwable throwable)
    {
        throwable.Throw(throwerPosition + direction * _offset, direction * _speed, _gravity);
    }
}