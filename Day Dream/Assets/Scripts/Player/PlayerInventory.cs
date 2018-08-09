using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour {
	//this class will have save and load data
	private PlayerMenu pMenu;
	private Animator anim;

	public float coinCount = 0; 

	public List <GameObject> renderedInventoryList = new List <GameObject>();
	public List <Item> fullInventory = new List <Item>();

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

	[SerializeField]
	private Item currentWeapon = null;
	public Item CurrentWeapon{
		get{return currentWeapon;}
	}

	[SerializeField]
	private Item currentShield = null;
	public Item CurrentShield{
		get{return currentShield;}
	}

	private Item currentDetailedItem = null; //this is the item that the player is currently viewing in the detail panel

	[SerializeField] private GameObject UIitemPrefab;
	[SerializeField] private Transform inventoryParent; 

	//Menu gameobjects
	[SerializeField]
	GameObject inventoryScreen;

#region UIInformation
	//Item Description Tab/////////
	[SerializeField]
	TextMeshProUGUI d_type;

	[SerializeField]
	Image d_icon;

	[SerializeField]
	TextMeshProUGUI d_name;

	[SerializeField]
	TextMeshProUGUI d_damage;

	[SerializeField]
	TextMeshProUGUI d_damageType;

	[SerializeField]
	Button d_discardButton;

//Equipped Info Tab
	//melee
	[SerializeField]
	TextMeshProUGUI e_m_name;

	[SerializeField]
	TextMeshProUGUI e_m_damage;

	[SerializeField]
	TextMeshProUGUI e_m_damageType;

	[SerializeField]
	Image e_m_icon;

	//Ranged
	[SerializeField]
	TextMeshProUGUI e_r_name;

	[SerializeField]
	TextMeshProUGUI e_r_damage;

	[SerializeField]
	TextMeshProUGUI e_r_damageType;

	[SerializeField]
	Image e_r_icon;

	//Shield
	[SerializeField]
	TextMeshProUGUI e_s_name;

	[SerializeField]
	TextMeshProUGUI e_s_damage;

	[SerializeField]
	TextMeshProUGUI e_s_damageType;

	[SerializeField]
	Image e_s_icon;

	
	[SerializeField] 
	TextMeshProUGUI UIcoinCount;

	[SerializeField]
	TextMeshProUGUI UIDungeonKeyCount;

#endregion

	void Start(){
		anim = GetComponent<Animator>();
		pMenu = GetComponent<PlayerMenu>();
	
		currentDetailedItem = fullInventory[0];

		if(!currentShield){
			currentShield = fullInventory[1];
			ShowEquippedItems();
		}

		if(!currentWeapon){
			currentWeapon = fullInventory[0];
			ShowEquippedItems();
		}

	}

	public void OpenCloseInventory(){
		inventoryScreen.SetActive(!inventoryScreen.activeInHierarchy);
		if(!inventoryScreen.activeInHierarchy){
			ClearList();
			isOpen = false;
			
		}else{
			RenderList();
			pMenu.ClosePlayerMenu();
			isOpen = true;
		}
	}	

	public void AddItem(Item item){
		Item[] searchList;
		if(item.itemType == Item.ItemType.Weapon){
			if(item.weaponType == Item.WeaponType.Shield){
				searchList = playerShields;
			}else{
				searchList = playerWeapons;
			}

			foreach(Item playerItem in searchList){
				if(playerItem._name == item._name){
					fullInventory.Add(playerItem);
					break;
				}
			}

		}else{
			//its a consumable
		}

		BoxCollider box = item.GetComponent<BoxCollider>();
		SphereCollider sphere = item.GetComponent<SphereCollider>();

		if(box){
			box.enabled = false;
		}

		if(sphere){
			sphere.enabled = false;
		}
		
		item.gameObject.SetActive(false);
	}

	public void RemoveItem(){
		if(currentDetailedItem && currentWeapon != currentDetailedItem && currentShield != currentDetailedItem){
			ClearList();
			fullInventory.Remove(currentDetailedItem);
			RenderList();
			ShowItemInfo(fullInventory[0]);
		}
	}

	public void ShowItemInfo(Item item){
		d_icon.sprite = item.icon;
		d_type.text = item.itemType.ToString();
 		d_name.text = item._name;
		d_damage.text = item.damage.ToString();
		d_damageType.text = item.damageType.ToString();

		currentDetailedItem = item;
		
		if(currentDetailedItem._name == "Default Sword" || currentDetailedItem._name == "Default Shield"){
			d_discardButton.gameObject.SetActive(false);
		}else{
			d_discardButton.gameObject.SetActive(true);
		}
	}

	public void ClearList(){
		foreach(GameObject UIitem in renderedInventoryList){
			Destroy(UIitem);
		}
		renderedInventoryList.Clear();
	}

	public void RenderList(){
		foreach(Item item in fullInventory){
			Item info = item.GetComponent<Item>();
			
			GameObject UInewItem = Instantiate(UIitemPrefab, transform.position, Quaternion.identity);
			UInewItem.transform.SetParent(inventoryParent);
			renderedInventoryList.Add(UInewItem);//gives the clear list function access to the rendered ui
			
			//set all the images and stuff
			InventoryItem UInewItemInfo = UInewItem.GetComponent<InventoryItem>();
			if(info.icon){
				UInewItem.GetComponent<Image>().sprite = info.icon;
			}

			UInewItemInfo.item = item;
			UInewItemInfo._name = info._name;
		}

		ShowItemInfo(fullInventory[0]);
		LoadLevelInformation();
	}

	public void LoadLevelInformation(){
		int dungeonKeyCount = GameManager.instance.CurrentLevel.FoundKeys.Count;
		int dungeonKeyMax = GameManager.instance.CurrentLevel.keys.Length;

		UIcoinCount.text = coinCount.ToString();
		UIDungeonKeyCount.text = dungeonKeyCount + "/" + dungeonKeyMax;
	}

	public void SetWeaponAsEquipped(){
		if(!currentDetailedItem){
			Debug.Log("no item has been selected");
			return;
		}

		//find weapon in playerWeapons
		Item weaponToUnequip;
		Item[] listToSearch;
		if(currentDetailedItem.weaponType == Item.WeaponType.Shield){
			listToSearch = playerShields;
			weaponToUnequip = currentShield;
		}else{
			listToSearch = playerWeapons;
			weaponToUnequip = currentWeapon;
		}

		foreach(Item pWeapon in listToSearch){
			if(pWeapon == currentDetailedItem){
				if(pWeapon.weaponType == Item.WeaponType.Shield){
					currentShield = currentDetailedItem;
				}else{
					currentWeapon = currentDetailedItem;
				}

				if(equipped){
					currentDetailedItem.gameObject.SetActive(true);
					if(currentDetailedItem != weaponToUnequip){
						weaponToUnequip.gameObject.SetActive(false);
					}
				}

				//show the item info in the equipped panel
				ShowEquippedItems();

				return;
			}
		}

		Debug.LogWarning("no weapon was found");
	}

	private void ShowEquippedItems(){
		if(currentWeapon){
			e_m_name.text = currentWeapon._name;
			e_m_damage.text = currentWeapon.damage.ToString();
			e_m_damageType.text = currentWeapon.damageType.ToString();
			e_m_icon.sprite = currentWeapon.icon;
		}

		if(currentShield){
			e_m_name.text = currentShield._name;
			e_m_damage.text = currentShield.defence.ToString();
			e_m_damageType.text = currentShield.damageType.ToString();
			e_s_icon.sprite = currentShield.icon;
		}
	}

	public void EquipWeapons(float equipDelay = 0){
		if(!currentWeapon){
			currentWeapon = playerWeapons[0];
		}
		if(!currentShield){
			currentShield = playerShields[0];
		}
		// anim.SetTrigger("Equip");

		StartCoroutine(Equip(equipDelay));
	}

	IEnumerator Equip(float equipDelay){
		yield return new WaitForSeconds(equipDelay);
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
