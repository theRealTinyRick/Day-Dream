using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    private PlayerMovement move;
    private PlayerAttack atk;
    [HideInInspector]public Animator anim;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public ThirdPersonCamera playerCam;

    [SerializeField] private Transform headLevel;
    [SerializeField] private Transform midLevel;
    [SerializeField] private Transform feetLevel;
    
    [SerializeField] private float jumpHieght;
    [SerializeField] private float fallMultiplyer = 2.5f;
    [SerializeField] private float lowJumpMultiplyer = 3f;

    [HideInInspector]public GameObject ladder = null;
    [HideInInspector]public bool canClimbLadder = false;

    public enum PlayerState { FreeMovement, Traversing, Attacking, Blocking, Dead};
    public PlayerState currentState = PlayerState.FreeMovement;


    [SerializeField] float speed;

    float currentCamX  = 0.0f;
    float currentCamY  = 0.0f;

    public int coinCount = 0;


    private void Awake()
    {
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        #endregion

        move = GetComponent<PlayerMovement>();
        atk = GetComponent<PlayerAttack>();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerCam = GetComponentInChildren<ThirdPersonCamera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){
        MoveInput();
        AttackInput();
    }

    private void LateUpdate(){
        CameraInput();
    }

    private void FixedUpdate(){
        playerCam.CameraClipping();
        BetterJumpPhysics();
        CheckGrounded();
        CheckPlatform();
    }
    
    void MoveInput(){
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(h,0,v);
        
        //Jump
        if (CheckGrounded()){
            if (Input.GetKeyDown(KeyCode.Space)){
                if (ladder != null){
                    move.StartCoroutine(move.ClimbLadder(ladder.GetComponent<Ladder>().bottomPos.position, ladder.GetComponent<Ladder>().topPos.position, ladder.GetComponent<Ladder>().endPos.position));
                }else{
                    Debug.Log("Jump");
                    move.Jump(jumpHieght); //maybe remove standard jump mech
                    anim.SetBool("isGrounded", false);
                }
            }
        }

        if (currentState != PlayerState.Traversing && movement != Vector3.zero)
            move.FreeMovement(movement, speed);
        else
            anim.SetBool("isMoving", false);
    }

    void CameraInput(){
        currentCamX += Input.GetAxis("Mouse X");
        currentCamY += Input.GetAxis("Mouse Y"); 
        playerCam.MouseOrbit(currentCamX, currentCamY );
    }

    void AttackInput(){
        if(currentState == PlayerState.FreeMovement || currentState == PlayerState.Attacking){
            if (Input.GetMouseButtonDown(0))
                atk.Attack();
        }
    }

    public bool CheckGrounded(){
        RaycastHit hit;
        if(Physics.Raycast(feetLevel.position,-Vector3.up, out hit, 1.0f)){
            if(CheckRange(feetLevel.position, hit.point, .1f)){
                anim.SetBool("isGrounded", true);
                return true;
            }
        }
        anim.SetBool("isGrounded", false);
        return false;
    }

    public void CheckPlatform(){
        RaycastHit hit;
        if (Physics.Raycast(feetLevel.position, -Vector3.up, out hit, 1.0f)){
            if (CheckRange(feetLevel.position, hit.point, .15f)){
                if (hit.transform.tag == "Platform"){
                    transform.parent = hit.transform;
                    Debug.Log(transform.parent.name);
                }else
                    transform.parent = null;
            }
        }
    }

    public bool CheckRange(Vector3 start, Vector3 end, float range){
        if (Vector3.Distance(start, end) <= range)
            return true;
        return false;
    }

    private void BetterJumpPhysics( ){
        if (rb.velocity.y  < 0) 
            rb.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplyer - 1) * Time.deltaTime;
        else if (rb.velocity.y  > 0 && !Input.GetButton("Jump"))
            rb.velocity += Vector3.up * Physics.gravity.y  * (lowJumpMultiplyer - 1) * Time.deltaTime;
    }


}
