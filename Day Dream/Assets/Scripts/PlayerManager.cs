using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    
    private PlayerMovement move;
    private PlayerAttack atk;
    private PlayerInventory inv;
    public PlayerTargeting targeting;
    private Rigidbody rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public ThirdPersonCamera playerCam;

    public enum PlayerState { FreeMovement, CanNotMove, Traversing, Attacking, Blocking, Dead};
    public PlayerState currentState = PlayerState.FreeMovement;

    [SerializeField] private float speed = 5;
    private Vector3 movement = Vector3.zero;
    public bool isLockedOn = false;
    private bool hasUsedDoubleJump = false;
    public bool isVulnerable = true;

    float currentCamX  = 0.0f;
    float currentCamY  = 0.0f;
    [SerializeField] Transform climbingCamPoint;

    public int coinCount = 0;

    [SerializeField] private float jumpHieght;
    [SerializeField] private float fallMultiplyer = 2.5f;
    [SerializeField] private float lowJumpMultiplyer = 3f;

     public GameObject ladder = null;
    [HideInInspector] public GameObject shimyPipe = null;
    [SerializeField] private Transform putDownPos;
    [SerializeField] private Transform feetLevel;

    private bool isHoldingObject;
    private GameObject pickUpObject = null;
    public GameObject item;
    public GameObject pushBlock;
    public bool isPushingBlock;

    //Menu Items
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject Inventory;

    private void Awake(){
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        #endregion

        move = GetComponent<PlayerMovement>();
        atk = GetComponent<PlayerAttack>();
        inv = GetComponent<PlayerInventory>();
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
        MenuInput();
        LockOnInput();
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
        
        //Jump - climb and rolls
        if (Input.GetKeyDown(KeyCode.Space)){
            if(currentState != PlayerState.Traversing){
                if (ladder && CheckGrounded()){
                    move.StartCoroutine(move.LadderStart(ladder));
                }else if(shimyPipe && CheckGrounded()){
                    move.StartCoroutine(move.ShimyPipeStart(shimyPipe));
                }else if(CheckGrounded() && isLockedOn){
                    move.Evade(jumpHieght);
                }else if(CheckGrounded()){
                    move.Jump(jumpHieght); //maybe remove standard jump mech
                }else if(!CheckGrounded() && !hasUsedDoubleJump){
                    hasUsedDoubleJump = true;
                    move.Jump(jumpHieght - (jumpHieght/4)); //maybe remove standard jump mech
                }
            }else{
                if(shimyPipe){
                    move.EndShimy();
                }else if(ladder){
                    move.LadderEnd();
                    move.Jump(jumpHieght);
                }
            }
        }
    }

    private void ApplyMove(){
        //apply move controls
        if(currentState == PlayerState.Traversing && shimyPipe){
            move.ShimyPipe(movement, shimyPipe.GetComponent<ShimyPipe>());
        }else if(ladder && currentState == PlayerState.Traversing && currentState != PlayerState.CanNotMove){
            move.ClimbLadder(movement, ladder);
        }else if(pushBlock && isPushingBlock){
            move.MoveBlock(movement);
        }else if(currentState != PlayerState.Traversing && movement != Vector3.zero && currentState != PlayerState.CanNotMove){
            move.FreeMovement(movement, speed);
        }else if(movement == Vector3.zero)
            anim.SetBool("isMoving", false);
    }

    void CameraInput(){
        if(ladder && currentState == PlayerState.Traversing){
            playerCam.ClimbingCamera(climbingCamPoint.position);
        }else if(!isLockedOn){
            currentCamX += Input.GetAxis("Mouse X");
            currentCamY += Input.GetAxis("Mouse Y"); 
            playerCam.MouseOrbit(currentCamX, currentCamY );
        }else{
            playerCam.LockedOnCam();
        }
    }

    void AttackInput(){
        if(currentState == PlayerState.FreeMovement || currentState == PlayerState.Attacking){
            if (Input.GetMouseButtonDown(0) && CheckGrounded())
                atk.Attack();
        }
        if(currentState == PlayerState.Attacking && isLockedOn){
            move.LookAtTarget(targeting.currentTarget.transform);
        }
    }

    private void LockOnInput(){
        if(Input.GetMouseButtonDown(2)){
            targeting.ToggleLockedOnEnemies();
        }
        targeting.transform.position = transform.position;
    }

    public bool CheckGrounded(){
        RaycastHit hit;
        if(Physics.Raycast(feetLevel.position,-Vector3.up, out hit, 0.1f)){
            anim.SetBool("isGrounded", true);
            hasUsedDoubleJump = false;
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
                StartCoroutine(PutDownObject());
            }else if(pickUpObject && !isHoldingObject){
                //pickup
                isHoldingObject = true;
                StartCoroutine(PickUpObject());
            }else if(item){
                inv.AddItem(item);
            }else if(pushBlock && !isPushingBlock){
                isPushingBlock = true;
                transform.LookAt(pushBlock.transform.position);
            }else if(pushBlock && isPushingBlock){
                isPushingBlock = false;
                pushBlock.transform.SetParent(null);
            }
        }
    }

    private IEnumerator PickUpObject(){
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

        yield return null;
    }

    private IEnumerator PutDownObject(){
        while(Vector3.Distance(pickUpObject.transform.position, putDownPos.position) > .1){
            pickUpObject.transform.position = Vector3.Lerp(pickUpObject.transform.position, putDownPos.position, .1f);
        }
        pickUpObject.GetComponent<Rigidbody>().isKinematic = false;
        pickUpObject.GetComponent<BoxCollider>().enabled = true;
        yield return null;
    }
    
    private void MenuInput(){
        if(Input.GetKeyDown(KeyCode.I)){
            //open and close inventory
            Inventory.SetActive(!Inventory.activeInHierarchy);
            if(!Inventory.activeInHierarchy){
                inv.ClearList();
            }else{
                inv.RenderList(inv.fullInventory);
            }
        }
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
        }else if(other.tag == "Item"){
            item = other.gameObject;
        }else if(other.tag == "WarpPad"){
            move.StartCoroutine(move.Warp(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "PickUp" && !isHoldingObject){
            pickUpObject = null;
        }else if(other.tag == "Item"){
            item = null;
        }
    }
}
