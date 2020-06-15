using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
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

	public static Vector3 VectorMA(
		Vector3 start, float scale, Vector3 dir)
	{
		Vector3 dest;
		dest.x = start.x + dir.x * scale;
		dest.y = start.y + dir.y * scale;
		dest.z = start.z + dir.z * scale;
		return dest;
	}

	public static void FillSpreadAngles(Transform tran, List<Vector3> results)
	{
		if (results == null) { return; }
		int len = results.Count;
		if (len == 0) { return; }
		Vector3 origin = tran.origin;
		Vector3 forward = -tran.basis.z;
		Vector3 up = tran.basis.y;
		Vector3 right = tran.basis.x;
		Random r = new Random();
		float spreadH = 2000;
		float spreadV = 1200;

		results[0] = forward;
		for (int i = 1; i < len; ++i)
		{
			Vector3 end = VectorMA(origin, 8192, forward);
			end = VectorMA(end, (float)spreadH * (float)r.NextDouble(), right);
			end = VectorMA(end, (float)spreadV * (float)r.NextDouble(), up);
			results[i] = (end - origin).Normalized();
		}
	}
}
