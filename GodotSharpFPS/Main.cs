using Godot;
using System;
using GodotSharpFps.src;

public class Main : Spatial
{
    ///////////////////////////////////////
    // Static
    ///////////////////////////////////////
    private static Main _instance = null;
    public static Main instance { get { return _instance; } }
    
    ///////////////////////////////////////
    // Instance
    ///////////////////////////////////////
    public CmdConsole console;
    public GameFactory factory;
    public GameCamera cam;
    public UI ui;

    public bool _gameInputActive = true;
    public bool gameInputActive {  get { return _gameInputActive; } }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Console.WriteLine("MAIN INIT");
        _instance = this;
        console = new CmdConsole();
        console.AddObserver("test", "", "Test console", ExecCmdTest);
        console.AddObserver("map", "", "Load a scene from the maps folder, eg 'map test_box'", ExecCmdScene);
        factory = new GameFactory();
        cam = GetNode<GameCamera>("game_camera");
        ui = GetNode<UI>("/root/ui");
        Input.SetMouseMode(Input.MouseMode.Captured);
    }

    private void SetGameInputActive(bool isActive)
    {
        _gameInputActive = isActive;
        if (isActive)
        {
            Input.SetMouseMode(Input.MouseMode.Captured);
            ui.Off();
        }
        else
        {
            Input.SetMouseMode(Input.MouseMode.Visible);
            ui.On();
        }
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("ui_cancel"))
        {
            SetGameInputActive(!_gameInputActive);
        }
    }

    public bool ExecCmdScene(string command, string[] tokens)
    {
        if (tokens.Length != 2)
        {
            Console.WriteLine("No scene name specified");
            return true;
        }
        // example path: "res://maps/test_box.tscn"
        string path = $"res://maps/{tokens[1]}.tscn";
        GetTree().ChangeScene(path);
        return true;
    }

    public bool ExecCmdTest(string command, string[] tokens)
    {
        Console.WriteLine($"MAIN Exec cmd test \"{command}\"");
		Console.WriteLine($"\tTokenised: {string.Join(", ", tokens)}");
        return true;
    }

    public void SetDebugText(string txt)
    {
        ui.SetDebugtext(txt);
    }

    /*
    func get_window_to_screen_ratio():
    	var real: Vector2 = OS.get_real_window_size()
    	var scr: Vector2 = OS.get_screen_size()
    	var result: Vector2 = Vector2(real.x / scr.x, real.y / scr.y)
    	return result
    */
    public Vector2 GetWindowToScreenRatio()
    {
        Vector2 real = OS.GetRealWindowSize();
        Vector2 screen = OS.GetScreenSize();
        return new Vector2(real.x / screen.x, real.y / screen.y);
    }
}
