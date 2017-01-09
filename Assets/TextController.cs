using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextController : MonoBehaviour {

	[Serializable]
	private class StateData {
		public string name;
		public string text;
		public string[] commands;
		// An Array because Dictionary is not serializable
		// Every 2 elements is a key/nextState pair
		// e.g. ["S", "Sheets", "M", "Mirror", "D", "Door"]
	}

	[Serializable]
	private class GameData {
		public string firstState;
		public StateData[] states;
	}

	private class State {
		public string name;
		public string text;
		public Dictionary<KeyCode, string> commands;
	};

	private Dictionary<string, State> _states;
	private State _state;

	public Text text;

	void Start () {
		string gameDataRes = "GameData";
		// Name of a resource file without extension
		// The file must be in Resources folder

		TextAsset gameDataAsset = Resources.Load(gameDataRes) as TextAsset;
		GameData gameData = JsonUtility.FromJson<GameData>(gameDataAsset.text);

		_states = new Dictionary<string, State> ();

		foreach (StateData stateData in gameData.states) {
			State state = new State {
				name = stateData.name,
				text = stateData.text,
				commands = new Dictionary<KeyCode, string> ()
			};

			for (int i = 0; i < stateData.commands.Length; i += 2) {
				string keyName = stateData.commands [i];
				string nextStateName = stateData.commands [i + 1];

				try {
					KeyCode keyCode = (KeyCode) Enum.Parse (typeof(KeyCode), keyName);
					state.commands.Add (keyCode, nextStateName);
				} catch (ArgumentException) {
					print ("Invalid key name " + keyName);
				}
			}

			_states.Add (stateData.name, state);
		}

		try {
			_state = _states[gameData.firstState];
		} catch (KeyNotFoundException) {
			print ("State not found: " + gameData.firstState);
		}
	}

	void Update () {
		if (_state == null)
			return;
		
		if (_state.text != null) {
			text.text = _state.text;
		} else {
			text.text = _state.name + "\n\n";
			foreach (var key in _state.commands) {
				text.text += key.Key + ": " + key.Value;
			}
		}

		foreach (var command in _state.commands) {
			bool keyPressed = (command.Key == KeyCode.None) ?
				Input.anyKeyDown : Input.GetKeyDown(command.Key);

			if (keyPressed) {
				try {
					_state = _states[command.Value];
				} catch (KeyNotFoundException) {
					print ("State not found: " + command.Value);
				}
				break;
			}
		}
	}
}
