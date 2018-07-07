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
		savePath  = Application.persistentDataPath + "playerData.dat";
		openLevels.Add(gameLevels[0]);
		Debug.Log(savePath);
	}

	void Update(){
		// if(Input.GetKeyDown(KeyCode.Space)){
		// 	SaveGame();
		// }
	}

	public void CreateNewGame(){
		currentLevel = gameLevels[0];
		UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevel.LevelName);
		SaveGame();
	}

	public void LoadLevel(Level level){
		currentLevel = level;
		UnityEngine.SceneManagement.SceneManager.LoadScene(currentLevel.LevelName);
	}
	
	public void SaveGame(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(savePath);

		LevelData[] levelData = new LevelData[openLevels
		.Count];
		for(int i = 0; i < openLevels.Count; i++){
			levelData[i].name = openLevels
			[i].LevelName;
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

			//load all game data from here
			foreach(LevelData levelData in data.openLevels){
				for(int i = 0; i < gameLevels.Length; i ++){
					if(gameLevels[i].name == levelData.name){
						if(!openLevels
						.Contains(gameLevels[i])){
							openLevels
							.Add(gameLevels[i]);
						}
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
