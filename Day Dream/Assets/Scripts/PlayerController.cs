using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//this class will handel all player actions
	private PlayerManager pManager;
	private PlayerMovement pMove;
	private PlayerAttack pAttack;
	private PlayerInventory pInv;
	[SerializeField] private PlayerTargeting pTargeting;
	private ThirdPersonCamera pCamera;
	public Animator anim{get; set;}
    private Rigidbody rb;

	private float speed = 8;

	//JUMP VARS
	private float jumpHieght = 20;
	private bool hasUsedDoubleJump = false;

	//PLATFORMS
	private GameObject ladder = null;
    private GameObject shimyPipe = null;
    public GameObject ledge = null;

	// 
	private bool isHoldingObject;
    private GameObject pickUpObject = null;
    public GameObject item;
    public GameObject pushBlock;
    public bool isPushingBlock;

	//Camera
	private float currentCamX  = 0.0f;
    private float currentCamY  = 0.0f;

    [SerializeField] private Transform putDownPos;
    [SerializeField] private Transform feetLevel;

	[SerializeField] Transform climbingCamPoint;
    [SerializeField] GameObject shadow;
    //UI Elements
	[SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject Inventory;

	void Start () {
		pManager = PlayerManager.instance;
		pMove = GetComponent<PlayerMovement>();
		pAttack = GetComponent<PlayerAttack>();
		pInv = GetComponent<PlayerInventory>();
		pCamera = Camera.main.GetComponent<ThirdPersonCamera>();
		anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		PlatFormingInput();
		AttackInput();
		LockOnInput();
		InteractInput();
        SetGroundShadow();
	}

    private void LateUpdate(){
		CamerInput();
        // SetGroundShadow();
    }

	private void FixedUpdate(){
		CheckGrounded();
        // SetGroundShadow();
		MoveInput();
	}

	private void MoveInput(){
		float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(h,0,v);

		if(pManager.currentState == PlayerManager.PlayerState.Traversing && shimyPipe){
            pMove.ShimyPipe(moveDir, shimyPipe.GetComponent<ShimyPipe>());
        }else if(ladder && pManager.currentState == PlayerManager.PlayerState.Traversing){
            pMove.ClimbLadder(moveDir, ladder);
        }else if(pManager.currentState == PlayerManager.PlayerState.Traversing && ledge){
            pMove.ShimyLedge(moveDir, ledge.transform);
        }else if(pManager.currentState == PlayerManager.PlayerState.FreeMovement && moveDir != Vector3.zero){
            pMove.FreeMovement(moveDir, speed);
        }else if(moveDir == Vector3.zero && CheckGrounded()){
            anim.SetBool("isMoving", false);
            rb.velocity = new Vector3(0, rb.velocity.y, 0); //stop the player from sliding on platforms
        }
        
	}

	private void PlatFormingInput(){
		if (Input.GetButtonDown("Jump")){
            if(pManager.currentState != PlayerManager.PlayerState.Traversing){
                if (ladder && CheckGrounded()){
                    pMove.StartCoroutine(pMove.LadderStart(ladder));
                }else if(shimyPipe && CheckGrounded()){
                    pMove.StartCoroutine(pMove.ShimyPipeStart(shimyPipe));
                }else if(ledge){
                    if(!pMove.CanGrabLedge(ledge) && CheckGrounded()){
                        pMove.Jump(jumpHieght); //maybe remove standard jump mech
                    }
                }else if(CheckGrounded() && pManager.isLockedOn){
                    pMove.Evade(jumpHieght);
                }else if(CheckGrounded()){
                    pMove.Jump(jumpHieght); //maybe remove standard jump mech
                }else if(!CheckGrounded() && !hasUsedDoubleJump){
                    hasUsedDoubleJump = true;
                    pMove.Jump(jumpHieght - (jumpHieght/4)); //maybe remove standard jump mech
                }
            }else{
                if(shimyPipe){
                    pMove.EndShimy();
                }else if(ladder){
                    pMove.LadderEnd();
                    pMove.Jump(jumpHieght);
                }else if(ledge){
                    pMove.DropLedge();
                }
            }
        }
	}

	private void AttackInput(){
		if(pManager.currentState == PlayerManager.PlayerState.FreeMovement || pManager.currentState == PlayerManager.PlayerState.Attacking){
            if (Input.GetMouseButtonDown(0) && CheckGrounded())
                pAttack.Attack();
        }
        if(pManager.currentState == PlayerManager.PlayerState.Attacking && pManager.isLockedOn){
            pMove.LookAtTarget(pTargeting.currentTarget.transform);
        }
	}
	
	private void CamerInput(){
	    if(ladder && pManager.currentState == PlayerManager.PlayerState.Traversing){
            // pCamera.ClimbingCamera();
            currentCamX += Input.GetAxisRaw("Mouse X") * 2;
            currentCamY += Input.GetAxisRaw("Mouse Y") * 2; 
            pCamera.MouseOrbit(currentCamX, currentCamY );
        }else if(!pManager.isLockedOn){
            currentCamX += Input.GetAxisRaw("Mouse X") * 2;
            currentCamY += Input.GetAxisRaw("Mouse Y") * 2; 
            pCamera.MouseOrbit(currentCamX, currentCamY );
        }else{
            pCamera.LockedOnCam();
        }	
	}

	private void LockOnInput(){
		if(Input.GetMouseButtonDown(2)){
            pTargeting.ToggleLockedOnEnemies();
        }

        if(Input.GetKeyDown(KeyCode.Tab)){
            pTargeting.LockOff();
        }

        pTargeting.transform.position = transform.position;
	}

	private void SkillInput(){

	}

	private void InteractInput(){
		if(Input.GetKeyDown(KeyCode.F)){
            if(pickUpObject && isHoldingObject){
                //drop
                isHoldingObject = false;
                //StartCoroutine(PutDownObject());
            }else if(pickUpObject && !isHoldingObject){
                //pickup
                isHoldingObject = true;
                //StartCoroutine(PickUpObject());
            }else if(item){
                pInv.AddItem(item.GetComponent<Item>());
				item = null; 
            }else if(pushBlock && isPushingBlock){
                isPushingBlock = false;
                pushBlock.transform.SetParent(null);
            }
        }
	}

	private void MenuInput(){
		if(Input.GetKeyDown(KeyCode.I)){
			//open and close inventory
			Inventory.SetActive(!Inventory.activeInHierarchy);
			if(!Inventory.activeInHierarchy){
				pInv.ClearList();
			}else{
				pInv.RenderList();
			}
		}
	}

	public bool CheckGrounded(){
        RaycastHit hit;
        if(Physics.Raycast(feetLevel.position,-Vector3.up, out hit, 0.5f)){
			if(Vector3.Distance(hit.point, feetLevel.position) <= .2f){
				anim.SetBool("isGrounded", true);
				hasUsedDoubleJump = false;

				if(hit.transform.tag == "Platform"){
					transform.SetParent(hit.transform);
				}else{
					transform.SetParent(null);
				}
			}else{
				transform.SetParent(null);
			}
            return true;
        }else{
            anim.SetBool("isGrounded", false);
			transform.SetParent(null);
            return false;
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

    private void SetGroundShadow(){
        RaycastHit hit; 
        if(Physics.Raycast(feetLevel.position, -Vector3.up, out hit, 100)){
            shadow.SetActive(true);
            Vector3 tp = hit.point;
            tp.y = hit.point.y + 0.1f;
            shadow.transform.position = Vector3.Lerp(shadow.transform.position, tp, 1f);
        }else{
            shadow.SetActive(false);
        }
    }
	
	private void OnTriggerEnter(Collider other){
        if(other.tag == "PickUp"){
            pickUpObject = other.gameObject;
        }else if(other.tag == "Item"){
            item = other.gameObject;
        }else if(other.tag == "WarpPad"){
            pMove.StartCoroutine(pMove.Warp(other.gameObject));
        }else if(other.tag == "Ledge"){
            ledge = other.gameObject;
        }else if(other.tag == "Ladder"){
			ladder = other.gameObject;
		}else if(other.tag == "ShimyPipe"){
            shimyPipe = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "PickUp" && !isHoldingObject){
            pickUpObject = null;
        }else if(other.tag == "Item"){
            item = null;
        }else if(other.tag == "Ledge"){
           	if(ledge && pManager.currentState == PlayerManager.PlayerState.Traversing){
               pMove.DropLedge();
           	}
        	ledge = null;
        }else if(other.tag == "Ladder"){
			ladder = null;
		}else if(other.tag == "ShimyPipe"){
            shimyPipe = null;
        }else if(other.gameObject.name == "Camera Distance Changer"){
            Debug.Log("hsfsdf");
        }   
    }
}
