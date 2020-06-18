using Godot;
using Godot.Collections;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;
using System;

public class PointProjectile : Spatial
{
	private ProjectileDef _def = null; // def should be readonly
	private float _tick = 0;
	private bool _isDead = false;

	private int _hideTicksMax = 3;
	private int _hideTicks = 2;

	private PhysicsBody _ignoreBody = null;

	private TouchData _touch = new TouchData();

	public void Respawn()
	{
		_isDead = false;
	}

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
			_touch.damage = _def.damage;
			_touch.teamId = 0;
			_touch.touchType = TouchType.Bullet;
			IActorProvider actorProvider = hitResult["collider"] as IActorProvider;
			if (actorProvider != null)
			{
				IActor actor = actorProvider.GetActor();
				if (actor != null)
				{
					TouchResponseData response = actor.ActorTouch(_touch);
					Console.WriteLine($"Prj hit actor for {response.damageTaken}");
				}
				else
				{
					Console.WriteLine($"Prj hit provider but actor is null!");
				}
				
			}
			else
			{
				//Node hitObj = (hitResult["collider"] as Node);
				//Console.WriteLine($"Prj hit non-actor node {hitObj.Name}");
			}
			Vector3 gfxOrigin = (Vector3)hitResult["position"];
			GFXQuick gfx = Main.i.factory.SpawnGFX(GameFactory.Path_GFXImpact);
			gfx.Spawn(gfxOrigin);
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

		_hideTicks--;
		if (_hideTicks <= 0)
		{
			Show();
		}

		MoveAsRay(delta);

		//Transform t = GlobalTransform;
		//Vector3 velocity = -t.basis.z * _def.launchSpeed;
		//t.origin += velocity * delta;
		//GlobalTransform = t;
	}

	/*public void Launch(Transform globalOrigin, ProjectileDef def, PhysicsBody ignoreBody)
	{
		_ignoreBody = ignoreBody;
		_def = def;
		Vector3 origin = globalOrigin.origin;
		Console.WriteLine($"Prj spawned at {origin.x}, {origin.y}, {origin.z}");
		GlobalTransform = globalOrigin;
		_tick = def.timeToLive;
	}*/

	public void Launch(Vector3 origin, Vector3 forward, ProjectileDef def, PhysicsBody ignoreBody)
	{
		_ignoreBody = ignoreBody;
		_def = def;

		Transform t = GlobalTransform;
		t.origin = new Vector3();
		GlobalTransform = t;
		Vector3 lookPos = t.origin + forward;
		this.LookAt(lookPos, Vector3.Up);

		//Vector3 origin = globalOrigin.origin;
		t.origin = origin;
		//Console.WriteLine($"Prj spawned at {origin.x}, {origin.y}, {origin.z}");
		GlobalTransform = t;
		_tick = def.timeToLive;
		//Console.WriteLine($"Prj TTL {_tick}");
		_hideTicks = _hideTicksMax;
		Hide();
	}
}
