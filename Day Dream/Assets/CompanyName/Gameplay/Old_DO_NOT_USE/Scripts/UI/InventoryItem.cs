using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {
	public Image icon;
	public string _name;
	public Item item;

	PlayerInventory pInv;

	void Start(){
		pInv = PlayerManager.instance.GetComponent<PlayerInventory>();
	}

	public void SelectItem(){
		pInv.ShowItemInfo(item);
	}
}
