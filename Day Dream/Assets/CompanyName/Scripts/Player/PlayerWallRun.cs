using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallRun : MonoBehaviour {

	[SerializeField] 
	private float wallOffset;

	[SerializeField]
	private float searchRange;

	private Transform wallRunHelper;
	private LayerMask layer = 1 << 8;

	private const string wallTag = "WallRun";

	private void Start () {
		wallRunHelper = new GameObject().transform;
		wallRunHelper.name = "Wall Run Helper";

		layer = ~layer;		
	}
	
	private void Update () {
		
	}

	private void InitForWallRun(){
		//if the player is holding shift check both sides of the player for a runnable wall
		Vector3 origin = transform.position;
		origin.y += 1;
		RaycastHit hit;

		if(Physics.Raycast(origin, transform.right, out hit, 1, layer)){
			if(hit.transform.tag == wallTag){
				
				return;	
			}
		}

		if(Physics.Raycast(origin, -transform.right, out hit, 1, layer)){
			if(hit.transform.tag == wallTag){

				return;	
			}
		}

	}

	private void WallRun(){

	}
}
