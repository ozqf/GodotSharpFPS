using Godot;
using System;

public class EmptyMap : Node
{
    public override void _Ready()
    {
        Console.WriteLine("Empty map - changing to default");
        Main.instance.console.Execute("map test_box");
    }
}
