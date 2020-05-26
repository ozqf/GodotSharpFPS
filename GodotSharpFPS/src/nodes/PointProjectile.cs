using Godot;
using Godot.Collections;
using GodotSharpFps.src;
using System;

public class PointProjectile : Spatial
{
	private ProjectileDef _def = null; // def should be readonly
	private float _tick = 0;
	private bool _isDead = false;

	private PhysicsBody _ignoreBody = null;

	private void Die()
	{
		_isDead = true;
		QueueFree();
	}

	private void MoveAsRay(float delta)
	{
		Transform t = GlobalTransform;
		Vector3 origin = t.origin;
		Vector3 forward = -t.basis.z;
		Vector3 dest = origin;
		dest += (forward * _def.launchSpeed) * delta;
		uint mask = uint.MaxValue;
		PhysicsDirectSpaceState space = GetWorld().DirectSpaceState;
		Godot.Collections.Array arr = null;
		if (_ignoreBody != null)
		{
			arr = new Godot.Collections.Array();
			arr.Add(_ignoreBody);
		}
		Dictionary hitResult = space.IntersectRay(origin, dest, arr, mask);
		if (hitResult.Keys.Count > 0)
		{
			Node hitObj = (hitResult["collider"] as Node).GetParent();
			Console.WriteLine($"Prj hit {hitObj.Name}");
			Die();
			return;
		}
		t.origin = dest;
		GlobalTransform = t;
	}

	public override void _PhysicsProcess(float delta)
	{
		if (_isDead) { return; }
		if (_tick <= 0)
		{
			Die();
			return;
		}
		_tick -= delta;

		MoveAsRay(delta);

		//Transform t = GlobalTransform;
		//Vector3 velocity = -t.basis.z * _def.launchSpeed;
		//t.origin += velocity * delta;
		//GlobalTransform = t;
	}

	public void Launch(Transform globalOrigin, ProjectileDef def, PhysicsBody ignoreBody)
	{
		_ignoreBody = ignoreBody;
		_def = def;
		Vector3 origin = globalOrigin.origin;
		Console.WriteLine($"Prj spawned at {origin.x}, {origin.y}, {origin.z}");
		GlobalTransform = globalOrigin;
		_tick = def.timeToLive;
	}
}
