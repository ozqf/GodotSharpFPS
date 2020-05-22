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

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Console.WriteLine("MAIN INIT");
        _instance = this;
        console = new CmdConsole();
        console.AddObserver("test", "", ExecCmdTest);
        factory = new GameFactory();
    }

    public bool ExecCmdTest(string command, string[] tokens)
    {
        Console.WriteLine($"MAIN Exec cmd test \"{command}\"");
		Console.WriteLine($"\tTokenised: {string.Join(", ", tokens)}");
        return true;
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
