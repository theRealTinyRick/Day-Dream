using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {

	[SerializeField] ParticleSystem fx_Prefab;

	public void ExplodeInit(){
		//add it to level data that this obsticle is now removed
		if(fx_Prefab)
			Instantiate(fx_Prefab, transform.position, Quaternion.identity);
		
		Destroy(gameObject);
	}	
}
