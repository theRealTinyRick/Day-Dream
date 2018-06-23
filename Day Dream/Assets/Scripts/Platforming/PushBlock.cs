using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBlock : MonoBehaviour {
	private void OnTriggerEnter(Collider other){
		if(other.tag == "PlayerWeapon"){
			PlayerManager.instance.pushBlock = gameObject;
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "PlayerWeapon"){
			PlayerManager.instance.pushBlock = null;
		}
	}
}
