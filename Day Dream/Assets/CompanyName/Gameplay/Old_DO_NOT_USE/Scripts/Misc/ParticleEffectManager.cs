using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectManager : MonoBehaviour {

	[SerializeField] ParticleSystem waterSplash;
	[SerializeField] GameObject shadow;

	private void LateUpdate(){
		SetGroundShadow();
	}

	private void SetGroundShadow(){
        RaycastHit hit; 
		Vector3 feetLevel = transform.position;
        if(Physics.Raycast(feetLevel, -Vector3.up, out hit, 100)){
            shadow.SetActive(true);
            Vector3 tp = hit.point;
            tp.y = hit.point.y + 0.05f;
            shadow.transform.position = Vector3.Lerp(shadow.transform.position, tp, 1f);
        }else{
            shadow.SetActive(false);
        }
    }

	public void WaterSplash(){
		Vector3 tp = transform.position;
		waterSplash.transform.position = tp;
		waterSplash.Play();
	}
}
