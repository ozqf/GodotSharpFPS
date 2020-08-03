using Godot;
using GodotSharpFps.src;
using System;

public class UI : Node
{
    private Main _main;
    private CanvasLayer _hudContainer;
    private Control _mainMenuContainer;

    private Label debugtext;
    private Label playerStatus;
    private Label _gameMessage;
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
        _gameMessage = GetNode<Label>("hud/gameplay_message");
        _console = GetNode<CmdConsoleUI>("main_menu_canvas/console");
        SetConsoleOn(false);
        SetMainMenuOn(false);
        _main = Main.i;
        _main.AddObserver(OnGlobalEvent, this, false, "UI");

        Button btn = _mainMenuContainer.GetNode<Button>("root_menu/start");
        if (btn == null) { Console.WriteLine($"Couldn't find button"); }
        else { btn.Connect("pressed", this, "OnRootStartClicked"); }
    }

    public void OnGlobalEvent(GlobalEventType type, object obj)
    {
        if (type == GlobalEventType.GameStateChange)
        {
            if (!(obj is Game.State)) { return; }
            Game.State state = (Game.State)obj;

            switch (state)
            {
                case Game.State.Limbo:
                    _gameMessage.Text = string.Empty;
                    playerStatus.Hide();
                    break;
                case Game.State.Pregame:
                    _gameMessage.Text = "Press fire to start";
                    playerStatus.Hide();
                    break;
                case Game.State.Gamplay:
                    _gameMessage.Text = string.Empty;
                    playerStatus.Show();
                    break;
                case Game.State.PostGame:
                    _gameMessage.Text = "Level complete.";
                    playerStatus.Hide();
                    break;
                case Game.State.GameOver:
                    _gameMessage.Text = "Press fire to reload";
                    playerStatus.Hide();
                    break;
            }
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
