using Godot;
using GodotSharpFps.src;

public class PointProjectile : Spatial
{
	public ProjectileDef def = null;

	public override void _Ready()
	{
		
	}

	public void Launch(Transform globalOrigin, ProjectileDef def)
	{
		GlobalTransform = globalOrigin;
	}
}
