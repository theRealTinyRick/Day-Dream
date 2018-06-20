using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
	//this class will have save and load data

	public float coinCount = 0; 

	public List <GameObject> renderedInventoryList;
	public List <GameObject> fullInventory = new List <GameObject>();
	public List <GameObject> weaponInventory = new List <GameObject>();
	public List <GameObject> consumableInventory = new List <GameObject>();

	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private GameObject inventoryParent; 	

	public void AddItem(){

	}

	public void RemoveItem(){

	}

	public void ClearList(){
		foreach(GameObject item in renderedInventoryList){
			Destroy(item);
			renderedInventoryList.Clear();
		}
	}

	public void RenderList(List <GameObject> inventoryToRender){
		foreach(GameObject item in inventoryToRender){
			GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
			newItem.transform.parent = inventoryParent.transform;
			renderedInventoryList.Add(newItem);
			
			//set all the images and stuff
		}
	}
}
