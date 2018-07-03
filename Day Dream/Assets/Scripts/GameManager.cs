using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	public static GameObject Player;

	void Start () {
		if(!instance){
			instance = this;
		}else{
			Destroy(gameObject);
		}

	}
	
	public void SaveGame(){

	}

	public void LoadGame(){

	}
}
