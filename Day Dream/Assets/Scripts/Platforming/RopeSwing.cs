using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSwing : MonoBehaviour {

	[SerializeField] GameObject swingPoint;
	[SerializeField] Transform pointA;
	[SerializeField] Transform pointB;

	void Update () {
		if(Input.GetKeyDown(KeyCode.F)){
			StartCoroutine(Swing());
		}
	}

	private IEnumerator Swing(){
		Vector3 EndPoint;
		if(Vector3.Distance(PlayerManager.instance.transform.position, pointA.position) < Vector3.Distance(PlayerManager.instance.transform.position, pointB.position)){
			EndPoint = pointB.position;
		}else{
			EndPoint = pointA.position;
		}

		PlayerManager.instance.transform.SetParent(swingPoint.transform);
		PlayerManager.instance.GetComponent<Rigidbody>().isKinematic = true;
		while(Vector3.Distance(EndPoint, PlayerManager.instance.transform.position) > .1f){
			PlayerManager.instance.transform.position = Vector3.Lerp(PlayerManager.instance.transform.position, EndPoint, .05f);
			PlayerManager.instance.transform.LookAt(EndPoint);
			yield return new WaitForEndOfFrame();
			
		}

		PlayerManager.instance.GetComponent<Rigidbody>().isKinematic = false;
		PlayerManager.instance.transform.SetParent(null);

		yield return null;
	}
}
