using Godot;
using System;
using GodotSharpFps.src;

public class EntPlayer : Spatial
{
    private const string MoveForward = "move_forward";
    private const string MoveBackward = "move_backward";
    private const string MoveLeft = "move_left";
    private const string MoveRight = "move_right";

    private FPSInput _input = new FPSInput();
    
    private FPSController _fpsCtrl;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        KinematicBody body = GetNode<KinematicBody>("body");
        _fpsCtrl = new FPSController(body);
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        _input.buttons = 0;

        if (Input.IsActionPressed(MoveForward))
        { _input.buttons |= FPSInput.BitMoveForward; }
        if (Input.IsActionPressed(MoveBackward))
        { _input.buttons |= FPSInput.BitMoveBackward; }
        if (Input.IsActionPressed(MoveLeft))
        { _input.buttons |= FPSInput.BitMoveLeft; }
        if (Input.IsActionPressed(MoveRight))
        { _input.buttons |= FPSInput.BitMoveRight; }

        _fpsCtrl.ProcessInput(_input, delta);
    }
}
