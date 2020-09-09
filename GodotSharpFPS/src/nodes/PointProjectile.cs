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

	// def should be considered readonly - stores static settings.
	private ProjectileDef _def = null;
	private float _tick = 0;
	private float _speed = 0;
	private bool _isDead = false;

	private Team _team = Team.None;
	private int _targetActorId = Game.NullActorId;

	private int _hideTicksMax = 3;
	private int _hideTicks = 2;

	private PhysicsBody _ignoreBody = null;

	// TODO: No need to pre-alloc this it is a struct!
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

		// apply move effects
		if (_def.speedMode == ProjectileDef.SpeedMode.Accel)
		{
			_speed += _def.accelPerSecond * delta;
			_speed = ZqfGodotUtils
				.Capf(_speed, _def.minSpeed, _def.maxSpeed);
		}

		if (_def.guideMode == ProjectileDef.GuideMode.Turn
			&& _targetActorId != Game.NullActorId)
        {
			IActor actor = Main.i.game.GetActor(_targetActorId);
			// TODO lerp rotation toward target
			Transform turnTarget = actor.GetTransformForTarget();
		}

		Vector3 origin = t.origin;
		Vector3 forward = -t.basis.z;
		Vector3 dest = origin;
		dest += (forward * _speed) * delta;
		// Set origin back slightly as otherwise rays seem to tunnel
		// (perhaps due to ray starting inside collider...?)
		origin -= (forward * 0.25f);
		uint mask = uint.MaxValue;

		Dictionary hitResult = ZqfGodotUtils.CastRay(this, origin, dest, mask, _ignoreBody);
		bool spawnGFX = true;

		// no hits, just move and leave
		if (hitResult.Keys.Count <= 0)
		{
			t.origin = dest;
			GlobalTransform = t;
			return;
		}

		_touch.hitPos = (Vector3)hitResult["position"];
		_touch.hitNormal = (Vector3)hitResult["normal"];
		_touch.damage = _def.damage;
		_touch.teamId = _team;
		_touch.touchType = TouchType.Projectile;
		_touch.damageType = _def.damageType;

		object obj = hitResult["collider"];
		/*
		IActor actor = Game.ExtractActor(obj);
		//ITouchable actor = Game.ExtractTouchable(obj);
		if (actor != null)
		{
			TouchResponseData response = actor.ActorTouch(_touch);
			if (response.responseType != TouchResponseType.None)
			{
				// leave spawning particles to the victim
				spawnGFX = false;
			}
		}
		*/
		TouchResponseData response = Game.TouchGameObject(_touch, obj);
		if (response.responseType == TouchResponseType.None)
		{
			t.origin = dest;
			GlobalTransform = t;
			return;
		}
		if (response.responseType != TouchResponseType.None)
		{
			// leave spawning particles to the victim
			spawnGFX = false;
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
		if (_hideTicks <= 0) { Show(); }

		if (_state == State.Live) { MoveAsRay(delta); }
	}

	public void Launch(
		Vector3 origin,
		Vector3 forward,
		ProjectileDef def,
		PhysicsBody ignoreBody,
		Team team,
		int targetActorId = Game.NullActorId)
	{
		_ignoreBody = ignoreBody;
		_def = def;
		_state = State.Live;

		Transform t = GlobalTransform;
		t.origin = new Vector3();
		GlobalTransform = t;
		Vector3 lookPos = t.origin + forward;
		this.LookAt(lookPos, Vector3.Up);

		t.origin = origin;
		GlobalTransform = t;
		_tick = def.timeToLive;
		_speed = def.launchSpeed;
		_team = team;
		_targetActorId = targetActorId;
		_hideTicks = _hideTicksMax;
		Hide();
	}
}
