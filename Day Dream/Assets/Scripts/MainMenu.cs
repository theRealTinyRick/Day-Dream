using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	[SerializeField]
	GameObject levelSelectScreen;
	
	public void StartNewGame(){
		GameManager.instance.CreateNewGame();
	}

	public void OpenLevelSelect(){
		levelSelectScreen.SetActive(true);
		levelSelectScreen.GetComponent<LevelSelect>().LoadLevelSelect(GameManager.instance.openLevels);
	}	
}
