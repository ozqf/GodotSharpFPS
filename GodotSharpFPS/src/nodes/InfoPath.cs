using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
	public class InfoPath : Spatial
	{
		[Export]
		public string nextPathName = string.Empty;

		private Spatial _nextPath = null;

		public override void _Ready()
		{
			Main.i.game.RegisterPathNode(this);
			// assume next path candidate has same parent
			if (!string.IsNullOrWhiteSpace(nextPathName))
			{
				_nextPath = GetParent().GetNode<InfoPath>(nextPathName);
				if (_nextPath != null)
				{
					Console.WriteLine($"InfoPath found next {nextPathName}");
				}
				else
				{
					Console.WriteLine($"InfoPath failed to find next {nextPathName}");
				}
			}
		}

		public override void _ExitTree()
		{
			Main.i.game.DeregisterPathNode(this);
		}
	}
}
