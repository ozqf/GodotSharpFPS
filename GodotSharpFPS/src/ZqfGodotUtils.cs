using System;
using Godot;

public class ZqfGodotUtils
{
	public static T CreateInstance<T>(string path, Node parentNode = null)
	{
		PackedScene scene = (PackedScene)ResourceLoader.Load(path);
		Node instanceNode = scene.Instance();
		if (parentNode != null)
		{
			parentNode.AddChild(instanceNode);
		}
		T instance = (T)Convert.ChangeType(instanceNode, typeof(T));
		return instance;
	}
}
