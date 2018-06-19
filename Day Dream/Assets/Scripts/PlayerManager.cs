using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    
    private PlayerMovement move;
    private PlayerAttack atk;
    private Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public ThirdPersonCamera playerCam;

    public enum PlayerState { FreeMovement, Traversing, Attacking, Blocking, Dead};
    public PlayerState currentState = PlayerState.FreeMovement;

    [SerializeField] private float speed = 5;
    private Vector3 movement = Vector3.zero;
    float currentCamX  = 0.0f;
    float currentCamY  = 0.0f;
    public int coinCount = 0;
    [SerializeField] private float jumpHieght;
    [SerializeField] private float fallMultiplyer = 2.5f;
    [SerializeField] private float lowJumpMultiplyer = 3f;


    [HideInInspector] public GameObject ladder = null;
    [HideInInspector] public GameObject shimyPipe = null;
    public GameObject pickUpObject = null;
    [SerializeField] private Transform putDownPos;
    [SerializeField] private Transform feetLevel;

    [SerializeField] private bool isHoldingObject;

    private void Awake(){
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
        playerCam = Camera.main.GetComponent<ThirdPersonCamera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update(){
        AttackInput();
        MoveInput();
        CameraInput();
        Interact();
    }

    private void LateUpdate(){
    }

    private void FixedUpdate(){
        ApplyMove();
        playerCam.CameraClipping();
        BetterJumpPhysics();
        CheckGrounded();
        CheckPlatform();
    }
    
    void MoveInput(){
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 m = new Vector3(h,0,v);
        movement = m;
        
        //Jump
        if (Input.GetKeyDown(KeyCode.Space)){
            if(currentState != PlayerState.Traversing){
                if (ladder && CheckGrounded()){
                    move.StartCoroutine(move.ClimbLadder(ladder.GetComponent<Ladder>().bottomPos.position, 
                    ladder.GetComponent<Ladder>().topPos.position, ladder.GetComponent<Ladder>().endPos.position));
                }else if(shimyPipe && CheckGrounded()){
                    move.StartCoroutine(move.ShimyPipeStart(shimyPipe));
                }else if(CheckGrounded()){
                    move.Jump(jumpHieght); //maybe remove standard jump mech
                    anim.SetBool("isGrounded", false);
                }
            }else{
                if(shimyPipe){
                    move.EndShimy();
                }
            }
        }
    }

    private void ApplyMove(){
        //apply move controls
        if (currentState != PlayerState.Traversing && movement != Vector3.zero)
            move.FreeMovement(movement, speed);
        else if(currentState == PlayerState.Traversing && shimyPipe && movement != Vector3.zero){
            move.ShimyPipe(movement, shimyPipe.GetComponent<ShimyPipe>());
        }else
            anim.SetBool("isMoving", false);
    }

    void CameraInput(){
        currentCamX += Input.GetAxis("Mouse X");
        currentCamY += Input.GetAxis("Mouse Y"); 
        playerCam.MouseOrbit(currentCamX, currentCamY );
    }

    void AttackInput(){
        if(currentState == PlayerState.FreeMovement || currentState == PlayerState.Attacking){
            if (Input.GetMouseButtonDown(0) && CheckGrounded())
                atk.Attack();
        }
    }

    public bool CheckGrounded(){
        RaycastHit hit;
        if(Physics.Raycast(feetLevel.position,-Vector3.up, out hit, 0.1f)){
            anim.SetBool("isGrounded", true);
            return true;
        }else{
            anim.SetBool("isGrounded", false);
            return false;
        }

    }

    private void Interact(){
        if(Input.GetKeyDown(KeyCode.F)){
            if(pickUpObject && isHoldingObject){
                //drop
                isHoldingObject = false;
            }else if(pickUpObject && !isHoldingObject){
                //pickup
                isHoldingObject = true;
                StartCoroutine(PickUpObject());
            }
        }
    }

    private IEnumerator PickUpObject(){
        Debug.Log("start");
        while(isHoldingObject){
            Vector3 tp = transform.position;
            tp.y = transform.position.y + 2;
            pickUpObject.transform.position = Vector3.Lerp(pickUpObject.transform.position, tp, .3f);
            pickUpObject.GetComponent<Rigidbody>().isKinematic = true;
            pickUpObject.GetComponent<BoxCollider>().enabled = false;
            speed = 2.5f;
            yield return new WaitForEndOfFrame();
        }
        speed = 5;
        pickUpObject.GetComponent<Rigidbody>().isKinematic = false;
        pickUpObject.GetComponent<BoxCollider>().enabled = true;
        yield return null;
    }

    public void GroundedDelay(){
        //I created this function to give the player a split second after coming off the ledge to jump
    }

    public void CheckPlatform(){
        RaycastHit hit;
        if (Physics.Raycast(feetLevel.position, -Vector3.up, out hit, 1.0f)){
            if (CheckRange(feetLevel.position, hit.point, .5f)){
                if (hit.transform.tag == "Platform"){
                    transform.parent = hit.transform;
                }else
                    transform.parent = null;
            }
        }else{
            transform.parent = null;
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

    private void OnTriggerEnter(Collider other){
        if(other.tag == "PickUp"){
            pickUpObject = other.gameObject;
        }
    }
}
