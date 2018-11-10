using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelect : MonoBehaviour {

	[SerializeField]
	GameObject UILevelSelectItem;

	[SerializeField] 
	Transform TableOfContents;

	[SerializeField] 
	TextMeshProUGUI levelTitle;

	public int currentShownLevelIndex = 0;

	public void LoadLevelSelect(List <Level> completeLevels){
		foreach(Level level in completeLevels){
			GameObject levelSelectItem = Instantiate(UILevelSelectItem, Vector3.zero, Quaternion.identity);
			levelSelectItem.transform.SetParent(TableOfContents);
			
			levelSelectItem.GetComponent<LevelSelectItem>().level = completeLevels.IndexOf(level);
			levelSelectItem.GetComponent<LevelSelectItem>().levelSelect = this;
			levelSelectItem.GetComponentInChildren<TextMeshProUGUI>().text = level.name;
		}

		ShowLevelDetails(0);
	}

	public void ShowLevelDetails(int levelIndex){
		currentShownLevelIndex = levelIndex;
		levelTitle.text = GameManager.instance.gameLevels[levelIndex].LevelName;
	}

	public void StartLevel(){
		GameManager.instance.LoadLevel(GameManager.instance.gameLevels[currentShownLevelIndex]);
	}

	public void ReturnToMainMenu(){

	}

	public void ClearLevelList(){

	}
}
