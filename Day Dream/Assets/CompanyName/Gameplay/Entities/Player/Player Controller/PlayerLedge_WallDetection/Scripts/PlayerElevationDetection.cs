using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class LedgeData
{

}

namespace AH.Max.Gameplay
{
	public class PlayerElevationDetection : MonoBehaviour 
	{
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float distanceToCheck;		

		[HideInInspector]
		public float DistanceToCheck{ get{ return distanceToCheck; } }

		/// <summary>
		/// The master bool to tell other systems if the player has hit a ledge
		/// </summary>
		[SerializeField]
		[TabGroup(Tabs.Properties)]
		private bool validLedge = false;
		public bool ValidLedge{ get { return validLedge; } }		

		/// <summary>
		/// The actual position of the ledge
		/// </summary>
		/// <returns></returns>
		[SerializeField]
		[TabGroup(Tabs.Properties)]
		private Vector3 ledge = new Vector3();
		public Vector3 Ledge{ get { return ledge; } }
		
		/// <summary>
		/// The normal of the wall that we are detecting against.
		/// It should be kept in mind that if you use this yo should assume to only use this if there is a validLedge
		/// </summary>
		/// <returns></returns>
		[SerializeField]
		[TabGroup(Tabs.Properties)]
		private Vector3 wallNormal = new Vector3();
		public Vector3 WallNormal{ get { return wallNormal; } }

		/// <summary>
		/// The events to tell other systems if we have ledge
		/// </summary>
		/// <returns></returns>
		public LedgeDetectedEvent ledgeDetectedEvent = new LedgeDetectedEvent();
		
		/// <summary>
		/// The events to tell other systems that we do not have a ledge
		/// </summary>
		/// <returns></returns>
		public NoLedgeDetectedEvent noLedgeDetectedEvent = new NoLedgeDetectedEvent();

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float maxHeight;

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float minHeight;

		private LayerMask layerMask = 1 << 8;

		private void Start()
		{
			layerMask = ~layerMask;
		}

		private void FixedUpdate()
		{
			DetectElevation();
		}

		private void DetectElevation()
		{
			// if( playerStateManager.CurrentState == PlayerState.Traversing ) return;
			// if( playerLocomotion.MoveDirection.magnitude < 0.5f ) return;

			Vector3 _origin = transform.position;
			_origin.y += 0.3f;

			RaycastHit _hit;

			Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
			if(Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck, layerMask))
			{
				Vector3 _ledge = _hit.point;
				Vector3 _normal = _hit.normal;
				//we store this distance so we can offset our downward ray cast and not over shoot it
				float _distanceToCheckDown = Vector3.Distance(_origin, _hit.point) + 0.1f;

				if(CheckWallAngle(_hit))
				{
					_origin = transform.position;
					_origin.y += maxHeight;
					
					Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
					if(!Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck, layerMask))
					{
						_origin = transform.position;

						//WARNING, This may cause misses
						_origin += transform.forward * _distanceToCheckDown;
						_origin.y += maxHeight;

						if(Physics.Raycast(_origin, Vector3.down, out _hit, maxHeight, layerMask))
						{
							_ledge.y = _hit.point.y;

							if(CheckFloorAngle(_hit))
							{
								// playerVault.FindActualPosition(_hit.point, layerMask, this);
								SetLedge(_ledge, _normal);
								return;
								// set a target position and tell the player vault that we have a ledge to get too. We will also be showing UI with this
							}
						}
					}
				}
			}

			DeleteLedge();
		}

		private bool CheckWallAngle(RaycastHit hit)
		{
			float _angle = Vector3.Angle(hit.normal, Vector3.up);

			if(_angle > 85)
			{
				return true;
			}

			return false;
		}

		private bool CheckFloorAngle(RaycastHit hit)
		{
			float _angle = Vector3.Angle(hit.normal, Vector3.up);

			if(_angle < 05f)
			{
				return true;
			}

			return false;
		} 

		private void SetLedge(Vector3 ledgePoint, Vector3 normal)
		{
			validLedge = true;
			ledge = ledgePoint;

			wallNormal = normal;

			if(ledgeDetectedEvent != null)
			{
				ledgeDetectedEvent.Invoke();
			}
		}

		private void DeleteLedge()
		{
			validLedge = false;
			ledge = Vector3.zero;

			if(noLedgeDetectedEvent != null)
			{
				noLedgeDetectedEvent.Invoke();
			}
		}

		/// <summary>
		/// Draw the ledge positno if its good
		/// </summary>
		private void OnDrawGizmos()
		{
			if(validLedge)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(ledge, Vector3.one * 0.15f);
			}
		}
	}
}
