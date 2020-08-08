using Godot;
using System.Collections.Generic;

namespace GodotSharpFps.src.nodes
{
	public class KinematicWrapper : KinematicBody, IActorProvider
	{
		public IActor actor { get; set; }

		// Gather display elements
		private List<Spatial> _displayNodes = new List<Spatial>();
		private CollisionShape _shape = null;
		private RayCast _groundCheckCentre = null;

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
			_displayNodes.Add(GetNode<Spatial>("MeshInstance"));
			_displayNodes.Add(GetNode<Spatial>("head/MeshInstance"));
			_displayNodes.Add(GetNode<Spatial>("weapon/MeshInstance"));
			_groundCheckCentre = GetNode<RayCast>("ground_check_centre");
			// Need shape so we can report its position.
			_shape = GetNode<CollisionShape>("CollisionShape");
		}

		public IActor GetActor()
		{
			return actor;
		}
	}
}
