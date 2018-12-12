using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerLocomotionAnimationHook : MonoBehaviour 
{
	public const string LockedOn = "LockedOn";
	public const string Horizontal = "Horizontal";
	public const string Vertical = "Vertical";

	[TabGroup("Animation")]
	[SerializeField]
	private float verticalAnimatorFloat = 0;

	[TabGroup("Animation")]
	[SerializeField]
	private float horizontalAnimatorFloat = 0;

	[TabGroup("Animation")]
	[SerializeField]
	[Range(0, 1)]
	private int lockedOnAnimatorFloat = 0; // for testing purposes

	private PlayerLocomotion playerLocomotion;
	private PlayerStateComponent playerStateComponent;
	private PlayerAttackAnimationController playerAttackAnimatorController;
	private Animator animator;

	private void Start()
	{
		playerLocomotion = GetComponent<PlayerLocomotion>();
		playerStateComponent= GetComponent<PlayerStateComponent>();
		playerAttackAnimatorController = GetComponent<PlayerAttackAnimationController>();
		animator = GetComponent<Animator>();
	}

	private void Update () 
	{
		LocomotionAnimation();
		ApplyAnimationFloats();
	}

	private void LocomotionAnimation()
	{
		Vector3 _forwardVector = transform.forward;
		Vector3 _moveDirection = playerLocomotion.playerOrientationDirection * InputDriver.LocomotionDirection.magnitude; //multiply here so we can dampen some of the values.
		Vector3 _crossProduct = Vector3.Cross(_forwardVector, _moveDirection);

		Debug.DrawRay(transform.position, _moveDirection, Color.red);
		Debug.DrawRay(transform.position, _forwardVector, Color.blue);
		Debug.DrawRay(transform.position, _crossProduct, Color.green);

		if(lockedOnAnimatorFloat == 0)
		{
			verticalAnimatorFloat = _moveDirection.magnitude;
			horizontalAnimatorFloat = _crossProduct.y;
		}
		else
		{
			verticalAnimatorFloat = _moveDirection.magnitude * Vector3.Dot(_forwardVector, _moveDirection);
			horizontalAnimatorFloat = _crossProduct.y;
		}
	}

	private void ApplyAnimationFloats()
	{
		if(playerAttackAnimatorController.CurrentlyInAttackState())
		{
			animator.SetFloat(Horizontal, 0);
			animator.SetFloat(Vertical, 0);
			return;
		}

		animator.SetFloat(LockedOn, (float)lockedOnAnimatorFloat);
		animator.SetFloat(Horizontal, horizontalAnimatorFloat);
		animator.SetFloat(Vertical, verticalAnimatorFloat);
	}
}
