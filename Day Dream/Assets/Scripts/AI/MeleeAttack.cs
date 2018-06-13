using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour {

	EnemyBase ebase;
	[SerializeField] private float attackSpeed = 0;
	[SerializeField] private float attackRecoverDelay = 0;

	private void Start(){
		ebase = GetComponent<EnemyBase>();

		if(attackRecoverDelay>=attackSpeed)
			attackRecoverDelay = (attackSpeed-.5f);
		
		StartCoroutine(AttackPattern());
	}

	IEnumerator AttackPattern(){
		while(ebase.currentState != EnemyBase.State.Dead){
			yield return new WaitForSeconds(attackSpeed);
			
			while(!ebase.CheckRange(ebase.attackRange, ebase.Player.transform.position)){
				yield return new WaitForEndOfFrame();
			}

			FindRandomAttack();
			yield return new WaitForSeconds(attackRecoverDelay);

			if(ebase.currentState != EnemyBase.State.Dead || ebase.currentState != EnemyBase.State.Stunned){
				ebase.currentState = EnemyBase.State.Walking;
			}
			yield return new WaitForEndOfFrame();
		}

		yield return null;
	}

	void FindRandomAttack(){
		int result = Random.Range(0,3);
		switch(result){
			case 0:
				AttackOne();
				break;
			case 1:
				AttackTwo();
				break;
			case 2:
				AttackThree();
				break;
		}
	}

	void AttackOne(){
		ebase.anim.Play("Attack1");
		ebase.currentState = EnemyBase.State.Attacking;
	}

	void AttackTwo(){
		ebase.anim.Play("Attack2");
		ebase.currentState = EnemyBase.State.Attacking;
	}

	void AttackThree(){
		ebase.anim.Play("Attack3");
		ebase.currentState = EnemyBase.State.Attacking;
	}
}
