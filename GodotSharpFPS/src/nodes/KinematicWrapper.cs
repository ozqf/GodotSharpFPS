using Godot;
using System.Collections.Generic;

namespace GodotSharpFps.src.nodes
{
	public class KinematicWrapper : KinematicBody, IActorProvider
	{
		public IActor actor { get; set; }

		// Gather display elements
		private List<Spatial> _displayNodes = new List<Spatial>();

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

		public override void _Ready()
		{
			_displayNodes.Add(GetNode<Spatial>("MeshInstance"));
			_displayNodes.Add(GetNode<Spatial>("head/MeshInstance"));
			_displayNodes.Add(GetNode<Spatial>("weapon/MeshInstance"));
		}

		public IActor GetActor()
		{
			return actor;
		}
	}
}
