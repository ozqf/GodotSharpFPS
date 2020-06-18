using Godot;
using GodotSharpFps;

public class CmdConsoleUI : Node
{
	private LineEdit _lineEdit;

	public override void _Ready()
	{
		_lineEdit = GetNode<LineEdit>("VBoxContainer/LineEdit");
	}

	public void On()
	{
		_lineEdit.Visible = true;
		_lineEdit.GrabFocus();
	}

	public void Off()
	{
		_lineEdit.Visible = false;
	}

	public bool CustomProcess(float delta)
	{
		if (Input.IsActionJustReleased("ui_accept"))
		{
			string text = _lineEdit.Text;
			_lineEdit.Text = string.Empty;
			Main.i.console.Execute(text);
			return true;
		}
		return false;
	}
}
