using Godot;
using System.Collections.Generic;

namespace GodotSharpFps.src
{
	public enum PatternType
	{ None, Cone3DRandom, Cone3D, HorizontalLine, VerticalLine, Line };

	public struct PatternDef
	{
		public PatternType patternType;
		public int count;
		public Vector3 scale;
	}

	public static class SpawnPatterns
	{
		/// <summary>
		/// If returned false, don't continue
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		private static bool PrepareTransformList(
			Transform source, List<Transform> results, int count)
		{
			if (count == 0) { return false; }
			while (results.Count < count)
			{
				results.Add(new Transform());
			}
			if (count == 1)
			{
				results[0] = source;
				return false;
			}
			return true;
		}
		private static void Cone3D(
			Transform source, List<Transform> results, int count, float spreadH, float spreadV)
		{
			if (!PrepareTransformList(source, results, count)) { return; }

			Vector3 origin = source.origin;
			Vector3 forward = -source.basis.z;
			Vector3 up = source.basis.y;
			Vector3 right = source.basis.x;

			float radians = 0;
			float radiansStep = ZqfGodotUtils.TAU / (count - 1);
			for (int i = 0; i < count; ++i)
			{
				float rSpreadH = Mathf.Cos(radians) * spreadH;
				float rSpreadV = Mathf.Sin(radians) * spreadV;
				Vector3 end = ZqfGodotUtils.VectorMA(origin, 8192, forward);
				end = ZqfGodotUtils.VectorMA(end, rSpreadH, right);
				end = ZqfGodotUtils.VectorMA(end, rSpreadV, up);
				results[i] = source.LookingAt(end, Vector3.Up);
				//results[i].SetLookAt(source.origin, end, source.basis.y);
				radians += radiansStep;
			}
		}

		private static void Cone3DRandom(
			Transform source, List<Transform> results, int count, float spreadH, float spreadV)
		{
			if (!PrepareTransformList(source, results, count)) { return; }

			Vector3 origin = source.origin;
			Vector3 forward = -source.basis.z;
			Vector3 up = source.basis.y;
			Vector3 right = source.basis.x;

			// always shoot one dead centre.
			results[0] = source;
			for (int i = 1; i < count; ++i)
			{
				float rSpreadH = ZqfGodotUtils.RandomRange(-spreadH, spreadH);
				float rSpreadV = ZqfGodotUtils.RandomRange(-spreadV, spreadV);
				Vector3 end = ZqfGodotUtils.VectorMA(origin, 8192, forward);
				end = ZqfGodotUtils.VectorMA(end, rSpreadH, right);
				end = ZqfGodotUtils.VectorMA(end, rSpreadV, up);
				results[i] = source.LookingAt(end, Vector3.Up);
				//results[i].SetLookAt(source.origin, end, Vector3.Up);
			}
		}

		private static void HorizontalLine(
			Transform source, List<Transform> results, int count, float length)
		{
			if (!PrepareTransformList(source, results, count)) { return; }

			Vector3 right = source.basis.x;
			float halfLen = length / 2f;

			Vector3 lineStart = (right * -halfLen);
			Vector3 lineEnd = (right * halfLen);

			float lerp = 0;
			float step = 1f / count;
			for (int i = 0; i < count; ++i)
			{
				Vector3 offset = lineStart.LinearInterpolate(lineEnd, lerp);
				Transform t = source;
				t.origin += offset;
				results[i] = t;
				lerp += step;
			}
		}

		private static void VerticalLine(
			Transform source, List<Transform> results, int count, float length)
		{
			if (!PrepareTransformList(source, results, count)) { return; }

			Vector3 up = source.basis.y;
			float halfLen = length / 2f;

			Vector3 lineStart = (up * -halfLen);
			Vector3 lineEnd = (up * halfLen);

			float lerp = 0;
			float step = 1f / count;
			for (int i = 0; i < count; ++i)
			{
				Vector3 offset = lineStart.LinearInterpolate(lineEnd, lerp);
				Transform t = source;
				t.origin += offset;
				results[i] = t;
				lerp += step;
			}
		}

		public static void FillPattern(
			Transform source, PatternDef def, List<Transform> results)
		{
			switch (def.patternType)
			{
				case PatternType.Cone3D:
					Cone3D(source, results, def.count, def.scale.x, def.scale.y);
					break;
				case PatternType.Cone3DRandom:
					Cone3DRandom(source, results, def.count, def.scale.x, def.scale.y);
					break;
				case PatternType.HorizontalLine:
					HorizontalLine(source, results, def.count, def.scale.x);
					break;
				case PatternType.VerticalLine:
					VerticalLine(source, results, def.count, def.scale.y);
					break;
				default:
					Cone3DRandom(source, results, def.count, def.scale.x, def.scale.y);
					break;
			}
		}
	}
}
