using Godot;
using System;

public class GameCamera : Camera
{
	private Node g_originalParent;
	public override void _Ready()
	{
		base._Ready();
		g_originalParent = GetParent();
	}

	public void Reset()
	{
		AttachToTarget(g_originalParent);
	}

	public void AttachToTarget(Node newParent)
	{
		//Transform t = GlobalTransform;
		Node parent = GetParent();
		parent.RemoveChild(this);
		newParent.AddChild(this);
		//GlobalTransform = t;
	}
}
