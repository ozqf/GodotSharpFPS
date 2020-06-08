using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotSharpFps.src.nodes
{
	public class EntitySpawn : Spatial
	{
		[Export]
		private string entityType = string.Empty;
		[Export]
		private int entityId = 0;
	}
}
