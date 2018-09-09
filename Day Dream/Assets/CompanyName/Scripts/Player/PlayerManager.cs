using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    
    public enum PlayerState { FreeMovement, CanNotMove, Traversing, FreeClimbing, Attacking, Dead};
    public static PlayerState currentState = PlayerState.FreeMovement;

    [TabGroup("Player States")]
    public PlayerState CurrentState;

    [TabGroup("Player States")]
    public bool isLockedOn = false;

    [TabGroup("Player States")]
    public bool isVulnerable = true;
    
    [TabGroup("Player States")]
    public bool isBlocking = false;

    [SerializeField]
    [TabGroup("Player States")]
    private bool isPaused = false;
    public bool IsPaused{ get{return isPaused;} }

    [SerializeField]
    [TabGroup("Player SetUp")]
    private Transform startPosition;

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

        CurrentState = currentState;
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

    public void LoadMainMenu(){
        GameManager.instance.LoadMainMenu();
    }
    
    public IEnumerator Invulnerabe(){
        isVulnerable = false;
        yield return new WaitForSeconds(.5f);
        isVulnerable = true;
    }

    private void OnTriggerEnter(Collider other){
        if(other.tag == "Water"){
            GetComponent<ParticleEffectManager>().WaterSplash();
            transform.position = startPosition.position;
        }
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.gameObject.tag == "Environment" ){
            Debug.Log("hit something");
        }
    }

}
