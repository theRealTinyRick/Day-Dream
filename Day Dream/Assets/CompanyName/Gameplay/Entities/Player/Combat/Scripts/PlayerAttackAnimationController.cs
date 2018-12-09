using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerAttackAnimationController : MonoBehaviour 
{
	[TabGroup(Tabs.Properties)]
	[ShowInInspector]
	private bool isAttacking = false;
	public bool IsAttacking{ get{ return isAttacking; } }

	[TabGroup(Tabs.Debug)]
	[Tooltip("This field is only used for debuging and has no barring on the functionality of queuing animations. That is done in the animator controller")]
	[SerializeField]
	private List <string> queue = new List<string>();
	
	[TabGroup(Tabs.Properties)]
	[ShowInInspector]
	private int maxNumberOfClicks = swordAnimations.Length;
	
	[TabGroup(Tabs.Properties)]
	[ShowInInspector]
	private int currentNumberOfClicks = 0;

	[TabGroup(Tabs.Properties)]
	[ShowInInspector]
	private float timeToClick = 0.4f;

	[TabGroup(Tabs.Properties)]
	[ShowInInspector]
	private float time = 0;

	private static string[] swordAnimations = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};
	private Animator animator;

	/// TODO
	void Start () 
	{
		animator = GetComponent<Animator>();
	}

	private void OnEnable()
	{
		InputDriver.lightAttackButtonEvent.AddListener(QuereyAttack);
	}

	private void OnDisable()
	{
		InputDriver.lightAttackButtonEvent.RemoveListener(QuereyAttack);
	}

	private void Update()
	{
		AttackTimer();
	}

	// this method simply determines if the player is still clicking. 
	// if the player keeps clicking then stops then the attacks should stop as well. 
	private void AttackTimer()
	{
		if(queue.Count > 0)
		{
			time += Time.deltaTime;

			if(time > timeToClick)
			{
				StopAttacking();
			}
		}
	}

	private void StopAttacking()
	{
		// clear out the queue and stop attacking
		foreach(string _animation in swordAnimations)
		{
			if(_animation != swordAnimations[0])
			{
				animator.SetBool(_animation, false);
			}
		}

		queue.Clear();
		currentNumberOfClicks = 0;
		time = 0;
		isAttacking = false;
	}
	
	///<Summary>
	/// The input reciever for the standar attack in the game
	///</Summary>
	private void QuereyAttack()
	{
		if(EvaluateQueueConditions())
		{
			currentNumberOfClicks ++;
			time = 0;

			int _index = currentNumberOfClicks - 1;	

			queue.Add(swordAnimations[_index]);

			// I use Play() with the first animation to start it immeiately 
			if(_index== 0)
			{
				animator.Play(swordAnimations[0]);
			}
			else
			{
				animator.SetBool(swordAnimations[_index], true);
			}

			isAttacking = true;
		}
	}

	private bool EvaluateQueueConditions()
	{
		if(currentNumberOfClicks < maxNumberOfClicks)
		{
			return true;
		}
		else
		{
			time = 0;
		}

		return false;
	}

	public void ExitAttackQueue()
	{
		StopAttacking();
	}
}
