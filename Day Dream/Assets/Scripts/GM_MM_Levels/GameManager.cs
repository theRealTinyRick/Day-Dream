using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public static GameObject Player;

	public Level[] gameLevels;

	private Level currentLevel;
	public Level CurrentLevel{
		get{return currentLevel;}
	}

	public List <Level> openLevels= new List<Level>();

	private string savePath;

	void Awake(){
		if(!instance){
			instance = this;
		}else{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		currentLevel = gameLevels[0];
		savePath  = Application.persistentDataPath + "/playerData.dat";
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Space)){
			SaveGame();
		}
	}

	public void CreateNewGame(){
		currentLevel = gameLevels[0];
		UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevel.LevelName);
		openLevels.Add(currentLevel);
		SaveGame();
	}

	public void LoadLevel(Level level){
		currentLevel = level;
		UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevel.LevelName);
	}

	public void LoadMainMenu(){
		SaveGame();
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}
	
	public void SaveGame(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(savePath);

		LevelData[] levelData = new LevelData[openLevels.Count];
		for(int i = 0; i < openLevels.Count; i++){
			//get and array of indexs off of the level for each found key. 
			int [] keyIndexs = new int[openLevels[i].FoundKeys.Count];
			for(int j = 0; j < keyIndexs.Length; j++){
				keyIndexs[j] = Array.IndexOf(openLevels[i].keys, openLevels[i].FoundKeys[j]); 
			}
			levelData[i] = new LevelData(openLevels[i].LevelName, openLevels[i].DungeonOpen, keyIndexs);
		}

		PlayerData data = new PlayerData(levelData);
		bf.Serialize(file,data);
		file.Close();
	}

	public void LoadGame(){
		if(File.Exists(savePath)){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(savePath, FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);

			foreach(LevelData levelData in data.openLevels){
				for(int i = 0; i < gameLevels.Length; i ++){
					if(gameLevels[i].LevelName == levelData.name){
						if(!openLevels.Contains(gameLevels[i])){
							openLevels.Add(gameLevels[i]);
						}
						gameLevels[i].LevelSetUp(levelData.indexOfFoundKeys);
					}
				}
			}
			
			file.Close();
		}
	}	
}

[Serializable]
public class PlayerData{
	public string playerName = "Max";
	public int playerLevel = 1;
	public LevelData[] openLevels;

	public PlayerData(LevelData[] openLevels){
		this.openLevels = openLevels;
	}
}
