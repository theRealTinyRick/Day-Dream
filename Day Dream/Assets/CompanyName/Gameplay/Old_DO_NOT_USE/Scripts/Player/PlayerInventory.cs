using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;

public class PlayerInventory : MonoBehaviour {

	#region UIInformation
	//Item Description Tab/////////
	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI d_type;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private Image d_icon;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI d_name;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI d_damage;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI d_damageType;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private Button d_discardButton;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_m_name;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_m_damage;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_m_damageType;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private Image e_m_icon;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_r_name;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_r_damage;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_r_damageType;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private Image e_r_icon;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_s_name;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_s_damage;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI e_s_damageType;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private Image e_s_icon;

	[TabGroup("UI SetUp")]
	[SerializeField] 
	private TextMeshProUGUI UIcoinCount;

	[TabGroup("UI SetUp")]
	[SerializeField]
	private TextMeshProUGUI UIDungeonKeyCount;

#endregion
	
	[TabGroup("Set Up")]
	[SerializeField] 
	private Item[] playerWeapons;
	
	[TabGroup("Set Up")]
	[SerializeField]
	private Item[] playerShields;

	[TabGroup("Set Up")]
	[SerializeField] 
	private GameObject UIitemPrefab;

	[TabGroup("Set Up")]
	[SerializeField] 
	private Transform inventoryParent; 

	[TabGroup("Set Up")]
	[SerializeField]
	private GameObject inventoryScreen;

	[TabGroup("State Information")]
	[SerializeField]
	private Item currentWeapon = null;
	public Item CurrentWeapon{ get{return currentWeapon;} }

	[TabGroup("State Information")]
	[SerializeField]
	private Item currentShield = null;
	public Item CurrentShield{ get{return currentShield;} }

	[TabGroup("State Information")]
	[SerializeField]
	public float coinCount = 0; 

	[TabGroup("Inventory")]
	[SerializeField]
	public List <GameObject> renderedInventoryList = new List <GameObject>();
	
	[TabGroup("Inventory")]
	[SerializeField]
	public List <Item> fullInventory = new List <Item>();

	private bool isOpen = false;
	public bool IsOpen{ get{return isOpen;} }

	private bool equipped = false;
	public bool Equipped{ get{return equipped;} }

	private Item currentDetailedItem = null; 
	private PlayerMenu pMenu;
	private Animator anim;

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
