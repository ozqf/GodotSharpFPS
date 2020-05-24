using Godot;

public class UI : Node
{
    private Label debugtext;
    private CmdConsoleUI _console;

    public override void _Ready()
    {
        debugtext = GetNode<Label>("debug_text/Label");
        _console = GetNode<CmdConsoleUI>("main_menu/console");
        Off();
    }

    public void SetDebugtext(string text)
    {
        debugtext.Text = text;
    }

    public void On()
    {
        _console.On();
    }

    public void Off()
    {
        _console.Off();
    }

    public override void _Process(float delta)
    {
        if (_console.CustomProcess(delta))
        {
            Off();
        }
    }
}
