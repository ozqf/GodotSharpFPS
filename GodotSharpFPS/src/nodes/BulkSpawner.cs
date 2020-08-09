﻿using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
    public class BulkSpawner : Spatial, IActor, IActorProvider
    {
		enum SpawnMode { Burst, Trickle };
		private SpawnMode _mode = SpawnMode.Trickle;
		private int _patternIndex = 0;
        private int _entId = 0;
		private int _totalChildren = -1; // endless if negative
		private int _maxLiveChildren = 1;
		private int _numLiveChildren = 0;
		private float _tick = 0;
		private bool _isSpawning = true;

		private string _mobType = GameFactory.MobType_Pinkie;

        public IActor GetActor() => this;
		public void SetActorId(int newId) { _entId = newId; }
		public int ParentActorId { get; set; }
		public int actorId { get { return _entId; } }
		public Team GetTeam() { return Team.NonCombatant; }
		public void RemoveActor() { this.QueueFree(); }
		public Transform GetTransformForTarget() { return GlobalTransform; }
		public void ActorTeleport(Transform t) { GlobalTransform = t; }
		public void ChildActorRemoved(int id) { _numLiveChildren--; }

		public TouchResponseData ActorTouch(TouchData touchData)
		{
			return TouchResponseData.empty;
		}

		public string GetActorDebugText()
		{
			return $"BulkSpawner - Alive: {_numLiveChildren}, Mode {_mode}";
		}

		private void SpawnChild()
		{
			EntMob mob = Main.i.factory.SpawnMob(_mobType);
			if (mob == null)
			{
				Console.WriteLine($"BulkSpawner - failed to spawn child - disabling");
				_isSpawning = false;
				return;
			}
			ZqfGodotUtils.Teleport(mob, GlobalTransform.origin);
			mob.ParentActorId = _entId;
			_numLiveChildren++;
			_tick = 2;
			Console.WriteLine($"Bulk spawned mob {mob.actorId} parent {mob.ParentActorId}");
		}

		public override void _Ready()
		{
			base._Ready();
			Main.i.game.RegisterActor(this);
		}

		public override void _Process(float delta)
		{
			if (!_isSpawning) { return; }
			if (_numLiveChildren == 0)
			{
				_tick -= delta;
				if (_tick <= 0)
				{
					SpawnChild();
				}
			}
		}

	}
}
