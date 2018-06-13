using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    private Transform camLookAt;
    private Transform clippingOrigin;

    private float currentDistance = 5;
    private float originalCameraDistance = 5;
    private float Y_ANGLE_MIN = -30;
    private float Y_ANGLE_MAX = 12;

    private void Start(){
        camLookAt = new GameObject().transform;
        camLookAt.name = "Cam Look At Point";
        DontDestroyOnLoad(camLookAt);

        clippingOrigin = new GameObject().transform;
        clippingOrigin.name = "Cam Clipping Origin";
        DontDestroyOnLoad(clippingOrigin);

        camLookAt.transform.parent = PlayerManager.instance.transform;
        clippingOrigin.transform.parent = PlayerManager.instance.transform;
    }

    public void MouseOrbit(float currentX, float currentY){
        Vector3 tp = PlayerManager.instance.transform.position;
        tp.y = PlayerManager.instance.transform.position.y + 1f;

        camLookAt.position = tp;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);  //set and check variables

        Vector3 dis = new Vector3(0f, 0f, -currentDistance);   // use variables to get offeset and the rotation
        Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);

        transform.position = camLookAt.position + rotation * dis; //apply rotation and offset to the position of the camera

        transform.LookAt(camLookAt.position);  //Look at
    }

    public void CameraClipping(){
        clippingOrigin.position = camLookAt.position;
        Vector3 camPos = transform.position;
        Vector3 dir = camPos - clippingOrigin.transform.position;
        float distance = originalCameraDistance + 2f;

        RaycastHit hit;
        if (Physics.Raycast(clippingOrigin.position, dir, out hit, distance)){
            if (hit.collider.tag == "Environment"){
                float newDistance = Vector3.Distance(clippingOrigin.position, hit.point) - .5F;
                if(newDistance < 0){
                    newDistance = 0;
                }
                currentDistance = Mathf.Lerp(currentDistance, newDistance, .8f);
            }
        }else
            currentDistance = Mathf.Lerp(currentDistance, originalCameraDistance, .1f);
    }
}
