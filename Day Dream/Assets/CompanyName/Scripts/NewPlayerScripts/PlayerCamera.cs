using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerCamera : MonoBehaviour 
	{	
		[TabGroup("Set Up")]
		[SerializeField]
		private Transform camLookAt;

		[TabGroup("Preferences")]
		[SerializeField]
		[Range(1, 20)]
		public float sensitivity;

		[TabGroup("Preferences")]
		[SerializeField]
		private float originalCameraDistance = 8;

		private Transform clippingOrigin;
		
		private float currentDistance = 10;
		private float Y_ANGLE_MIN = -60;
		private float Y_ANGLE_MAX = 20;
		private float camX;
		private float camY;
		
		private void Start()
		{
			clippingOrigin = new GameObject().transform;
			clippingOrigin.name = "Cam Clipping Origin";
			DontDestroyOnLoad(clippingOrigin);

		}

		private void FixedUpdate()
		{
			// CameraClipping();

			currentDistance = originalCameraDistance;
		}

		public void MouseOrbit(float x, float y)
		{
			camX += x * sensitivity;
			camY += y * sensitivity;

			// currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);  //set and check variables
			if(camY < Y_ANGLE_MIN){
				camY = Y_ANGLE_MIN + .01f;
			}else if(camY > Y_ANGLE_MAX){
				camY = Y_ANGLE_MAX;
			}

			Vector3 dis = new Vector3(0f, 0f, -currentDistance);   // use variables to get offeset and the rotation
			Quaternion rotation = Quaternion.Euler(-camY, camX, 0);
			Vector3 pos = camLookAt.position + rotation * dis; //apply rotation and offset to the position of the camera

			transform.position = Vector3.Lerp(transform.position, pos, 1f);
			transform.LookAt(camLookAt);    
		}   

		public void LockedOnCam()
		{
			// Vector3 tp = -currentDistance * Vector3.Normalize(pTargeting.currentTarget.transform.position - PlayerManager.instance.transform.position) + PlayerManager.instance.transform.position;
			// tp.y = PlayerManager.instance.transform.position.y + 4f;
			// transform.position = Vector3.Lerp(transform.position, tp, .4f);

			// Vector3 lookAtTP = pTargeting.currentTarget.transform.position;
			// if (pTargeting.currentTarget.transform.localScale.y > 1){
			// 	lookAtTP.y = pTargeting.currentTarget.transform.position.y + 2f;
			// }

			// Quaternion rot = Quaternion.LookRotation(lookAtTP - transform.position);
			// transform.rotation = Quaternion.Lerp(transform.rotation, rot, .4f);
		}

		public void CameraClipping()
		{
		    int layermask = 1<<8;
			layermask = ~layermask;

			clippingOrigin.position = camLookAt.position;
			Vector3 camPos = transform.position;
			Vector3 dir = camPos - clippingOrigin.transform.position;
			float distance = originalCameraDistance + 1.75f;

			RaycastHit hit;
			if (Physics.Raycast(clippingOrigin.position, dir, out hit, distance, layermask)){
				if (hit.collider.tag == "Environment" || hit.collider.tag == "Climbable" || hit.collider.tag == "Untagged"){
					float newDistance = Vector3.Distance(clippingOrigin.position, hit.point) - .75F;
					if(newDistance <= 0){
						newDistance = 0.1f;
					}
					currentDistance = Mathf.Lerp(currentDistance, newDistance, .8f);
				}
			}else
				currentDistance = Mathf.Lerp(currentDistance, originalCameraDistance, .1f);
		}
	}
}
