using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour {

 	[SerializeField] private GameObject button;
	[SerializeField] private GameObject door;
 	private GroundTrigger trigger;

	[SerializeField] Transform upPos;
	[SerializeField] Transform downPos;

	void Start () {
		trigger = button.GetComponent<GroundTrigger>();
	}
	
	void Update () {
		if(trigger.isActivated){
			door.transform.position = Vector3.Lerp(door.transform.position, upPos.position, .3f);
		}else{
			door.transform.position = Vector3.Lerp(door.transform.position, downPos.position, .3f);
		}
	}
}
