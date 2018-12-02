using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAnimationController : MonoBehaviour 
{
	private Animator animator;
	PlayerAttackEvaluator playerAttackEvaluator;

	private bool isAttacking = false;
	public bool IsAttacking{ get{ return isAttacking; } }

	private static string[] swordAnimations = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};

	private int maxNumberOfClicks = swordAnimations.Length;
	private int currentNumberOfClicks = 0;

	private float timeToAtk = 0.4f;
	private float time = 0;

	void Start () 
	{
		animator = GetComponent<Animator>();
		playerAttackEvaluator = GetComponent<PlayerAttackEvaluator>();
	}

	private void OnEnabled()
	{
		InputDriver.lightAttackButtonEvent.AddListener(QuereyAttack);
	}

	private void OnDisabled()
	{
		InputDriver.lightAttackButtonEvent.RemoveListener(QuereyAttack);
	}

	private void Update()
	{
		AttackTimer();
	}

	private void AttackTimer()
	{
		if((Time.time - time) > timeToAtk)
			{
				foreach(string animation in swordAnimations)
				{
					if(animator.GetCurrentAnimatorStateInfo(0).IsName(animation))
					{
						// and the time since a click hasnt been too long
						isAttacking = true;
						return;
					}
				}

				foreach(string a in swordAnimations)
				{
					animator.SetBool(a, false);
				}

				isAttacking = false;
				currentNumberOfClicks = 0;
				
			}
			else
			{
				isAttacking = true;
			}
	}
	
	///<Summary>
	/// The input reciever for the standar attack in the game
	///</Summary>
	private void QuereyAttack()
	{
		// Run the attack evaluator thing
		if(playerAttackEvaluator.EvaluateAttackConditions())
		{
			// Do the attack
		}
	}
}
