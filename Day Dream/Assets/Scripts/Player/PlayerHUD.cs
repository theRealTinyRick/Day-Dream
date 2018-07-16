using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour {

	[SerializeField]
	private Image healthBar;

	private PlayerHealth pHealth;

	void Start(){
		pHealth = GetComponent<PlayerHealth>();
	}

	void Update () {
		SetHealthBar();
	}

	private void SetHealthBar(){
		float fillAmount = pHealth.CurrentHealth/pHealth.StartingHealth;
		healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, fillAmount, 0.3f);
	}

	private void SetPoiseMeter(){
		//when the poise meter is build
	}
}
