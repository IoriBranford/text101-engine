using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData {
	public string firstState;
	public List<State.Data> states;

	// dataResource = Name of a resource file without extension
	// The file must be in Resources folder
	public static GameData Load (string dataResource) {
		var gameDataAsset = Resources.Load(dataResource) as TextAsset;
		if (gameDataAsset == null) {
			MonoBehaviour.print ("Missing GameData resource");
			return null;
		}

		return JsonUtility.FromJson<GameData>(gameDataAsset.text);
	}
}
