using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenu : MonoBehaviour {

	[SerializeField]
	GameObject playerMenu;

	[SerializeField]
	GameObject playerInventory;

	private PlayerManager pManager;
	private PlayerInventory pInventory;

	private bool isOpen = false;
	public bool IsOpen{
		get{return isOpen;}
	}

	void Start(){
		pManager = PlayerManager.instance;
		pInventory = GetComponent<PlayerInventory>();
	}

	void Update(){
		CheckOpenWindows();
	}

	public void CheckOpenWindows(){
		//this function will check of there are windows open and set pause and cursor acordingly
		if(isOpen || pInventory.IsOpen){
			pManager.Pause(true);
		}else{
			pManager.Pause(false);
		}
	}

	public void OpenClosePlayerMenu(){
        playerMenu.SetActive(!playerMenu.activeInHierarchy);
        if(playerMenu.activeInHierarchy){
			isOpen = true;
        }else{
			isOpen = false;        
		}
    }

	public void ClosePlayerMenu(){
		playerMenu.SetActive(false);
		isOpen = false;
	}

	public void CloseAllWindows(){
		//escape key or a button in the UI
		//use this function to resume close all windows
		if(pManager.IsPaused){
			OpenClosePlayerMenu();
		}

		if(pInventory.IsOpen){
			pInventory.OpenCloseInventory();
		}
	}

	public void Back(){
		if(pInventory.IsOpen){
			pInventory.OpenCloseInventory();
		}else if(playerMenu.activeInHierarchy){
			OpenClosePlayerMenu();
		}
	}

	public void QuickSave(){
        GameManager.instance.SaveGame();
    }

    public void ReturnToMainMenu(){
        GameManager.instance.LoadMainMenu();
    }
}
