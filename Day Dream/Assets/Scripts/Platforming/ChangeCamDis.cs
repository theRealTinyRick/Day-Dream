using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamDis : MonoBehaviour {

	[SerializeField] float adjustedDistance = 5;
	float distance = 0;

	private void OnTiggerEnter(Collider other){
		if(other.tag ==	"Player"){
			ThirdPersonCamera camScript = Camera.main.gameObject.GetComponent<ThirdPersonCamera>();
			Change(camScript);
		}
	}
	
	private void OnTriggerExit(Collider other){
		if(other.tag ==	 "Player"){
			ThirdPersonCamera camScript = Camera.main.gameObject.GetComponent<ThirdPersonCamera>();
			Revert(camScript);
		}
	}

	private void Change(ThirdPersonCamera camScript){
		distance = camScript.originalCameraDistance;
		camScript.originalCameraDistance = adjustedDistance;
	}

	private void Revert(ThirdPersonCamera camScript){
		camScript.originalCameraDistance = distance;
	}
}
