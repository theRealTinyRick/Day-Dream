using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
	//this class will have save and load data

	public float coinCount = 0; 

	public List <GameObject> renderedInventoryList = new List <GameObject>();
	public List <GameObject> fullInventory = new List <GameObject>();
	public List <GameObject> weaponInventory = new List <GameObject>();
	public List <GameObject> consumableInventory = new List <GameObject>();

	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private Transform inventoryParent; 	

	public void AddItem(GameObject item){
		fullInventory.Add(item);
		item.GetComponent<BoxCollider>().enabled = false;
		item.GetComponent<SphereCollider>().enabled = false;
		PlayerManager.instance.item = null;
		item.SetActive(false);
	}

	public void RemoveItem(){

	}

	public void ClearList(){
		for(int i = 0; i < fullInventory.Count; i++){
			Destroy(renderedInventoryList[i]);
		}
		renderedInventoryList.Clear();
	}

	public void RenderList(List <GameObject> inventoryToRender){
		foreach(GameObject item in inventoryToRender){
			GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
			newItem.transform.SetParent(inventoryParent);
			renderedInventoryList.Add(newItem);
			
			//set all the images and stuff
			Item info = item.GetComponent<Item>();
			InventoryItem newItemInfo = newItem.GetComponent<InventoryItem>();
			if(info.icon)
				newItem.GetComponent<Image>().sprite = info.icon;
			newItemInfo.item = item;
			newItemInfo._name = info.name;
		}
	}
}
