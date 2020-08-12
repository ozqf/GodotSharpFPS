using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
	public class ViewModel : Spatial
	{
		private bool _enabled = false;
		
		public void SetEnabled(bool flag)
		{
			Console.WriteLine($"{Name} enabled - {flag}");
			_enabled = flag;
			Visible = _enabled;
		}
	}
}
