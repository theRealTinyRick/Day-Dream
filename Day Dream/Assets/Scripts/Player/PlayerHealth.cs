using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

	[SerializeField]
	private float startingHealth;
	public float StartingHealth{
		get{return startingHealth;}
	}

	[SerializeField]
	private float currentHealth;
	public float CurrentHealth{
		get{return currentHealth;}
	}

	void Start () {
		currentHealth = startingHealth;
	}
}
