using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTrigger : MonoBehaviour {

	public bool isActivated = false;
 
	[SerializeField] private GameObject button;
	[SerializeField] private Transform upPos;
	[SerializeField] private Transform downPos;
	
	void Update () {
		if(isActivated){
			button.transform.position = Vector3.Lerp(button.transform.position, downPos.position, 0.3f);
		}else{
			button.transform.position = Vector3.Lerp(button.transform.position, upPos.position, 0.3f);
		}	
	}

	private void OnTriggerStay(Collider other){
		if(other.tag == "PickUp" || other.tag == "Player"){
			isActivated = true;
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "PickUp" || other.tag == "Player"){
			isActivated = false;
		}
	}
}
