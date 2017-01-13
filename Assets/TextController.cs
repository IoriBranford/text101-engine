using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextController : MonoBehaviour {
	private Dictionary<string, State> _states;
	private State _state;

	public Text text;

	void Start () {
		var gameData = GameData.Load("GameData");

		_states = new Dictionary<string, State> ();

		foreach (State.Data stateData in gameData.states) {
			try {
				_states.Add (stateData.name, new State (stateData));
			} catch (Exception exception) {
				print (exception);
			}
		}

		try {
			_state = _states[gameData.firstState];
			text.text = _state.text;
		} catch (KeyNotFoundException) {
			print ("Missing state: " + gameData.firstState);
		}
	}

	void Update () {
		foreach (var command in _state.commands) {
			bool keyPressed = (command.Key == KeyCode.None) ?
				Input.anyKeyDown : Input.GetKeyDown(command.Key);

			if (keyPressed) {
				try {
					_state = _states[command.Value];
					text.text = _state.text;
				} catch (KeyNotFoundException) {
					print ("Missing state: " + command.Value);
				}
				break;
			}

		}
	}
}
