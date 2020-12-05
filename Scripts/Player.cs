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
    // public override void _Ready()
    // {
    //     // rb = GetNode<RigidBody2D>("Player");
    //     GD.Print("Test");
    // }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2 input = new Vector2();
        input.x -= Input.GetActionStrength("MoveLeft");
        input.x += Input.GetActionStrength("MoveRight");
        input *= delta * MoveSpeed;

        AddForce(Vector2.Zero, input);
    }
}
