using Godot;
using GodotSharpFps;

public class CmdConsoleUI : Node
{
	private LineEdit _lineEdit;

	public override void _Ready()
	{
		_lineEdit = GetNode<LineEdit>("VBoxContainer/LineEdit");
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustReleased("ui_accept"))
		{
			string text = _lineEdit.Text;
			_lineEdit.Text = string.Empty;
			Main.instance.console.Execute(text);
		}
	}
}
