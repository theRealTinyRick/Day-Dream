using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {
	
	void Update () {
		transform.Rotate(Vector3.up * 5);
	}

	private IEnumerator Collect(){

		Vector3 tp = PlayerManager.instance.transform.position;
		tp.y = tp.y + 1;

		while(Vector3.Distance(transform.position, tp) > .1f){
			transform.position = Vector3.Lerp(transform.position, tp, .3f);
			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0,0,0), .3f);
			yield return new WaitForEndOfFrame();
		}

		PlayerManager.instance.GetComponent<PlayerInventory>().coinCount++;
		Destroy(gameObject);

		yield return null;
	}

	private void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			StartCoroutine(Collect());
		}
	}
}
