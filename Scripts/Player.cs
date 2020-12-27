using System;
using Godot;
using Graphing_Server;

public class Player : KinematicBody2D
{
    [Export] private float MaxSpeed;
    [Export] private float TimeToMaxSpeed;  // Time to go from 0 - max speed.
    [Export] private float TimeToTurn;      // Time to change directions from max speed.
    [Export] private float TimeToStop;      // Time to go from max speed - 0.

    [Export] private float JumpHeight;      //config     
    [Export] private float JumpDuration;    //config
    [Export] private float FallDuration;

    private float _horizontalInput = 0f;
    private bool _jump;
    private ForceBasedCharacterController _cc;

    private AnimationTree _animTree;
    private GraphServer _gServer;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _cc = new ForceBasedCharacterController(this, MaxSpeed, TimeToMaxSpeed, TimeToTurn, TimeToStop, JumpHeight, JumpDuration, FallDuration);
        _animTree = GetNode<AnimationTree>("AnimationTree");
        _gServer = new GraphServer("localhost", 8080, 100000, new GDLogger());
        _gServer.StartServer();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _horizontalInput = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

        if (Input.IsActionJustPressed("jump"))
            _jump = true;

        _animTree.Set("parameters/movement/current", _cc.GetHorizontalVelocity != 0f && _cc.IsGrounded);
    }

    GraphData data;
    float time;
    public override void _PhysicsProcess(float delta)
    {
        time += delta;
        base._PhysicsProcess(delta);

        _cc.Move((int)_horizontalInput);

        if (_jump)
        {
            _cc.Jump();
            _jump = false;
        }

        _cc.ProcessPhysics(delta);
        data.y = _cc.GetVelocity.x;
        data.x = time;
        _gServer.SendData(data);
    }
}

public class GDLogger : ILogger
{
    public void Log(string message)
    {
        GD.Print(message);
    }

    public void Log(object message)
    {
        GD.Print(message);
    }
}