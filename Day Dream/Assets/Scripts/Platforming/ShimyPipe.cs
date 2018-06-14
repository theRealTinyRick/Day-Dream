using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShimyPipe : MonoBehaviour {

	public Transform sideA;
	public Transform sideB;

	private void OnTriggerStay(Collider other){
        if(other.tag == "Player"){
            PlayerManager.instance.shimyPipe = gameObject;
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "Player"){
            PlayerManager.instance.shimyPipe = null;
        }
    }
}
