using Godot;
using GodotSharpFps.src;
using System;

public class PointProjectile : Spatial
{
	private ProjectileDef _def = null; // def should be readonly
	private float _tick = 0;

	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(float delta)
	{
		if (_tick <= 0)
		{
			QueueFree();
			return;
		}
		_tick -= delta;
		Transform t = GlobalTransform;
		Vector3 velocity = -t.basis.z * _def.launchSpeed;
		t.origin += velocity * delta;
		GlobalTransform = t;
	}

	public void Launch(Transform globalOrigin, ProjectileDef def)
	{
		_def = def;
		Vector3 origin = globalOrigin.origin;
		Console.WriteLine($"Prj spawned at {origin.x}, {origin.y}, {origin.z}");
		GlobalTransform = globalOrigin;
		_tick = def.timeToLive;
	}
}
