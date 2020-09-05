using Godot;
using System;

public class GameCamera : Camera
{
	public enum ParentType { None, Misc, Player };
	private Node _originalParent;
	private Node _currentParent;
	private ParentType _parentType;

	public override void _Ready()
	{
		base._Ready();
		_originalParent = GetParent();
	}

	public ParentType GetParentType()
	{
		return _parentType;
	}

	public void DetachFromCustomParent()
	{
		if (_currentParent != null && _currentParent != _originalParent)
		{
			Transform t = GlobalTransform;
			AttachToTarget(_originalParent, Vector3.Zero, ParentType.None);
			GlobalTransform = t;
		}
	}

	public void Reset()
	{
		DetachFromCustomParent();
		Transform = Transform.Identity;
	}

	public void AttachToTarget(
		Node newParent, Vector3 offset, ParentType parentType = ParentType.Misc)
	{
		if (newParent == _currentParent) { return; }

		Node parent = GetParent();
		parent.RemoveChild(this);
		newParent.AddChild(this);
		_currentParent = newParent;
		Transform t = Transform.Identity;
		t.origin += offset;
		Transform = t;
		_parentType = parentType;
	}
}
