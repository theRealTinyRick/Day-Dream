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
        tp.y = PlayerManager.instance.transform.position.y + 1.5f;

        camLookAt.position = tp;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);  //set and check variables

        Vector3 dis = new Vector3(0f, 0f, -currentDistance);   // use variables to get offeset and the rotation
        Quaternion rotation = Quaternion.Euler(-currentY, currentX, 0);

        Vector3 pos = camLookAt.position + rotation * dis; //apply rotation and offset to the position of the camera
        transform.position = Vector3.Lerp(transform.position, pos, .7f);
    //    transform.position = pos;
        // Quaternion rot = Quaternion.LookRotation(camLookAt.position - transform.position);
        // transform.rotation = Quaternion.Lerp(transform.rotation, rot, .5f);
        transform.LookAt(camLookAt);    
    }

    public void LockedOnCam(){
        Vector3 tp = -currentDistance * Vector3.Normalize(PlayerManager.instance.targeting.currentTarget.transform.position - PlayerManager.instance.transform.position) + PlayerManager.instance.transform.position;
        tp.y = PlayerManager.instance.transform.position.y + 2.5f;
        // transform.position = tp;
        transform.position = Vector3.Lerp(transform.position, tp, .4f);

        Vector3 lookAtTP = PlayerManager.instance.targeting.currentTarget.transform.position;
        if (PlayerManager.instance.targeting.currentTarget.transform.localScale.y > 1){
            lookAtTP.y = PlayerManager.instance.targeting.currentTarget.transform.position.y + 2f;
        }
        Quaternion rot = Quaternion.LookRotation(lookAtTP - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, .4f);
    }

    public void ClimbingCamera(Vector3 tp){
        transform.position = Vector3.Lerp(transform.position, tp, .3f);
        transform.LookAt(camLookAt);
    }

    public void CameraClipping(){
        clippingOrigin.position = camLookAt.position;
        Vector3 camPos = transform.position;
        Vector3 dir = camPos - clippingOrigin.transform.position;
        float distance = originalCameraDistance + 1f;

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
