using System;
using Godot;

public class Player : RigidBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    public float MoveSpeed = 100;
    private RigidBody2D rb;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // rb = GetNode<RigidBody2D>("Player");
        GD.Print("Test");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2 input = new Vector2();
        input.x -= Input.GetActionStrength("MoveLeft");
        input.x += Input.GetActionStrength("MoveRight");
        input *= delta * MoveSpeed;
        // if (InputEvent.IsActionPressed("Jump"))
        // {
        AddForce(Vector2.Zero, input);
        // this.AddForce(Vector2.Zero, new Vector2(input));
        // RigidBody2D rb = ((RigidBody2D) this).Addforce();
        // instance.AddForce();
        // G
        // }
    }

    // public override void _PhysicsProcess(float delta)
    // {
    //     Vector2 input = new Vector2();
    //     input.x -= Input.GetActionStrength("MoveLeft");
    //     input.x += Input.GetActionStrength("MoveRight");

    //     MoveAndCollide(input.Normalized() * MoveSpeed * delta);
    //     RigidBody2D rb = new RigidBody2D();
    //     AddForce()
    //     // rb.AddForce()
    // }
}
