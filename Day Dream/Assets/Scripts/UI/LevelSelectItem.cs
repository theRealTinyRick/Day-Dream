using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectItem : MonoBehaviour {
	public int level = 0;
	public Sprite icon;
	public LevelSelect levelSelect;

	public void ShowLevelDetails(){
		levelSelect.ShowLevelDetails(level);
	}
}
