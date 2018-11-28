using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerGroundedComponent : MonoBehaviour 
{
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
		if(Grounded())
		{
			Debug.Log("grounded");
		}	
		else
		{
			Debug.Log("not grounded");
		}
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
}
