using System;
using System.CodeDom;
using System.Collections.Generic;
using Godot;

public class ZqfGodotUtils
{
	public const float TAU = (3.1415926535897931f * 2f);

	private static Random _random = new Random();

	public static Godot.Collections.Dictionary CastRay(
		Spatial worldSourceNode, Vector3 origin, Vector3 target, uint mask, PhysicsBody ignoreBody)
	{
		Godot.Collections.Array ignoreBodies = null;
		if (ignoreBody != null)
		{
			ignoreBodies = new Godot.Collections.Array();
			ignoreBodies.Add(ignoreBody);
		}
		PhysicsDirectSpaceState space = worldSourceNode.GetWorld().DirectSpaceState;
		
		return space.IntersectRay(origin, target, ignoreBodies, mask);
	}

	public static float Capf(float value, float min, float max)
	{
		if (value < min) { return min; }
		if (value > max) { return max; }
		return value;
	}

	/// <summary>
	/// TODO Replace with proper usage of Godot Viewports
	/// </summary>
	/// <returns></returns>
	public static Vector2 GetWindowToScreenRatio()
	{
		Vector2 real = OS.GetRealWindowSize();
		Vector2 screen = OS.GetScreenSize();
		return new Vector2(real.x / screen.x, real.y / screen.y);
	}

	public static float Randomf()
	{
		return (float)_random.NextDouble();
	}

	public static float RandomRange(float min, float max)
	{
		float r = (float)_random.NextDouble();
		r = r * (max - min) + min;
		return r;
	}
	
	public static void Teleport(Spatial node, Vector3 pos)
	{
		Transform t = node.GlobalTransform;
		t.origin = pos;
		node.GlobalTransform = t;
	}

	public static T CreateInstance<T>(string path, Node parentNode = null)
	{
		PackedScene scene = (PackedScene)ResourceLoader.Load(path);
		Node instanceNode = scene.Instance();
		if (parentNode != null)
		{
			parentNode.AddChild(instanceNode);
		}
		try
		{
			T instance = (T)Convert.ChangeType(instanceNode, typeof(T));
			return instance;
		}
		catch (InvalidCastException ex)
		{
			Console.WriteLine($"ERROR casting prefab at {path} to {typeof(T)}!");
			return default(T);
		}
	}

	public static void AddChildNodeToList<T>(Node parent, List<T> list, string path)
    {
		if (!parent.HasNode(path)) { return; }
		var typ = typeof(T);
		object obj = parent.GetNode(path);
		if (obj is T)
        {
			list.Add((T)obj);
        }
    }

	public static void GetAllChildrenOfType<T>(Node parent, List<T> list)
    {
		Godot.Collections.Array arr = parent.GetChildren();
		int len = arr.Count;
		for (int i = 0; i < len; ++i)
        {
			object child = arr[i];
			if (child is T)
            {
				list.Add((T)child);
            }
        }
    }

	public static void SwapSpatialParent(Spatial child, Spatial newParent)
	{
		Transform t = child.GlobalTransform;
		child.GetParent().RemoveChild(child);
		newParent.AddChild(child);
		child.GlobalTransform = t;
	}

	public static float Distance(Vector3 a, Vector3 b)
	{
		return (b - a).Length();
	}
	
	/// <summary>
	/// Calculate pitch and yaw angles based on a flat x/z plane.
	/// </summary>
	/// <param name="v"></param>
	/// <returns></returns>
	public static Vector3 CalcEulerDegrees(Vector3 v)
	{
		// yaw
		float yawRadians = (float)Math.Atan2(-v.x, -v.z);
		// pitch
		Vector3 flat = new Vector3(v.x, 0, v.z);
		float flatMag = flat.Length();
		float pitchRadians = (float)Math.Atan2(v.y, flatMag);
		return new Vector3(
			Mathf.Rad2Deg(pitchRadians),
			Mathf.Rad2Deg(yawRadians),
			0);
	}

	public static float FlatYawDegreesBetween(Vector3 origin, Vector3 target)
	{
		// I always screw this stuff up or get it in the wrong order :(
		float dx = target.x - origin.x;
		float dz = target.z - origin.z;
		return Mathf.Rad2Deg(Mathf.Atan2(dx, dz));
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

	public static void FillSpreadAngles(
		Transform tran,
		List<Vector3> results,
		float spreadH,
		float spreadV)
	{
		if (results == null) { return; }
		// Check capacity
		int len = results.Count;
		if (len == 0) { return; }

		Vector3 origin = tran.origin;
		Vector3 forward = -tran.basis.z;
		Vector3 up = tran.basis.y;
		Vector3 right = tran.basis.x;
		
		results[0] = forward;
		for (int i = 1; i < len; ++i)
		{
			float rSpreadH = RandomRange(-spreadH, spreadH);
			float rSpreadV = RandomRange(-spreadV, spreadV);
			Vector3 end = VectorMA(origin, 8192, forward);
			end = VectorMA(end, rSpreadH, right);
			end = VectorMA(end, rSpreadV, up);
			results[i] = (end - origin).Normalized();
		}
	}
}
