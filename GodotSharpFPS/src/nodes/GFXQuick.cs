using Godot;

namespace GodotSharpFps.src.nodes
{
	public class GFXQuick : Spatial
	{
		private float _tick;
		private bool _dead;

		public void Spawn(Vector3 origin, Vector3 dir)
		{
			_dead = false;
			//_tick = 1;
			Particles p = GetNode<Particles>("Particles");
			p.Emitting = true;
			_tick = p.Lifetime;

			RotationDegrees = ZqfGodotUtils.CalcEulerDegrees(-dir);

			Transform t = GlobalTransform;
			t.origin = origin;
			GlobalTransform = t;
		}

		private void Die()
		{
			if (_dead) { return; }
			_dead = true;
			QueueFree();
		}

		public override void _PhysicsProcess(float delta)
		{
			if (_tick <= 0)
			{
				Die();
			}
			else
			{
				_tick -= delta;
			}
		}
	}
}
