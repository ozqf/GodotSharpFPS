using Godot;
using Godot.Collections;
using GodotSharpFps.src;
using GodotSharpFps.src.nodes;
using System;

public class PointProjectile : Spatial
{
	private enum State { Live, Dead, Embedded };
	private State _state = State.Dead;
	private string impactGFX = GameFactory.Path_GFXBulletImpact;

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

	private void RunImpactDef(Vector3 origin, ProjectileImpactDef impactDef)
	{

	}

	private void Die()
	{
		_state = State.Dead;
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
		// Set origin back slightly as otherwise rays seem to tunnel
		// (perhaps due to ray starting inside collider...?)
		origin -= (forward * 0.25f);
		uint mask = uint.MaxValue;

		Dictionary hitResult = ZqfGodotUtils.CastRay(this, origin, dest, mask, _ignoreBody);
		bool spawnGFX = true;
		if (hitResult.Keys.Count > 0)
		{
			_touch.hitPos = (Vector3)hitResult["position"];
			_touch.hitNormal = (Vector3)hitResult["normal"];
			IActor actor = Game.ExtractActor(hitResult["collider"]);
			if (actor != null)
			{
				_touch.damage = _def.damage;
				_touch.teamId = 0;
				_touch.touchType = TouchType.Bullet;

				TouchResponseData response = actor.ActorTouch(_touch);
				if (response.responseType != TouchResponseType.None)
				{
					// leave spawning particles to the victim
					spawnGFX = false;
				}
			}
			if (spawnGFX)
			{
				GFXQuick gfx = Main.i.factory.SpawnGFX(impactGFX);
				gfx.Spawn(_touch.hitPos, _touch.hitNormal);
			}
			if (_def.impactDef != null)
			{
				RunImpactDef(_touch.hitPos, _def.impactDef);
			}
			if (_def.destroyMode == ProjectileDef.DestroyMode.Embed)
			{
				_state = State.Embedded;
				t.origin = _touch.hitPos;
				_tick = 3;
				GlobalTransform = t;
			}
			else
			{
				Die();
			}
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

		if (_state == State.Live)
		{
			MoveAsRay(delta);
		}
	}

	public void Launch(
		Vector3 origin,
		Vector3 forward,
		ProjectileDef def,
		PhysicsBody ignoreBody,
		Team team)
	{
		_ignoreBody = ignoreBody;
		_def = def;
		_state = State.Live;

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
