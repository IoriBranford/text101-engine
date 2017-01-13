using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameEditor : EditorWindow {
	[MenuItem ("Window/Text101 Editor")]
	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(GameEditor));
	}

	private Dictionary<string, State> _states;
	private Dictionary<string, bool> _statesOpen;
	private Dictionary<string, string> _statesNewNames;
	private string _firstState;

	public void LoadData() {
		var gameData = TextController.GameData.Load("GameData");

		_firstState = gameData.firstState;
		_states = new Dictionary<string, State> ();
		_statesOpen = new Dictionary<string, bool> ();
		_statesNewNames = new Dictionary<string, string> ();

		foreach (State.Data stateData in gameData.states) {
			try {
				_states.Add (stateData.name, new State (stateData));
				_statesOpen.Add (stateData.name, false);
			} catch (Exception exception) {
				MonoBehaviour.print (exception);
			}
		}
	}

	void OnGUI () {
		if (_states == null)
			LoadData();

		titleContent.text = "Text101";

		GUILayout.Label ("General", EditorStyles.boldLabel);
		_firstState = EditorGUILayout.TextField("First State", _firstState);
		GUILayout.Label ("States", EditorStyles.boldLabel);
		foreach (var namestate in _states) {
			string name = namestate.Key;
			State state = namestate.Value;

			_statesOpen[name] = EditorGUILayout.Foldout(_statesOpen[name], name, true);
			if (!_statesOpen[name])
				continue;

			string newName = EditorGUILayout.DelayedTextField("Name", state.name);
			GUILayout.Label ("Text");
			state.text = EditorGUILayout.TextArea(state.text, EditorStyles.textArea);

			Rect rect = EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Key");
			GUILayout.Label("Destination");
			EditorGUILayout.EndHorizontal();
			foreach (var command in state.commands) {
				rect = EditorGUILayout.BeginHorizontal();
				EditorGUILayout.TextField(command.Key.ToString());
				EditorGUILayout.TextField(command.Value);
				EditorGUILayout.EndHorizontal();
			}

			if (newName != state.name) {
				_statesNewNames.Add(state.name, newName);
			}
		}

		foreach (var stateNewName in _statesNewNames) {
			State state = _states[stateNewName.Key];
			bool stateOpen = _statesOpen[stateNewName.Key];
			string newName = stateNewName.Value;
			_states.Remove(state.name);
			_states.Add(newName, state);
			_statesOpen.Remove(state.name);
			_statesOpen.Add(newName, stateOpen);
			state.name = newName;
		}

		_statesNewNames.Clear();
	}
}
