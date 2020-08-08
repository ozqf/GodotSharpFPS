using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
    public class BulkSpawner : Spatial, IActor, IActorProvider
    {
        private int _entId = 0;
		private int _numLiveChildren = 0;
		private float _tick = 0;
		private bool _isSpawning = false;

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

		private void SpawnChild()
		{
			EntMob mob = Main.i.factory.SpawnMob();
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
