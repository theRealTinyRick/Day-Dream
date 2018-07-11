using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {
    public static PlayerManager instance;
    
    public enum PlayerState { FreeMovement, CanNotMove, Traversing, Attacking, Dead};
    public PlayerState currentState = PlayerState.FreeMovement;

    public bool isLockedOn = false;
    public bool isVulnerable = true;
    public bool isBlocking = false;

    [SerializeField]
    private bool isPaused = false;
    public bool IsPaused{
        get{return isPaused;}
    }

    [SerializeField]
    Transform startPosition;

    private void Awake(){
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

    }

    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){
        ResetPlayerPosition();
    }

    public void Pause(bool paused){
        if(paused){
            isPaused = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }else{
            isPaused = false;
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void ResetPlayerPosition(){
        if(Input.GetKeyDown(KeyCode.F1)){
            transform.position = startPosition.position;
        }
    }
    
    public IEnumerator Invulnerabe(){
        isVulnerable = false;
        yield return new WaitForSeconds(.5f);
        isVulnerable = true;
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Water"){
            transform.position = startPosition.position;
        }
    }

}
