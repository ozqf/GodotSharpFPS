using Godot;
using System;
using GodotSharpFps.src;
using System.Collections.Generic;
using System.Reflection;

public enum GlobalEventType
{
	None, MapChange, GameStateChange, PlayerSpawned, PlayerDied, LevelComplete
}

public class GlobalEventObserver
{
	public delegate void ObserveEvent(GlobalEventType type, object obj);

	public ObserveEvent callback;
	public object subject;
	public bool clearOnMapChange;
	public string label;
}

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
	public Game game;
	public MobThink mobThink;
	public GameCamera cam;
	public UI ui;

	public bool gameInputActive {  get { return ui.GetGameInputEnabled(); } }

	// Orphan nodes are nodes not currently in the scene tree.
	private List<Node> _orphanNodes = new List<Node>();
	private List<GlobalEventObserver> _observers = new List<GlobalEventObserver>();

	public override void _Ready()
	{
		Console.WriteLine("MAIN INIT");
		_instance = this;
		console = new CmdConsole();
		console.AddCommand("test", "", "Test console", ExecCmdTest);
		console.AddCommand("map", "", "Load a scene from the maps folder, eg 'map test_box'", ExecCmdScene);
		console.AddCommand("quit", "", "Close application", ExecCmdQuit);
		console.AddCommand("exit", "", "Close application", ExecCmdQuit);

		// init services
		factory = new GameFactory(this);
		cam = GetNode<GameCamera>("game_camera");
		ui = GetNode<UI>("/root/ui");
		Input.SetMouseMode(Input.MouseMode.Captured);
		mobThink = new MobThink();

		game = new Game(this, GetNode<Spatial>("game"));

		// test stuff
		ZqfXml.ListAllAssemblyResources(Assembly.GetExecutingAssembly());
		TestReadTextFile();
	}

	public override void _Process(float delta)
	{
		game.Tick();
		ProcessOrphanNodes(delta);
	}

	/*******************************************************/
	#region Global event broadcast
	public void AddObserver(
		GlobalEventObserver.ObserveEvent callback,
		object subject,
		bool clearOnMapChange,
		string label)
	{
		if (callback == null) { throw new ArgumentNullException(nameof(callback)); }
		if (subject == null) { throw new ArgumentNullException(nameof(subject)); }
		if (label == null) { throw new ArgumentNullException(nameof(label)); }

		GlobalEventObserver ob = new GlobalEventObserver();
		ob.callback = callback;
		ob.subject = subject;
		ob.clearOnMapChange = clearOnMapChange;
		ob.label = label;
		_observers.Add(ob);
	}

	public void Broadcast(GlobalEventType type, object obj)
	{
		Console.WriteLine($"Broadcast global event type {type}");
		for (int i = _observers.Count - 1; i >= 0; --i)
		{
			_observers[i].callback(type, obj);
		}
	}

	#endregion

	/*******************************************************/
	#region Orphan Nodes
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

	private void ProcessOrphanNodes(float delta)
	{
		
		for (int i = _orphanNodes.Count - 1; i >= 0; --i)
		{
			IOrphanNodeUpdate updater = _orphanNodes[i] as IOrphanNodeUpdate;
			if (updater != null)
			{
				updater.OrphanNodeUpdate(delta);
			}
		}
	}

	#endregion

	/*******************************************************/
	#region Console commands
	public bool ExecCmdScene(string command, string[] tokens)
	{
		if (tokens.Length != 2)
		{
			Console.WriteLine("No scene name specified");
			return true;
		}

		// example path: "res://maps/test_box.tscn"
		string path = $"res://maps/{tokens[1]}.tscn";
		// Check the scene exists!
		Directory dir = new Directory();
		if (!dir.FileExists(path))
		{
			Console.WriteLine($"No map scene \"{path}\" found");
			return true;
		}
		// Cleanup
		cam.Reset();
		Broadcast(GlobalEventType.MapChange, tokens[1]);
		// Change
		GetTree().ChangeScene(path);
		return true;
	}

	public bool ExecCmdTest(string command, string[] tokens)
	{
		Console.WriteLine($"MAIN Exec cmd test \"{command}\"");
		Console.WriteLine($"\tTokenised: {string.Join(", ", tokens)}");
		return true;
	}

	public bool ExecCmdQuit(string command, string[] tokens)
	{
		GetTree().Quit();
		return true;
	}

	#endregion

	/*******************************************************/
	#region misc crap
	public void SetDebugText(string txt)
	{
		ui.SetDebugtext(txt);
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
	#endregion
}
