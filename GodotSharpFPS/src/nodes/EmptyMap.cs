using Godot;
using System;

public class EmptyMap : Node
{
	public override void _Ready()
	{
		Console.WriteLine("Empty map - changing to default");
		//Main.i.console.Execute("map test_box");
		Main.i.console.Execute("map test_box_boss");
	}
}
