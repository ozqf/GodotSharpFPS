using Godot;
using System;

namespace GodotSharpFps.src.nodes
{
	public interface IViewModel
	{
		void SetViewModelState();
	}

	public struct ViewModelState
	{
		public float lerp;
	}

	public class ViewModel : Spatial
	{
		private bool _enabled = false;
		private Spatial _a, _b, _model;
		private float _lerp = 1f;

		public override void _Ready()
		{
			base._Ready();
		}

		public void SetEnabled(bool flag)
		{
			Console.WriteLine($"{Name} enabled - {flag}");
			_enabled = flag;
			Visible = _enabled;
			_a = GetNode<Spatial>("a");
			_b = GetNode<Spatial>("b");
			_model = GetNode<Spatial>("model");
		}

		public void SetViewModelState(ViewModelState state)
		{
			_lerp = state.lerp;
			Transform at = _a.Transform;
			Transform bt = _b.Transform;
			Transform t = at.InterpolateWith(bt, _lerp);
			_model.Transform = t;
		}
	}
}
