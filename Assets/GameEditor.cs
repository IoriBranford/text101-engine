using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameEditor : EditorWindow {
	[MenuItem ("Window/Text101 Editor")]
	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(GameEditor));
	}

	private GameData _gameData;
	//private Dictionary<string, int> _statesIndex;
	private List<bool> _statesOpen;
	private string _firstState;

	public void LoadData () {
		_gameData = GameData.Load("GameData");

		_firstState = _gameData.firstState;
		//_statesIndex = new Dictionary<string, int> ();
		_statesOpen = new List<bool> (_gameData.states.Count);

		int i = 0;
		foreach (State.Data stateData in _gameData.states) {
			//_statesIndex.Add (stateData.name, i++);
			_statesOpen.Add(false);
		}
	}

	public void SaveData () {
		string gameDataJson = JsonUtility.ToJson(_gameData);
		StreamWriter streamWriter =
			File.CreateText("Assets/Resources/GameData.json");
		streamWriter.WriteLine(gameDataJson);
		streamWriter.Close();
	}

	void OnGUI_State (State.Data stateData) {
		stateData.name = EditorGUILayout.DelayedTextField("Name", stateData.name);
		GUILayout.Label ("Text");
		stateData.text = EditorGUILayout.TextArea(stateData.text, EditorStyles.textArea);

		Rect rect = EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Key");
		GUILayout.Label("Destination");
		EditorGUILayout.EndHorizontal();
		int deleted = -1;
		for (int i = 0; i < stateData.commands.Count; i += 2) {
			string keyCodeStr = stateData.commands [i];
			string nextState = stateData.commands [i + 1];

			EditorGUILayout.BeginHorizontal();
			stateData.commands[i] = EditorGUILayout.DelayedTextField(keyCodeStr);
			stateData.commands[i + 1] = EditorGUILayout.DelayedTextField(nextState);
			if (GUILayout.Button("Del")) {
				deleted = i;
			}
			EditorGUILayout.EndHorizontal();
		}
		if (deleted >= 0) {
			stateData.commands.RemoveRange(deleted, 2);
		}

		if (GUILayout.Button("Add New Command")) {
			stateData.commands.AddRange(new string[] {"", ""});
		}
	}

	void OnGUI () {
		if (_gameData == null)
			LoadData();

		titleContent.text = "Text101";

		GUILayout.Label ("General", EditorStyles.boldLabel);
		_firstState = EditorGUILayout.DelayedTextField("First State", _firstState);

		EditorGUILayout.Space();

		GUILayout.Label ("States", EditorStyles.boldLabel);
		int i = 0;
		int deleted = -1;
		foreach (var stateData in _gameData.states) {
			EditorGUILayout.BeginHorizontal();
			_statesOpen[i] = EditorGUILayout.Foldout(
					_statesOpen[i], stateData.name,
					true, EditorStyles.foldout);
			if (GUILayout.Button("Del")) {
				deleted = i;
			}
			EditorGUILayout.EndHorizontal();

			if (_statesOpen[i]) {
				OnGUI_State(stateData);
				EditorGUILayout.Space();
			}

			++i;
		}
		if (deleted >= 0) {
			_gameData.states.RemoveAt(deleted);
		}

		if (GUILayout.Button("Add New State")) {
			_gameData.states.Add(new State.Data());
			_statesOpen.Add(true);
		}

		SaveData();
	}
}
