using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotSharpFps.src
{
	public delegate bool ExecConsoleCmd(string command, string[] tokens);

	internal class CmdConsoleObserver
	{
		public string name = "";
		public string signature = "";
		public string helpText = "";
		public ExecConsoleCmd callback;
	}

	public class CmdConsole
	{
		private List<CmdConsoleObserver> _observers = new List<CmdConsoleObserver>();

		public CmdConsole()
		{
			AddCommand("help", "", "List all registered commands", ExecHelp);
		}

		private bool ExecHelp(string command, string[] tokens)
		{
			Console.WriteLine($"=== Cmd List ===");
			foreach(var ob in _observers)
			{
				Console.WriteLine($"{ob.name}: {ob.helpText}");
			}
			return true;
		}

		public void AddCommand(string name, string signature, string helpText, ExecConsoleCmd callback)
		{
			if (_observers.Any(x => x.name == name))
			{ throw new ArgumentException($"Command name {name} already taken!"); }
			_observers.Add(new CmdConsoleObserver
			{
				name = name,
				signature = signature,
				helpText = helpText,
				callback = callback
			});
		}

		public void Execute(string command)
		{
			command = command.Trim();
			if (string.IsNullOrWhiteSpace(command))
			{
				Console.WriteLine($"Empty cmd");
				return;
			}
			string[] tokens = command.Split(' ');
			if (tokens.Length == 0) { return; }
			int l = _observers.Count;
			for (int i = 0; i < l; ++i)
			{
				if (_observers[i].name == tokens[0])
				{
					_observers[i].callback(command, tokens);
				}
			}
		}
	}
}
