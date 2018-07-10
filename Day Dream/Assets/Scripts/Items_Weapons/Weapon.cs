using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public enum WeaponType{SingleHand, TwoHandSword, Wand};
	public WeaponType weaponType = WeaponType.SingleHand;
	//default weapon type is short sword

	public string _name;
	public bool aquired = false;

	public float baseDamage = 100;
}
