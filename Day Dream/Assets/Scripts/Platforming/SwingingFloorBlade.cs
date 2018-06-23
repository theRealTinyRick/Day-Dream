using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingFloorBlade : MonoBehaviour {

	[SerializeField] private float speed = 10;

	public bool isActive = false;

	void Update () {
		if(isActive){
			transform.Rotate(Vector3.up * speed * Time.deltaTime);
		}
	}
}
