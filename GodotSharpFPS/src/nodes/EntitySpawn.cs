using Godot;

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
