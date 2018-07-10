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

    [SerializeField] private Transform startPosition;

    private void Awake(){
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){
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
