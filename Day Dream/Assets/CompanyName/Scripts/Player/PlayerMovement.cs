using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerMovement : MonoBehaviour {

    private const string velocityX = "velocityX";
    private const string velocityY = "velocityY";
    
    [TabGroup("Preferences")]
    [SerializeField] 
    private float timeTillLand;
    
    [TabGroup("Preferences")]
    [SerializeField] 
    private float timeTillRoll;    
    
    [TabGroup("Preferences")]
    [SerializeField] 
    [Range(0, 1)]
    private float turnSpeed;
    
    [TabGroup("Set Up")]
    [SerializeField]
    private PlayerTargeting pTargeting;

    private float fallMultiplyer = 10f;
    private float lowJumpMultiplyer = 3f;
    private float timeSinceJump = 0.0f;
    
    private Rigidbody rb;
    private Animator anim;
    private GameObject pCamera;
    private PlayerManager pManager;
    private PlayerController pController;

    private void Start(){
        pManager = PlayerManager.instance;
        pController = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        pCamera = Camera.main.gameObject;
    }

    private void FixedUpdate(){
        BetterJumpPhysics();
    }

    public void FreeMovement(Vector3 movement, float speed){
        if (PlayerManager.currentState != PlayerManager.PlayerState.Attacking){
           
            if(pManager.isLockedOn && pManager.isBlocking){
                speed = speed/2.5f;
            }

            Vector3 move = movement;

            speed *= move.magnitude;

            movement = pCamera.transform.TransformDirection(movement);
            movement.y = 0;
            movement = Vector3.Normalize(movement);

            rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);

            if(pController.CheckGrounded() && movement != Vector3.zero){
                if(!pManager.isLockedOn || !pManager.isBlocking){
                    // float turnSpeed = Mathf.Max(Mathf.Abs(movement.x), Mathf.Abs(movement.z));
                    // if(turnSpeed > .5f){
                    //     turnSpeed = 0.5f;
                    // }
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), turnSpeed);
                }
            }

            AnimatePlayerWalking(movement, move);
        }
    }

    public void AnimatePlayerWalking(Vector3 moveDir, Vector3 move){
        Vector3 toVector = transform.position + moveDir;
        toVector -= transform.position;

        Vector3 forward = transform.forward;
        Vector3 crossProduct = Vector3.Cross(forward, toVector);
        
        var origin = transform.position;
        origin.y += 1;

        anim.SetFloat(velocityY, move.magnitude);
        anim.SetFloat(velocityX, crossProduct.y);

        Debug.DrawRay(origin, toVector, Color.red);
        Debug.DrawRay(origin, forward, Color.magenta);
    }

    public void LookAtTarget(Transform target){
        Vector3 tp = target.position;
        tp.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(tp - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, .2f);
    }

    public void Jump(float jumpHeight, Vector3 dir = new Vector3()){

        dir = pCamera.transform.TransformDirection(dir);
        dir.y = 0;
        dir.Normalize();

        Vector3 v = new Vector3(0, jumpHeight, 0) + (dir * jumpHeight);

        rb.velocity = v;
        anim.SetBool("isGrounded", false);
        anim.Play("Jump");
    }

    public void Evade(Vector3 dir = new Vector3()){
        if(pManager.isVulnerable){
            if(dir == Vector3.zero){
                if(pManager.isLockedOn){
                    Vector3 tp = pTargeting.currentTarget.transform.position;
                    tp.y = transform.position.y;

                    transform.LookAt(tp);
                }
                anim.Play("StepBack");
            }else{
                anim.Play("Roll");
            }
            
            pManager.StartCoroutine(pManager.Invulnerabe());
        }
    }


    private void BetterJumpPhysics(){
        if (rb.velocity.y  < 0) 
            rb.velocity += Vector3.up *  Physics.gravity.y  * (fallMultiplyer - 1) * Time.deltaTime;
        else if (rb.velocity.y  > 0 /*&& !Input.GetButton("Jump")*/)
            rb.velocity += Vector3.up * Physics.gravity.y *2 * Time.deltaTime;
    }

    public void JumpTime(){
        timeSinceJump = Time.time;
    }

    public void Land(){
        float t = Time.time - timeSinceJump;

        if(t > timeTillRoll){
            //roll
        }else if(t > timeTillLand){
            //land
        }
    }
}
