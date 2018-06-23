using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour {

	private EnemyBase ebase;
	private BoxCollider weaponCollider;
	
	[SerializeField] private float attackSpeed = 0;
	[SerializeField] private float attackRecoverDelay = 0;


	private void Start(){
		ebase = GetComponent<EnemyBase>();
		weaponCollider = GetComponentInChildren<BoxCollider>();
		if(attackRecoverDelay>=attackSpeed)
			attackRecoverDelay = (attackSpeed-.5f);
		StartCoroutine(AttackPattern());
	}

	IEnumerator AttackPattern(){
		while(ebase.currentState != EnemyBase.State.Dead && PlayerManager.instance){
			if(ebase.CheckRange(ebase.attackRange, PlayerManager.instance.transform.position) && ebase.isAggro){
				FindRandomAttack();
				yield return new WaitForSeconds(attackRecoverDelay);
				if(ebase.currentState != EnemyBase.State.Dead || ebase.currentState != EnemyBase.State.Stunned){
					ebase.currentState = EnemyBase.State.Walking;
					weaponCollider.enabled = true;
				}
				yield return new WaitForSeconds(attackSpeed);
			}
			yield return new WaitForEndOfFrame();
		}
		yield return null;
	}

	void FindRandomAttack(){
		int result = Random.Range(0,3);
		string animation = "";
		switch(result){
			case 0:
				animation = "Attack1";
				break;
			case 1:
				animation = "Attack2";
				break;
			case 2:
				animation = "Attack3";
				break;
		}
		ebase.anim.Play(animation);
		ebase.currentState = EnemyBase.State.Attacking;
		weaponCollider.enabled = false;
	}
}
