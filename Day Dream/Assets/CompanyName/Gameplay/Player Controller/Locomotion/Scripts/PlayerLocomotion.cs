using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.System;

public class PlayerLocomotion : MonoBehaviour 
{
	[TabGroup("TestIng")]
	[SerializeField]
	private bool LockedOn; // Remove

	[TabGroup("TestIng")]
	[SerializeField]
	private bool defending; // Remove

	[TabGroup("TestIng")]
	[SerializeField]
	private bool attacking;	// Remove
	
	[TabGroup("TestIng")]
	[SerializeField]
	private Transform target; // Remove

	[TabGroup(Tabs.Locomotion)]
	[SerializeField]
	private float baseSpeed;

	[TabGroup(Tabs.Locomotion)]
	[SerializeField]
	private float sprintSpeed;

	[TabGroup(Tabs.Locomotion)]
	[SerializeField]
	private float turnDamping;

	public Vector3 playerOrientationDirection = new Vector3();
	public Vector3 playerOrientationDirectionNotNormalized = new Vector3();

	private Rigidbody _rigidbody;
	private PlayerLocomotionAnimationHook playerLocomotionAnimationHook;

	private void Start () 
	{
		_rigidbody = GetComponentInChildren<Rigidbody>();
		playerLocomotionAnimationHook = GetComponentInChildren<PlayerLocomotionAnimationHook>();
	}
	
	private void FixedUpdate () 
	{
		Move(LockedOn);
		
		if(!LockedOn)
		{
			RotatePlayer();
		}
		else
		{
			if(defending || attacking)
			{
				FaceTarget();
			}
		}
	}
	
	private void Move(bool lockedOn)
	{
		Vector3 _direction = GetOrientationDirection();
		_rigidbody.velocity 
		= new Vector3( (_direction.x * baseSpeed) * InputDriver.LocomotionDirection.magnitude, _rigidbody.velocity.y, (_direction.z * baseSpeed) * InputDriver.LocomotionDirection.magnitude ) ;
	}

	private void RotatePlayer()
	{
		if(playerOrientationDirection != Vector3.zero)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, GetOrientationRotation(), turnDamping);
		}
	}

	private void FaceTarget()
	{
		Vector3 _direction = target.position - transform.position;
		_direction.y = 0;
		Quaternion _rotation = Quaternion.LookRotation(_direction);

		transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, turnDamping);
	}
	
	private Vector3 GetOrientationDirection()
	{
		Vector3 _direction = InputDriver.LocomotionDirection;
		_direction = EntityManager.Instance.GameCamera.transform.TransformDirection(_direction).normalized;
		_direction.y = 0;

		InputDriver.LocomotionOrientationDirection = _direction;
		playerOrientationDirection = _direction;

		return _direction;
	}

	private Quaternion GetOrientationRotation()
	{
		return Quaternion.LookRotation(GetOrientationDirection());
	}
}
