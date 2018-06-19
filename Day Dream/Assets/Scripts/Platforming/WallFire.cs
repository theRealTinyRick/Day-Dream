using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFire : MonoBehaviour {

	[SerializeField] float waitTime;
	[SerializeField] float fireTime;
	private ParticleSystem flame;
	private BoxCollider dmgCollider;


	void Start () {
		flame = GetComponentInChildren<ParticleSystem>();
		dmgCollider = GetComponent<BoxCollider>();

		StartCoroutine(FireTimer());
	}
	
	private IEnumerator FireTimer(){	
		yield return new WaitForSeconds(waitTime);
		flame.Play();
		dmgCollider.enabled = true;
		yield return new WaitForSeconds(fireTime);
		flame.Stop();
		dmgCollider.enabled = false;
		StartCoroutine(FireTimer());

		yield return null;
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			Debug.Log("Deal Fire Damage");
		}
	}
}
