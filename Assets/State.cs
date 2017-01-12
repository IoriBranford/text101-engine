using System;
using System.Collections.Generic;
using UnityEngine;

public class State {

	[Serializable]
	public class Data {
		public string name;
		public string text;
		public string[] commands;
		// An Array because Dictionary is not serializable
		// Every 2 elements is a key/nextState pair
		// e.g. ["S", "Sheets", "M", "Mirror", "D", "Door"]
	}

	public string name { get; }
	public string text { get; }
	public Dictionary<KeyCode, string> commands { get; }

	public State (Data data) {
		name = data.name;
		text = data.text;
		commands = new Dictionary<KeyCode, string> ();

		for (int i = 0; i < data.commands.Length; i += 2) {
			string keyName = data.commands [i];
			string nextStateName = data.commands [i + 1];

			var keyCode = (KeyCode) Enum.Parse (typeof(KeyCode), keyName);
			commands.Add (keyCode, nextStateName);
		}
	}
}
