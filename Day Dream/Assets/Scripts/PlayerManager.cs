using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    
    public enum PlayerState { FreeMovement, CanNotMove, Traversing, Attacking, Blocking, Dead};
    public PlayerState currentState = PlayerState.FreeMovement;

    public bool isLockedOn = false;
    public bool isVulnerable = true;

    [SerializeField] private Transform startPosition;

    private void Awake(){
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        #endregion

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.F1)){
            transform.position = startPosition.position;
        }
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Water"){
            transform.position = startPosition.position;
        }
    }

}
