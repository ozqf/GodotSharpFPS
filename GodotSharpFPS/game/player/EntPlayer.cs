using Godot;
using System;
using GodotSharpFps.src;

public class EntPlayer : Spatial
{
    private const string MoveForward = "move_forward";
    private const string MoveBackward = "move_backward";
    private const string MoveLeft = "move_left";
    private const string MoveRight = "move_right";
    private const string Attack1 = "attack_1";

    private FPSInput _input = new FPSInput();
    
    private FPSController _fpsCtrl;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        KinematicBody body = GetNode<KinematicBody>("body");
        _fpsCtrl = new FPSController(body);
        Main m = GetNode<Main>("/root/main");
        Console.WriteLine("Main: " + m.Name);
        Console.WriteLine("Main via instance: " + Main.instance.Name);
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

        _fpsCtrl.ProcessMovement(_input, delta);

        if (Input.IsActionJustPressed(Attack1))
        {
            PointProjectile prj = Main.instance.factory.SpawnPointProjectile();
            if (prj == null) { Console.WriteLine($"Got no prj instance"); return; }
            Transform transform = GetNode<KinematicBody>("body").GlobalTransform;
            prj.GlobalTransform = transform;
            Vector3 origin = transform.origin;
            Console.WriteLine($"Prj spawned at {origin.x}, {origin.y}, {origin.z}");
        }
    }
}
