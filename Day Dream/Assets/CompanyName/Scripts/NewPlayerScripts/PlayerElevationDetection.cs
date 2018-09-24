using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerElevationDetection : MonoBehaviour 
	{
		[TabGroup("Preferences")]
		[SerializeField]
		private float distanceToCheck;		
		
		[TabGroup("Preferences")]
		[SerializeField]
		private float maxHeight;

		[TabGroup("Preferences")]
		[SerializeField]
		private float minHeight;

		private Animator animator;
		private PlayerStateManager playerStateManager;
		private LayerMask layerMask = 1 << 8;

		private void Start ()
		{	
			animator = GetComponent<Animator>();
			playerStateManager = GetComponent<PlayerStateManager>();

			layerMask = ~layerMask;			
		}
		
		private void Update () 
		{
			DetectObsticals();
		}

		private void DetectObsticals()
		{
			Vector3 direction = transform.forward;
			Vector3 origin = transform.position;
			origin.y += 0.5f;

			RaycastHit hit;

			Debug.DrawRay(origin, transform.forward, Color.blue);
			if(Physics.Raycast(origin, direction, out hit, distanceToCheck, layerMask))
			{
				if(Vector3.Angle(hit.normal, Vector3.up) > 45)
				{	
					origin.y += 3;
					Vector3 checkLocationOne = origin + transform.forward;
					Vector3 checkLocationTwo = origin + transform.forward * 1.5f;
					Vector3 checkLocationThree = origin + transform.forward * 2f;
					Vector3 checkLocationFour = origin + transform.forward * 2.5f;

					RaycastHit hit1;
					RaycastHit hit2;
					RaycastHit hit3;
					RaycastHit hit4;
					
					Debug.DrawRay(checkLocationOne, Vector3.down * 5, Color.blue);
					Debug.DrawRay(checkLocationTwo, Vector3.down * 5, Color.yellow);
					Debug.DrawRay(checkLocationThree, Vector3.down * 5, Color.red);
					Debug.DrawRay(checkLocationFour, Vector3.down * 5, Color.green);

					ElevationPoint[] elevationPoints = new ElevationPoint[4];

					if(Physics.Raycast(checkLocationOne, Vector3.down, out hit1, 10, layerMask))
					{
						elevationPoints[0] = new ElevationPoint(hit1.point);
					}

					if(Physics.Raycast(checkLocationTwo, Vector3.down, out hit2, 10, layerMask))
					{
						elevationPoints[1]= new ElevationPoint(hit2.point);
					}

					if(Physics.Raycast(checkLocationThree, Vector3.down, out hit3, 10, layerMask))
					{
						elevationPoints[2] = new ElevationPoint(hit3.point);
					}

					if(Physics.Raycast(checkLocationFour, Vector3.down, out hit4, 10, layerMask))
					{
						elevationPoints[3] = new ElevationPoint(hit4.point);
					}

					foreach(ElevationPoint e in elevationPoints)
					{
						//check all hieghts against the players height and figure out what action to take
						if(e.height > transform.position.y)
						{
							if( e.height - transform.position.y > minHeight && e.height - transform.position.y < maxHeight)
							{
								Debug.Log("Location good");
								e.validPoint = true;
							}
						}
					}

					//use the valid points to work out an animation
					
				}
			}
		}
	}

	public class ElevationDetectionAnimationHook 
	{

	}

	public class ElevationPoint
	{	
		public Vector3 point;
		public float height;
		public bool validPoint = false;

		public ElevationPoint(Vector3 point)
		{
			this.point = point;
			this.height = point.y;
		}
	}
}
