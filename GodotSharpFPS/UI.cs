using Godot;
using GodotSharpFps.src;

public class UI : Node
{
    private Label debugtext;
    private Label playerStatus;
    private CmdConsoleUI _console;

    public override void _Ready()
    {
        debugtext = GetNode<Label>("debug_text/Label");
        playerStatus = GetNode<Label>("player_status/Label");
        _console = GetNode<CmdConsoleUI>("main_menu/console");
        Off();
    }

    public void SetDebugtext(string text)
    {
        debugtext.Text = text;
    }

    public void SetHudState(HUDPlayerState state)
    {
        playerStatus.Text = $"Health {state.health}\n{state.weaponName}\nAmmo {state.ammoLoaded}";
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
