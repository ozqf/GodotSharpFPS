using Godot;
using System.Collections.Generic;

namespace GodotSharpFps.src.nodes
{
	public class KinematicWrapper : KinematicBody, ITouchable, IActorProvider
	{
		public IActor actor { get; set; }

		// Gather display elements
		private List<Spatial> _displayNodes = new List<Spatial>();
		private CollisionShape _shape = null;
		private RayCast _groundCheckCentre = null;

		private int _health = 100;
		// health reset back to if revived
		private int _maxHealth = 100;
		private bool _dead = false;

		// callbacks
		private HealthChange _onHealthChange = null;
		private Death _onDeath = null;

		public void InitHealth(int current, int max)
		{
			_health = current;
			_maxHealth = max;
		}

		public void SetCallbacks(HealthChange onHealthChange, Death onDeath)
		{
			_onHealthChange = onHealthChange;
			_onDeath = onDeath;
		}

		public bool IsGrounded()
		{
			return _groundCheckCentre.IsColliding();
		}

		public void ShowModels()
		{
			for (int i = 0; i < _displayNodes.Count; ++i)
			{
				_displayNodes[i].Show();
			}
		} 

		public void HideModels()
		{
			for (int i = 0; i < _displayNodes.Count; ++i)
			{
				_displayNodes[i].Hide();
			}
		}

		public Transform GetTransformForTarget()
		{
			return _shape.GlobalTransform;
		}

		public override void _Ready()
		{
			if (HasNode("MeshInstance"))
			{
				_displayNodes.Add(GetNode<Spatial>("MeshInstance"));
			}
			if (HasNode("head/MeshInstance"))
			{
				_displayNodes.Add(GetNode<Spatial>("head/MeshInstance"));
			}
			if (HasNode("weapon/MeshInstance"))
			{
				_displayNodes.Add(GetNode<Spatial>("weapon/MeshInstance"));
			}
			if (HasNode("ground_check_centre"))
			{
				_groundCheckCentre = GetNode<RayCast>("ground_check_centre");
			}
			// Need shape so we can report its position.
			_shape = GetNode<CollisionShape>("CollisionShape");
		}

		public TouchResponseData Touch(TouchData touchData)
		{
			if (_dead) { return TouchResponseData.empty; }

			int previous = _health;
			_health -= touchData.damage;

			TouchResponseData result;
			result.damageTaken = touchData.damage;
			GFXQuick gfx = Main.i.factory.SpawnGFX(GameFactory.Path_GFXBloodImpact);
			gfx.Spawn(touchData.hitPos, touchData.hitNormal);
			if (_health <= 0)
			{
				_dead = true;
				result.responseType = TouchResponseType.Killed;
				_onDeath?.Invoke();
			}
			else
			{
				int change = _health - previous;
				result.responseType = TouchResponseType.Damaged;
				_onHealthChange?.Invoke(_health, change, touchData);
			}

			return result;
		}

		public IActor GetActor()
		{
			return actor;
		}
	}
}
