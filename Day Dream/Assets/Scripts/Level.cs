using System;
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

	private bool dungeonOpen = false;
	public bool DungeonOpen{
		get{return dungeonOpen;}
	}

	public GameObject[] keys;

	private List <GameObject> foundKeys = new List<GameObject>();
	public List <GameObject> FoundKeys{	
		get{return foundKeys;}
	}

	private int NumberOfKeysNeeded = 0;

	public void LevelSetUp(int[] indexs){
		//deactivate the keys that have already been found
		//call this function from the load function
		foreach(int index in indexs){
			keys[index].SetActive(false);
			foundKeys.Add(keys[index]);
		}
	}

	public void CheckDungeonKeys(){
		if(FoundKeys.Count >= NumberOfKeysNeeded){
			dungeonOpen = true;
		}
	}

	public void PickUpKey(GameObject key){
		foundKeys.Add(key);
		key.GetComponent<SphereCollider>().enabled = false;
		key.GetComponent<MeshRenderer>().enabled = false;
		key.GetComponentInChildren<ParticleSystem>().Stop();	

		CheckDungeonKeys();	
	}
}

[Serializable]
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