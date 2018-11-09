using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour {

	[SerializeField]
	EnemyBase[] enemies;

	[SerializeField]
	private bool startCondition;

	[SerializeField]
	GameObject[] platforms;

	void Start () {
		DeactivatePlatforms();
		StartCoroutine(CheckEnemyHealth());
	}

	private void DeactivatePlatforms(){
		foreach(GameObject platform in platforms){
			platform.SetActive(false);
		}
	}
	
	private IEnumerator CheckEnemyHealth(){
		bool conditionMet = startCondition;
		while(!conditionMet){
			List<EnemyBase> deadEnemies = new List<EnemyBase>();
			foreach(EnemyBase enemy in enemies){
				if(enemy.CurrentHealth <= 0 && !deadEnemies.Contains(enemy)){
					deadEnemies.Add(enemy);
				}
				yield return new WaitForEndOfFrame();
			}

			if(deadEnemies.Count >= enemies.Length){
				conditionMet = true;
			}

			yield return new WaitForEndOfFrame();
		}

		StartCoroutine(RaisePlatform());

		yield return null;
	}

	private IEnumerator RaisePlatform(){
		foreach(GameObject platform in platforms){
			platform.SetActive(true);
			yield return new WaitForSeconds(0.3f);
		}
		yield return null;
	}
}
