using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingAxe : MonoBehaviour {

	[SerializeField] private float delayTime;

	void Start () {
		// StartCoroutine(Swing());
	}
	
	void Update(){
		transform.Rotate(transform.forward * 120 * Time.deltaTime, Space.World);
	}

	private IEnumerator Swing(){
		Quaternion rot1 = transform.localRotation;
		Quaternion rot2 = transform.localRotation;
		rot1.z = transform.localRotation.z + 90;
		rot2.z = transform.localRotation.z - 90;

		while(transform.rotation != rot1){
			transform.rotation = Quaternion.Slerp(transform.rotation, rot1, .05f);
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(delayTime);

		while(transform.rotation != rot2){
			transform.rotation = Quaternion.Slerp(transform.rotation, rot2, .05f);
			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}
}
