using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
	//this class will have save and load data

	public float coinCount = 0; 

	public List <GameObject> renderedInventoryList = new List <GameObject>();
	public List <Item> fullInventory = new List <Item>();
	public GameObject[] allWeapons;
	public List <GameObject> consumableInventory = new List <GameObject>();

	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private Transform inventoryParent; 	

	public void AddItem(Item item){
		fullInventory.Add(item);
		item.GetComponent<BoxCollider>().enabled = false;
		item.GetComponent<SphereCollider>().enabled = false;
		Item itemInfo = item.GetComponent<Item>();
		if(itemInfo.itemType == Item.ItemType.Weapon){
			//enable the use of the item
		}
		item.gameObject.SetActive(false);
		PlayerManager.instance.item = null;
	}

	public void RemoveItem(){

	}

	public void ClearList(){
		for(int i = 0; i < fullInventory.Count; i++){
			Destroy(renderedInventoryList[i]);
		}
		renderedInventoryList.Clear();
	}

	public void RenderList(){
		foreach(Item item in fullInventory){
			GameObject newItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
			newItem.transform.SetParent(inventoryParent);
			renderedInventoryList.Add(newItem);//gives the clear list function access to the rendered ui
			
			//set all the images and stuff
			Item info = item.GetComponent<Item>();
			InventoryItem newItemInfo = newItem.GetComponent<InventoryItem>();
			if(info.icon)
				newItem.GetComponent<Image>().sprite = info.icon;

			newItemInfo.item = item;
			newItemInfo._name = info._name;
		}
	}
}
