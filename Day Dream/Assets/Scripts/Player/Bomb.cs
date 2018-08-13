using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	public bool isActivated = false;

	[SerializeField] ParticleSystem explosionFX;
	[SerializeField] float timeToExplode;
	[SerializeField] float timeToSelfDestruct;
	[SerializeField] float explosionRange;

	GameObject spawnedFX;
	public bool hasExploded = false;
	float t = 0.0f;

	void Start () {
	}
	
	void Update () {
		Tick();
	}

	void Tick(){
		if(!isActivated){
			t = Time.time;
		}

		if(isActivated && !hasExploded){
			if(Time.time - t > timeToExplode){
				Explode();
			}
		}

		if(hasExploded && Time.time - t >  timeToSelfDestruct){
			if(Time.time - t > timeToSelfDestruct){
				Destroy(spawnedFX);
				Destroy(gameObject);
			}
		}
	}

	void Explode(){
		hasExploded = true;
		t = Time.time;

		Vector3 tp = transform.position;
		Quaternion rot = Quaternion.identity;

		// spawnedFX = Instantiate(explosionFX, tp, rot);
		explosionFX.Play();

		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRange);
		foreach(Collider nearbyObject in colliders){
			if(nearbyObject.tag == "Explodable"){
				nearbyObject.GetComponent<Explode>().ExplodeInit();
			}
		}
	}
}
