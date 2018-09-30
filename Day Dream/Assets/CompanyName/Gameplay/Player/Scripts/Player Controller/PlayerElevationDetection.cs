using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerElevationDetection : MonoBehaviour 
	{
		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float distanceToCheck;		

		[HideInInspector]
		public float DistanceToCheck{ get{ return distanceToCheck; } }
		
		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float maxHeight;

		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float minHeight;

		private Animator animator;
		private PlayerVault playerVault;
		private PlayerLocomotion playerLocomotion;
		private PlayerStateManager playerStateManager;
		private LayerMask layerMask = 1 << 8;

		private void Start()
		{
			animator = GetComponent<Animator>();
			playerVault = GetComponent<PlayerVault>();
			playerLocomotion = GetComponent<PlayerLocomotion>();
			playerStateManager = GetComponent <PlayerStateManager>();
		
			layerMask = ~layerMask;
		}

		private void FixedUpdate()
		{
			DetectElevation();
		}

		private void DetectElevation()
		{
			if( playerStateManager.CurrentState == PlayerState.Traversing ) return;
			if( playerLocomotion.MoveDirection.magnitude > 0.5f ) return;

			Vector3 origin = transform.position;
			origin.y += 0.3f;

			RaycastHit hit;

			Debug.DrawRay(origin, transform.forward * distanceToCheck, Color.red);
			if(Physics.Raycast(origin, transform.forward, out hit, distanceToCheck, layerMask))
			{
				if(CheckWallAngle(hit))
				{
					origin = transform.position;
					origin.y += maxHeight;
					
					Debug.DrawRay(origin, transform.forward * distanceToCheck, Color.red);
					if(!Physics.Raycast(origin, transform.forward, out hit, distanceToCheck, layerMask))
					{
						origin = transform.position;
						origin += transform.forward * distanceToCheck;
						origin.y += maxHeight;

						if(Physics.Raycast(origin, Vector3.down, out hit, maxHeight, layerMask))
						{
							if(CheckFloorAngle(hit))
							{
								playerVault.FindActualPosition(hit.point, layerMask, this);
							}
						}
					}
				}
			}
		}

		private bool CheckWallAngle(RaycastHit hit)
		{
			float angle = Vector3.Angle(hit.normal, Vector3.up);

			if(angle > 85)
			{
				return true;
			}

			return false;
		}

		private bool CheckFloorAngle(RaycastHit hit)
		{
			float angle = Vector3.Angle(hit.normal, Vector3.up);

			if(angle < 05f)
			{
				return true;
			}

			return false;
		} 
	}

	
}
