using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {
	
	void Update () {
		transform.Rotate(Vector3.up * 5);
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			PlayerManager.instance.coinCount++;
			Destroy(gameObject);
		}
	}
}
