using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//this class will handel all player actions
	private PlayerManager pManager;
	private PlayerMovement pMove;
    private FreeClimb freeClimb;
    private PlayerTraversal pTraverse;
	private PlayerAttack pAttack;
    private PlayerMenu pMenu;
	private PlayerInventory pInv;
	private ThirdPersonCamera pCamera;
    private Rigidbody rb;
	private Animator anim;

	[SerializeField] 
    private PlayerTargeting pTargeting;
    public PlayerTargeting PTargeting{
        get{return pTargeting;}
    }

	private float speed = 6.5f;

	//JUMP VARS
	private float jumpHieght = 30;
    private float timeSinceGrounded;

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

    Vector3 dir = new Vector3();

	void Start () {
		pManager = PlayerManager.instance;
		pMove = GetComponent<PlayerMovement>();
        freeClimb = GetComponent<FreeClimb>();
        pTraverse = GetComponent<PlayerTraversal>();
		pAttack = GetComponent<PlayerAttack>();
        pMenu = GetComponent<PlayerMenu>();
		pInv = GetComponent<PlayerInventory>();
		pCamera = Camera.main.GetComponent<ThirdPersonCamera>();
		anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        timeSinceGrounded = Time.time;
	}
	
	void Update () {
		AttackInput();
        BlockingInput();
		LockOnInput();
		InteractInput();
        EquipmentInput();
        SetGroundShadow();
        MenuInput();
        PlatFormingInput(dir);
	}

    private void LateUpdate(){
		CamerInput();
    }

	private void FixedUpdate(){
        MoveInput();
		CheckGrounded();
	}

	private void MoveInput(){
		float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(h,0,v);
        dir = moveDir;

        if(freeClimb.isClimbing){
            moveDir = new Vector3(0, 0, 0);
            anim.SetFloat("velocityY", Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.z)));
            return;
        }

        if(pAttack.IsAttacking){
            moveDir = new Vector3(0, 0, 0);
            anim.SetFloat("velocityY", Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.z)));
            return;
        }else if(ladder && pManager.currentState == PlayerManager.PlayerState.Traversing){
            pTraverse.ClimbLadder(moveDir, ladder);
        }else if(pManager.currentState == PlayerManager.PlayerState.Traversing && ledge){
            pTraverse.ShimyLedge(moveDir, ledge.transform);
        }else if(pManager.currentState == PlayerManager.PlayerState.FreeMovement && moveDir != Vector3.zero && pManager.isVulnerable){
            pMove.FreeMovement(moveDir, speed);
            pMove.AnimatePlayerWalking(moveDir);
        }else if(moveDir == Vector3.zero && CheckGrounded()){
            anim.SetFloat("velocityY", Mathf.Lerp(anim.GetFloat("velocityY"), 0, .2f));
            anim.SetFloat("velocityX", Mathf.Lerp(anim.GetFloat("velocityX"), 0, .2f));
            if(pManager.isVulnerable && pManager.currentState != PlayerManager.PlayerState.Attacking)
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
	}

	private void PlatFormingInput(Vector3 moveDir = new Vector3()){
		if (Input.GetButtonDown("Jump")){
            if(pManager.currentState != PlayerManager.PlayerState.Traversing &&
                pManager.currentState != PlayerManager.PlayerState.Attacking){
                if (ladder && CheckGrounded()){
                    pMove.StartCoroutine(pTraverse.LadderStart(ladder));
                }else if(shimyPipe && CheckGrounded()){
                    pMove.StartCoroutine(pTraverse.ShimyPipeStart(shimyPipe));
                }else if(freeClimb.CheckForClimb()){
                    return;
                }else if(CheckGrounded() /*|| Time.time - timeSinceGrounded < .2f*/){
                    timeSinceGrounded = Time.time;
                    pMove.Jump(jumpHieght, dir); //maybe remove standard jump mech
                }else if(!CheckGrounded()){
                   pTraverse.WallJump(jumpHieght);
                }
            }else{
                if(shimyPipe){
                    pTraverse.Drop();
                }else if(ladder){
                    pTraverse.LadderEnd();
                    pMove.Jump(jumpHieght);
                }else if(ledge && Input.GetAxisRaw("Vertical") > 0){
                   pMove.StartCoroutine(pTraverse.MoveToNextLedge());
                }else if(ledge && Input.GetAxisRaw("Vertical") < 0){
                    pTraverse.Drop();
                }else if(ledge){
                   pTraverse.WallJump(jumpHieght);
                }
            }
        }else if(Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.E)){
            if(CheckGrounded()){
                pMove.Evade(moveDir);
            }
        }
	}

	private void AttackInput(){
		if(pManager.currentState == PlayerManager.PlayerState.FreeMovement || pManager.currentState == PlayerManager.PlayerState.Attacking){
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("XButton")){//X button or click
                if(CheckGrounded() && !pManager.IsPaused){
                    pAttack.Attack();
                }
            }
        }

        if(pManager.currentState == PlayerManager.PlayerState.Attacking && pManager.isLockedOn){
            pMove.LookAtTarget(pTargeting.currentTarget.transform);
        }
	}

    private void BlockingInput(){
        if(Input.GetMouseButton(1) && pManager.currentState == PlayerManager.PlayerState.FreeMovement){
            anim.SetBool("IsBlocking", true);
            pManager.isBlocking = true;
        }else{
            anim.SetBool("IsBlocking", false);
            pManager.isBlocking = false;
            anim.SetFloat("velocityX", 0);
        }
    }
	
	private void CamerInput(){
        if(!pManager.IsPaused){
            if(!pManager.isLockedOn){
                currentCamX += Input.GetAxisRaw("Mouse X") * 2;
                currentCamY += Input.GetAxisRaw("Mouse Y") * 2; 
                pCamera.MouseOrbit(currentCamX, currentCamY );
            }else{
                pCamera.LockedOnCam();
            }	
        }
	}

	private void LockOnInput(){
		if(Input.GetMouseButtonDown(2) || Input.GetButtonDown("RightJoyStick")){
            pTargeting.ToggleLockedOnEnemies();
            if(!pInv.Equipped){
                pInv.EquipWeapons();
            }
        }

        if(Input.GetKeyDown(KeyCode.Tab)){
            pTargeting.LockOff();
        }

        pTargeting.transform.position = transform.position;
	}

    private void EquipmentInput(){
        if(Input.GetKeyDown(KeyCode.G) && !pManager.isLockedOn && pManager.currentState != PlayerManager.PlayerState.Attacking){
            pInv.EquipWeapons();
        }
    }

	private void InteractInput(){
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
                pInv.AddItem(item.GetComponent<Item>());
                anim.SetTrigger("PickUpItem");
				item = null; 
            }else if(pushBlock && isPushingBlock){
                isPushingBlock = false;
                pushBlock.transform.SetParent(null);
            }
        }
	}

	private void MenuInput(){
        if(Input.GetKeyDown(KeyCode.M) || Input.GetButtonDown("Start")){
            pMenu.OpenClosePlayerMenu();
        }

		if(Input.GetKeyDown(KeyCode.I)){
			pInv.OpenCloseInventory();
		}

        if(Input.GetKeyDown(KeyCode.Escape)){
            pMenu.CloseAllWindows();
        }

        if(Input.GetButtonDown("BButton")){
            pMenu.Back();
        }
	}

	public bool CheckGrounded(){
        if(pManager.currentState == PlayerManager.PlayerState.Attacking)
            return true;

        RaycastHit hit;
        if(Physics.Raycast(feetLevel.position, -Vector3.up, out hit, 0.25f)){
            timeSinceGrounded = Time.time;
            anim.SetBool("isGrounded", true);

            if(hit.transform.tag == "Platform"){
                transform.SetParent(hit.transform);
            }else{
                transform.SetParent(null);
            }

            anim.applyRootMotion = true;
            return true;

        }else{
            if(pManager.currentState != PlayerManager.PlayerState.FreeClimbing)
                anim.SetBool("isGrounded", false);
                
            transform.SetParent(null);
            anim.applyRootMotion = false;
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
            tp.y = hit.point.y + 0.05f;
            shadow.transform.position = Vector3.Lerp(shadow.transform.position, tp, 1f);
        }else{
            shadow.SetActive(false);
        }
    }

    private void PickUpKey(GameObject key){
        GameManager.instance.gameLevels[Array.IndexOf(GameManager.instance.gameLevels, GameManager.instance.CurrentLevel)].PickUpKey(key);
    }
    
	private void OnTriggerEnter(Collider other){
        if(other.tag == "PickUp"){
            pickUpObject = other.gameObject;
        }else if(other.tag == "Item"){
            item = other.gameObject;
        }else if(other.tag == "WarpPad"){
            pMove.StartCoroutine(pTraverse.Warp(other.gameObject));
        }else if(other.tag == "Ledge"){
            pTraverse.StartCoroutine(pTraverse.GrabLedge(other.gameObject));
        }else if(other.tag == "Ladder"){
			ladder = other.gameObject;
		}else if(other.tag == "ShimyPipe"){
            shimyPipe = other.gameObject;
        }else if(other.tag == "DungeonKey"){
            PickUpKey(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "PickUp" && !isHoldingObject){
            pickUpObject = null;
        }else if(other.tag == "Item"){
            item = null;
        }else if(other.tag == "Ladder"){
			ladder = null;
		}else if(other.tag == "ShimyPipe"){
            shimyPipe = null;
        }else if(other.tag == "Ledge"){
            ledge = null;
            pTraverse.Drop();
        }
        else if(other.gameObject.name == "Camera Distance Changer"){
            Debug.Log("hsfsdf");
        } 
    }
}
