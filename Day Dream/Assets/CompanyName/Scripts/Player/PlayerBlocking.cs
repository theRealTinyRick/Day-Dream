using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlocking : MonoBehaviour {

	private bool isBlocking = false;
	public bool IsBlocking{
		get{return isBlocking;}
	}

	Animator anim;

	void Start () {
		anim = GetComponent<Animator>();
	}
	
	void Update () {
		HandleBlock();
	}

	public void SetBlocking(bool val){
		isBlocking = val;	
	}

	private void HandleBlock(){
		if(isBlocking){
			anim.SetBool("IsBlocking", true);
		}else{
			anim.SetBool("IsBlocking", false);
		// anim.SetFloat("velocityX", 0);
		}
	}	
}
