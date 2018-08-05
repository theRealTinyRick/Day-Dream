using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbLadder : MonoBehaviour {

	Ladder ladder;
	bool ladderPresent = false;
	bool inPosition = false;

	private bool isClimbing = false;
	public bool IsClimbing{
		get{return isClimbing;}
	}

	float speed = 3;
	float t = 0.0f;

	public bool CheckForClimb(PlayerManager pManager){
		if(ladderPresent){
			InitClimb(pManager);
			return true;
		}
		return false;
	}

	void InitClimb(PlayerManager pManager){
		GetComponent<Rigidbody>().isKinematic = true;
		pManager.currentState = PlayerManager.PlayerState.Traversing;
		isClimbing = true;
	}

	public void Tick(float v){
		if(!inPosition){
			t += Time.deltaTime;
			GetInPosition(t);
			return;
		}else{

			if(v > 0){
				float dis = Vector3.Distance(transform.position, ladder.topPos.position);
				if(dis < 1){
					Drop(v);
					return;
				}

				Vector3 tp = Vector3.MoveTowards(transform.position, ladder.topPos.position, speed * Time.deltaTime);
				transform.position = tp;
				return;
			}

			if(v < 0){
				float dis = Vector3.Distance(transform.position, ladder.bottomPos.position);
				if(dis < 0.1){
					Drop(v);
					return;
				}

				Vector3 tp = Vector3.MoveTowards(transform.position, ladder.bottomPos.position, speed * Time.deltaTime);
				transform.position = tp;
				return;
			}

			HandleAnimation(v);
		}
	}

	void Drop(float v){
		GetComponent<Rigidbody>().isKinematic = false;
		PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
		isClimbing = false;

		if(v > 0){
			GetComponent<PlayerMovement>().Jump(30);
		}
	}

	void GetInPosition(float delta){
		if(t > 1){
			t = 1;
			inPosition = true;
		}

		Vector3 startPos = ladder.bottomPos.position;
		startPos.y = transform.position.y;
		transform.position = Vector3.Lerp(transform.position, startPos, t *2);

		Quaternion rot = ladder.bottomPos.rotation;
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, t * 2);
	}

	void HandleAnimation(float v){
		
	}

	private void OnTriggerStay(Collider other){
		if(other.tag == "Ladder"){
			ladderPresent = true;
			ladder = other.transform.gameObject.GetComponent<Ladder>();
		}
	}

	private void OnTriggerExit(Collider other){
		if(other.tag == "Ladder"){
			ladderPresent = false;
		}
	}
}
