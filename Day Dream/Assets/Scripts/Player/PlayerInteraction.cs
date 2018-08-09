using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour {

	private bool isCarrying = false;
	public bool IsCarrying{
		get{return isCarrying;}
	}

	bool puttingDown = false;

	private string[] tags = new string[]{"Item", "PickUp"};
	private Item targetItem;

	[SerializeField]
	private GameObject targetPickUp;

	PlayerInventory pInv;
	PlayerController pController;

	void Start () {
		pInv = GetComponent<PlayerInventory>();
		pController = GetComponent<PlayerController>();
	}
	
	void Update () {
		Tick();
	}

	void Tick(){
		if(isCarrying){
			if(Input.GetButtonDown("Jump")){
				DropPickUp();
			}

			Vector3 targetPos = transform.position;
			targetPos.y += 2.5f;

			Vector3 tp = Vector3.Lerp(targetPickUp.transform.position, targetPos, 0.4f);

			targetPickUp.transform.position = tp;
			targetPickUp.transform.rotation = transform.rotation;
		}
	}

	public void InitPickUp(){
		if(!puttingDown){
			if(isCarrying){
				DropPickUp(true);
				return;
			}

			if(targetItem){
				PickUpItem();
				isCarrying = false;
			}else if(targetPickUp && pController.CheckGrounded()){
				isCarrying = true;
				targetPickUp.GetComponent<Rigidbody>().isKinematic = true;

				if(pInv.Equipped)
					pInv.EquipWeapons();
			}
		}
	}



	void PickUpItem(){
		pInv.AddItem(targetItem);
		targetItem = null;
	}

	void DropPickUp(bool fireCR = false){
		isCarrying = false;

		if(fireCR){
			StartCoroutine(PutDown());
			return;
		}

		targetPickUp.GetComponent<Rigidbody>().isKinematic = false;
	}

	IEnumerator PutDown(){
		float _time = Time.time;
		Vector3 tp = transform.position + (transform.forward * 1.5f);
		GameObject targetObject = targetPickUp;

		puttingDown = true;

		while(Time.time - _time < 1){
			targetObject.transform.position = Vector3.Lerp(targetObject.transform.position, tp, 0.4f);
			yield return new WaitForEndOfFrame();
		}
		puttingDown = false;
		targetObject.GetComponent<Rigidbody>().isKinematic = false;
		
		yield return null;
	}

	private void OnTriggerStay(Collider other){
		if(other.transform.tag == tags[0]){
			targetItem = other.transform.GetComponent<Item>();
		}else if(other.transform.tag == tags[1]){
			targetPickUp = other.transform.gameObject;
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.transform.tag == tags[0]){
			targetItem = null;
		}else if(other.transform.tag == tags[1]){
			DropPickUp();
			targetPickUp = null;
		}
	}
}
