using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerTargeting : MonoBehaviour {

    private const string lockedOn = "LockedOn"; 
   
    [HideInInspector]
    public List<GameObject> enemiesInArea = new List<GameObject>();

    [HideInInspector]
    public GameObject currentTarget;

	public static PlayerTargeting instance;
    private Animator anim;

    private void Awake(){
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    private void Start(){
        anim = PlayerManager.instance.GetComponent<Animator>();
    }

    public void ToggleLockedOnEnemies(){
        if (enemiesInArea.Count > 0){
            if (currentTarget == null){
                // there is no target so set current target to the first in the list
                currentTarget = enemiesInArea[0];
                PlayerManager.instance.isLockedOn = true;
                anim.SetFloat(lockedOn, 1);
            }
            else{
                //there is a target so find out if its the last in the list then set the current targte to the next enemy
                if (enemiesInArea.IndexOf(currentTarget) < enemiesInArea.Count-1){
                    currentTarget = enemiesInArea[enemiesInArea.IndexOf(currentTarget) + 1];
                }
                else{
                    currentTarget = enemiesInArea[0];
                }
            }
        }
        else
            LockOff();
    }

    public void LockOff(){
        currentTarget = null;
        PlayerManager.instance.isLockedOn = false;
        anim.SetFloat(lockedOn, 0);
    }

    private void OnTriggerEnter(Collider other){
        if (other.tag == "LockOnTarget"){
            if(!enemiesInArea.Contains(other.gameObject))
                enemiesInArea.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "LockOnTarget"){
            enemiesInArea.Remove(other.gameObject);
            ToggleLockedOnEnemies();
        }
    }
}
