using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//this class will handel all player actions
	private PlayerManager pManager;
	private PlayerMovement pMove;
    private FreeClimb freeClimb;
    private LedgeClimb ledgeClimb;
    private WallJump wallJump;
    private ClimbLadder climbLadder;
    private PlayerTraversal pTraverse;
	private PlayerAttack pAttack;
    private PlayerBlocking pBlocking; 
    private PlayerMenu pMenu;
	private PlayerInventory pInv;
    private PlayerInteraction pInteraction;
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
	public float jumpHieght = 30;
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

    [SerializeField] private Transform putDownPos;
    [SerializeField] private Transform feetLevel;

	[SerializeField] Transform climbingCamPoint;
    [SerializeField] GameObject shadow;

    Vector3 dir = new Vector3();

	void Start () {
		pManager = PlayerManager.instance;
		pMove = GetComponent<PlayerMovement>();
        wallJump = GetComponent<WallJump>();
        freeClimb = GetComponent<FreeClimb>();
        ledgeClimb = GetComponent<LedgeClimb>();
        climbLadder = GetComponent<ClimbLadder>();
        pTraverse = GetComponent<PlayerTraversal>();
		pAttack = GetComponent<PlayerAttack>();
        pBlocking = GetComponent<PlayerBlocking>();
        pMenu = GetComponent<PlayerMenu>();
		pInv = GetComponent<PlayerInventory>();
        pInteraction = GetComponent<PlayerInteraction>();
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
	}

	private void MoveInput(){
		float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(h,0,v);
        dir = moveDir;

        if(freeClimb.isClimbing || ledgeClimb.IsClimbing){
            moveDir = new Vector3(0, 0, 0);
            anim.SetFloat("velocityY", Mathf.Max(Mathf.Abs(0), Mathf.Abs(0)));
            return;
        }
        
        if(pAttack.IsAttacking){
            // moveDir = new Vector3(0, 0, 0);
            anim.SetFloat("velocityY", Mathf.Max(Mathf.Abs(0), Mathf.Abs(0)));
            return;

        }else if(climbLadder.IsClimbing){
            climbLadder.Tick(moveDir.z);
            return;

        }else if(pManager.currentState == PlayerManager.PlayerState.Traversing && ledge){
            pTraverse.ShimyLedge(moveDir, ledge.transform);

        }else if(pManager.currentState == PlayerManager.PlayerState.FreeMovement && moveDir != Vector3.zero && pManager.isVulnerable){
            pMove.FreeMovement(moveDir, speed);
            pMove.AnimatePlayerWalking(moveDir);

        }else if(moveDir == Vector3.zero && CheckGrounded()){
            anim.SetFloat("velocityY", Mathf.Lerp(anim.GetFloat("velocityY"), 0, .2f));
            anim.SetFloat("velocityX", Mathf.Lerp(anim.GetFloat("velocityX"), 0, .2f));

            if(pManager.isVulnerable && pManager.currentState != PlayerManager.PlayerState.Attacking){
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }
        }
	}

	private void PlatFormingInput(Vector3 moveDir = new Vector3()){
		if (Input.GetButtonDown("Jump")){
            if(!freeClimb.isClimbing && !ledgeClimb.IsClimbing){
                if(freeClimb.CheckForClimb()){
                    return;

                }else if(climbLadder.CheckForClimb(pManager)){
                    return;

                }else if(CheckGrounded() && pManager.currentState != PlayerManager.PlayerState.Traversing){
                    pMove.Jump(jumpHieght);
                    return;

                }else if(!CheckGrounded() && wallJump.CheckWallJump(jumpHieght - 2)){
                    return;
                }
            }else if(freeClimb.isClimbing){
                if(dir.z == 0){
                    freeClimb.Drop();
                    wallJump.CheckWallJump(jumpHieght);
                }else if(dir.z < 0){
                    freeClimb.Drop();
                }

            }else if(ledgeClimb.IsClimbing){
                if(dir.z == 0){
                    ledgeClimb.Drop();
                    wallJump.CheckWallJump(jumpHieght);
                }else if(dir.z < 0){
                    ledgeClimb.Drop();
                }else if(dir.z > 0){
                    ledgeClimb.Drop();
                    pMove.Jump(jumpHieght);
                }
            }

        }else if(Input.GetButtonDown("BButton") || Input.GetKeyDown(KeyCode.E)){
            if(CheckGrounded()){
                pMove.Evade(moveDir);
            }
        }
	}

	private void AttackInput(){
		if(pManager.currentState == PlayerManager.PlayerState.FreeMovement){
            if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("XButton")){//X button or click
                if(CheckGrounded() && !pManager.IsPaused){
                    pAttack.Attack();
                }
            }
        }

        if(pAttack.IsAttacking && pManager.isLockedOn){
            pMove.LookAtTarget(pTargeting.currentTarget.transform);
        }
	}

    private void BlockingInput(){
        if(Input.GetMouseButton(1) && pManager.currentState == PlayerManager.PlayerState.FreeMovement &&
        !pAttack.IsAttacking){
            pBlocking.SetBlocking(true);
        }else{
            pBlocking.SetBlocking(false);
        }
    }
	
	private void CamerInput(){
        if(!pManager.IsPaused){
            if(!pManager.isLockedOn){
                float h = Input.GetAxis("Mouse X");
                float v = Input.GetAxis("Mouse Y"); 
                pCamera.MouseOrbit(h, v );
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
            pInteraction.InitPickUp();
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
        if(Physics.Raycast(feetLevel.position, -Vector3.up, out hit, 0.1f)){
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
}
