using Godot;
using System;
using GodotSharpFps.src;
using System.Collections.Generic;
using System.Reflection;

public class Main : Spatial
{
	///////////////////////////////////////
	// Static
	///////////////////////////////////////
	private static Main _instance = null;
	public static Main i { get { return _instance; } }
	
	///////////////////////////////////////
	// Instance
	///////////////////////////////////////
	
	// Services
	public CmdConsole console;
	public GameFactory factory;
	public GameCamera cam;
	public UI ui;

	public bool _gameInputActive = true;
	public bool gameInputActive {  get { return _gameInputActive; } }

	// Orphan nodes are nodes not currently in the scene tree.
	private List<Node> _orphanNodes = new List<Node>();

	public override void _Ready()
	{
		Console.WriteLine("MAIN INIT");
		_instance = this;
		console = new CmdConsole();
		console.AddObserver("test", "", "Test console", ExecCmdTest);
		console.AddObserver("map", "", "Load a scene from the maps folder, eg 'map test_box'", ExecCmdScene);

		// init services
		factory = new GameFactory(this);
		cam = GetNode<GameCamera>("game_camera");
		ui = GetNode<UI>("/root/ui");
		Input.SetMouseMode(Input.MouseMode.Captured);

		// test stuff
		ZqfXml.ListAllAssemblyResources(Assembly.GetExecutingAssembly());
		TestReadTextFile();
	}

	private void TestReadTextFile()
	{
		// Ready from Godot asset file
		/*
		string path = "res://txt/game_stats.xml";
		File f = new File();
		Error err = f.Open(path, File.ModeFlags.Read);
		if (err != Error.Ok)
		{
			Console.WriteLine($"Error loading text file '{path}'");
			f.Close();
			return;
		}
		string txt = f.GetAsText();
		Console.WriteLine($"Reading {txt.Length} chars from '{path}'");
		ZqfXml.TestReadXml(txt);
		f.Close();
		*/

		// Read from mono embedded resource
		string str = ZqfXml.ReadAssemblyEmbeddedText("GodotSharpFps.txt.game_stats.xml");
		ZqfXml.TestReadXml(str);
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

	private void SwapMap()
	{
		/*
		# Remove the current level
		var level = root.get_node("Level")
		root.remove_child(level)
		level.call_deferred("free")

		# Add the next level
		var next_level_resource = load("res://path/to/scene.tscn)
		var next_level = next_level_resource.instance()
		root.add_child(next_level)
		*/
	}

	public void AddOrphanNode(Node node)
	{
		Console.WriteLine($"Added orphan node {node.Name}");
		_orphanNodes.Add(node);
	}

	public void RemoveOrphanNode(Node node)
	{
		Console.WriteLine($"Remove orphan node {node.Name}");
		_orphanNodes.Remove(node);
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustReleased("ui_cancel"))
		{
			SetGameInputActive(!_gameInputActive);
		}
		for (int i = _orphanNodes.Count - 1; i >= 0; --i)
		{
			IOrphanNodeUpdate updater = _orphanNodes[i] as IOrphanNodeUpdate;
			if (updater != null)
			{
				updater.OrphanNodeUpdate(delta);
			}
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

	/// <summary>
	/// TODO Replace with proper usage of Godot Viewports
	/// </summary>
	/// <returns></returns>
	public Vector2 GetWindowToScreenRatio()
	{
		Vector2 real = OS.GetRealWindowSize();
		Vector2 screen = OS.GetScreenSize();
		return new Vector2(real.x / screen.x, real.y / screen.y);
	}
}
