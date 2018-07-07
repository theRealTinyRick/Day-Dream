using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	//on level load the current level will be assign to the game manager

	[SerializeField]
	private string levelName = "";
	public string LevelName{
		get{return levelName;}
	}

	[SerializeField]
	GameObject dungeonKeys;

	private List <GameObject> foundKeys = new List<GameObject>();
	public int FoundKeys{	
		get{return foundKeys.Count;}
	}
}

public class LevelData{
	public string name = "";
	public bool complete = false;
	public int [] indexOfFoundKeys;

	public LevelData(string name, bool complete, int[] indexOfFoundKeys){
		this.name = name;
		this.complete = complete;
		this.indexOfFoundKeys = indexOfFoundKeys;
	}

	public LevelData(){

	}
}