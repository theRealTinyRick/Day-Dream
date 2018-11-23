using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerLocomotion : MonoBehaviour 
{
	[TabGroup(Tabs.SetUp)]
	[SerializeField]
	private Transform playerCamera;

	[TabGroup(Tabs.Locomotion)]
	[SerializeField]
	private float baseSpeed;

	[TabGroup(Tabs.Locomotion)]
	[SerializeField]
	private float sprintSpeed;

	[TabGroup(Tabs.Locomotion)]
	[SerializeField]
	private float turnDamping;

	private Vector3 playerOrientationDirection = new Vector3();

	private Rigidbody _rigidbody;
	private PlayerLocomotionAnimationHook playerLocomotionAnimationHook;

	private void Start () 
	{
		_rigidbody = GetComponentInChildren<Rigidbody>();
		playerLocomotionAnimationHook = GetComponentInChildren<PlayerLocomotionAnimationHook>();
	}
	
	private void FixedUpdate () 
	{
		Move();
	}
	
	private Vector3 GetOrientationDirection()
	{
		Vector3 _direction = InputDriver.LocomotionDirection;
		_direction = playerCamera.TransformDirection(_direction).normalized;

		InputDriver.LocomotionOrientationDirection = _direction;
		playerOrientationDirection = _direction;

		return _direction;
	}

	private void Move()
	{
		Vector3 _direction = GetOrientationDirection();
		_rigidbody.velocity = new Vector3( _direction.x * baseSpeed, _rigidbody.velocity.y, _direction.z * baseSpeed );
	}

	private Quaternion GetOrientationRotation()
	{
		return Quaternion.LookRotation(GetOrientationDirection());
	}

	private void RotatePlayer()
	{
		if(playerOrientationDirection != Vector3.zero)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, GetOrientationRotation(), turnDamping);
		}
	}
}
