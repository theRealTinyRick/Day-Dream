using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerGroundedComponent : MonoBehaviour 
{
	/// <summary>
	/// Determines if the player is on the ground
	/// </summary>
	private bool isGrounded;
	public bool IsGrounded
	{
		get
		{
			return isGrounded;
		}
	}

	[SerializeField]
	private Vector3[] raycastOffsets;

	[SerializeField]
	[Range(0, 1)]
	private float offsetDistance;

	[SerializeField]
	[Range(0, 1)]
	private float yOffset;	

	private void FixedUpdate() 
	{
		isGrounded = Grounded();
	}

	///<Summary>
	/// Use this method to check if the chracter is reasonalbly on the ground
	///</Summary>
	public bool Grounded()
	{
		if(raycastOffsets == null)
		{
			Debug.LogError("You probably have not set up the raycast offsets on the PlayerGroundedComponent");
			return false;
		} 

		foreach(Vector3 _position in raycastOffsets)
		{
			Vector3 _tp = transform.position + (_position * offsetDistance);
			_tp.y  = transform.position.y + yOffset;

			RaycastHit _hit;
			if(Physics.Raycast(_tp, Vector3.down, out _hit, 1, PhysicsLayers.ingnorePlayerLayer))
			{
				return true;
			}
		}

		return false;
	}

	///<Summary>
	/// Draw the positions generating the raycasts
	///</Summary>
	private void OnDrawGizmos()
	{
		if(raycastOffsets == null) return;

		foreach(Vector3 _position in raycastOffsets)
		{
			Gizmos.color = Color.red;
			
			Vector3 _tp = transform.position + (_position * offsetDistance);
			_tp.y  = transform.position.y + yOffset;

			Gizmos.DrawSphere(_tp, 0.05f);
		}
	}	

	//FOR LATER USE

			// 	if (_rigidbody.velocity.y  < 0) 
            // 	_rigidbody.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplyer - 1) * Time.deltaTime;
			// else if (_rigidbody.velocity.y  > 0 )
			// 	_rigidbody.velocity += Vector3.up * Physics.gravity.y *2 * Time.deltaTime;
}
