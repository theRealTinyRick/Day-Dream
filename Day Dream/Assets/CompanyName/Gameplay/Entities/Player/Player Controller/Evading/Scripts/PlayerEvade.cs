using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvade : MonoBehaviour 
{
	public const string EvadeAnimation = "Evade";
	public const string EvadeHorizontal = "EvadeHorizontal";
	public const string EvadeVertical = "EvadeVertical";

	private Animator animator;
	private PlayerLocomotionAnimationHook playerLocomotionAnimationHook;
	private PlayerAttackAnimationController playerAttackAnimationController;

	public bool testLockOn = false;

	public bool isEvading;

	void Start () 
	{
		animator = GetComponent<Animator>();
		playerLocomotionAnimationHook = GetComponent<PlayerLocomotionAnimationHook>();
		playerAttackAnimationController = GetComponent<PlayerAttackAnimationController>();
	}

	private void OnEnable() 
	{
		InputDriver.jumpButtonEvent.AddListener(Evade);
	}

	private void OnDisable() 
	{
		InputDriver.jumpButtonEvent.RemoveListener(Evade);	
	}
	
	private void Evade()
	{
		playerAttackAnimationController.StopAttacking();
		isEvading = true;

		if(testLockOn)
		{
			if(InputDriver.LocomotionOrientationDirection != Vector3.zero)
			{
				animator.SetFloat(EvadeHorizontal, playerLocomotionAnimationHook.horizontalAnimatorFloat);
				animator.SetFloat(EvadeVertical, playerLocomotionAnimationHook.verticalAnimatorFloat);
				animator.Play(EvadeAnimation);
				
				return;
			}
		}
		
		DefaultDash();
	}

	private void DefaultDash()
	{
		animator.SetFloat(EvadeHorizontal, 0);
		animator.SetFloat(EvadeVertical, 0.5f);
		animator.Play(EvadeAnimation);
	}

	public void StoppedEvading()
	{
		isEvading = false;
	}
}