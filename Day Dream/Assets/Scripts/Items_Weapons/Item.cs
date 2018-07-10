using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

	public enum ItemType{Consumable, Weapon}
	public ItemType itemType;

	public enum WeaponType{None, SingleHand, DoubleHand, Shield, Staff};
	public WeaponType weaponType = WeaponType.SingleHand;

	public Sprite icon;
	public string _name;
	public string description;

	public int damage;
	public int defence;
}
