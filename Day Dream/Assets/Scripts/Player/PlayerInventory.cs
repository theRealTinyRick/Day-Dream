using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour {
	//this class will have save and load data
	public float coinCount = 0; 

	public List <GameObject> renderedInventoryList = new List <GameObject>();
	public List <Item> fullInventory = new List <Item>();
	public List <GameObject> consumableInventory = new List <GameObject>();

	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private Transform inventoryParent; 

	//Menu gameobjects
	[SerializeField]
	GameObject inventoryScreen;

	private bool isOpen = false;
	public bool IsOpen{
		get{return isOpen;}
	}

	private bool equipped = false;
	public bool Equipped{
		get{return equipped;}
	}

	//this is the array of weapons represented by the items here. 
	//these are present in the hirearchy
	[SerializeField] Item[] playerWeapons;
	[SerializeField] Item[] playerShields;

	private Item currentWeapon = null;
	public Item CurrentWeapon{
		get{return currentWeapon;}
	}

	private Item currentShield = null;
	public Item CurrentShield{
		get{return currentShield;}
	}

	Animator anim;

	void Start(){
		anim = GetComponent<Animator>();
	}

	public void OpenCloseInventory(){
		inventoryScreen.SetActive(!inventoryScreen.activeInHierarchy);
		if(!inventoryScreen.activeInHierarchy){
			ClearList();
			isOpen = false;
		}else{
			RenderList();
			isOpen = true;
		}
	}	

	public void AddItem(Item item){
		fullInventory.Add(item);
		item.GetComponent<BoxCollider>().enabled = false;
		item.GetComponent<SphereCollider>().enabled = false;
		Item itemInfo = item.GetComponent<Item>();
		if(itemInfo.itemType == Item.ItemType.Weapon){
			//enable the use of the item
		}
		item.gameObject.SetActive(false);
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

	public void SetWeaponAsEquipped(){

	}

	public void EquipWeapons(){
		if(!currentWeapon){
			currentWeapon = playerWeapons[0];
		}
		if(!currentShield){
			currentShield = playerShields[0];
		}
		anim.SetTrigger("Equip");

		StartCoroutine(Equip());
	}

	IEnumerator Equip(){
		yield return new WaitForSeconds(0.35f);
		currentWeapon.gameObject.SetActive(!currentWeapon.gameObject.activeInHierarchy);
		//if it is a s handed weapon skip the rest
		currentShield.gameObject.SetActive(!currentShield.gameObject.activeInHierarchy);

		if(currentWeapon.gameObject.activeInHierarchy || currentShield.gameObject.activeInHierarchy){
			equipped = true;
		}else{
			equipped = false;
		}

		yield return null;
	}
}
