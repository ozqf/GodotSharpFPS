using Godot;
using GodotSharpFps.src;
using System;

public class UI : Node
{
    private CanvasLayer _hudContainer;
    private Control _mainMenuContainer;

    private Label debugtext;
    private Label playerStatus;
    private CmdConsoleUI _console;

    private bool _consoleOn = false;
    private bool _mainMenuOn = false;
    private bool _gameMenuOn = false;

    public override void _Ready()
    {
        _hudContainer = GetNode<CanvasLayer>("hud");
        _mainMenuContainer = GetNode<Control>("main_menu_canvas/main_menu");
        debugtext = GetNode<Label>("hud/debug_text");
        playerStatus = GetNode<Label>("hud/player_status");
        _console = GetNode<CmdConsoleUI>("main_menu_canvas/console");
        SetConsoleOn(false);
        SetMainMenuOn(false);

        Button btn = _mainMenuContainer.GetNode<Button>("root_menu/start");
        if (btn == null)
        {
            Console.WriteLine($"Couldn't find button");
        }
        else
        {
            btn.Connect("pressed", this, "OnRootStartClicked");
        }
    }

    public void OnRootStartClicked()
    {
        Console.WriteLine($"Root Start button clicked");
    }

    public void OnOptionsClicked()
    {
        Console.WriteLine($"Options button clicked");
    }

    public void OnQuitClicked()
    {
        Console.WriteLine($"Quit button clicked");
    }

    public void SetDebugtext(string text)
    {
        debugtext.Text = text;
    }

    public void SetHudState(HUDPlayerState state)
    {
        playerStatus.Text = $"Health {state.health}\n{state.weaponName}\nAmmo {state.ammoLoaded}";
    }

    public bool GetGameInputEnabled()
    {
        return !(_mainMenuOn || _gameMenuOn || _consoleOn);
    }

    public void RefreshMouseState()
    {
        if (GetGameInputEnabled())
        {
            Input.SetMouseMode(Input.MouseMode.Captured);
        }
        else
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
        }
    }

    private void SetMainMenuOn(bool flag)
    {
        _mainMenuOn = flag;
        _mainMenuContainer.Visible = flag;
        RefreshMouseState();
    }

    private void SetGameMenuOn(bool flag)
    {
        _gameMenuOn = flag;
        RefreshMouseState();
    }

    private void SetConsoleOn(bool flag)
    {
        _consoleOn = flag;
        if (flag)
        { _console.On(); }
        else { _console.Off(); }
        RefreshMouseState();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("console"))
        {
            SetConsoleOn(!_consoleOn);
        }
        if (Input.IsActionJustReleased("ui_cancel"))
        {
            SetMainMenuOn(!_mainMenuOn);
        }

        if (_console.CustomProcess(delta))
        {
            SetConsoleOn(false);
        }
    }
}
