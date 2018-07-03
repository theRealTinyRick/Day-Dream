using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvRotate : MonoBehaviour {

	[SerializeField] private Vector3 axis;
	[SerializeField] private float speed;

	void Update () {
		transform.Rotate(axis * speed * Time.deltaTime);
	}
}
