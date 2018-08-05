using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeClimbAnimationHook : MonoBehaviour {

	Animator anim;

	void Start () {
		anim = GetComponent<Animator>();
	}
	
	public void HandleAnimation(float h, float v){
		// anim.SetFloat("FC_velocityX", h);
		// anim.SetFloat("FC_velocityY", v);
		string[] animations = new string[]{"FC_Up", "FC_RightUp", "FC_Right", 
		"FC_RightDown", "FC_Down", "FC_LeftDown", "FC_Left", "FC_LeftUp"};
		
		if(v > 0 && h == 0){
			anim.Play(animations[0]);

		}else if(v > 0 && h > 0){
			anim.Play(animations[1]);

		}else if(v == 0 && h > 0){
			anim.Play(animations[2]);

		}else if(v < 0 && h > 0){
			anim.Play(animations[3]);

		}else if(v < 0 && h == 0){
			anim.Play(animations[4]);

		}else if(v < 0 && h < 0){
			anim.Play(animations[5]);

		}else if(v == 0 && h < 0){
			anim.Play(animations[6]);

		}else if(v > 0 && h < 0){
			anim.Play(animations[7]);
		}
	}

	public void SetAnimZero(){
		anim.SetFloat("FC_velocityX", 0);
		anim.SetFloat("FC_velocityY", 0);
	}
}
