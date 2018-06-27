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
    public bool isLockedOn = false; //stay
    private bool hasUsedDoubleJump = false;
    public bool isVulnerable = true;

    float currentCamX  = 0.0f;
    float currentCamY  = 0.0f;
    [SerializeField] Transform climbingCamPoint;

    private float jumpHieght = 20;
    private float fallMultiplyer = 10f;
    private float lowJumpMultiplyer = 3f;

    [HideInInspector] public GameObject ladder = null;
    [HideInInspector] public GameObject shimyPipe = null;
    [HideInInspector] public GameObject ledge = null;

    [SerializeField] private Transform putDownPos;
    [SerializeField] private Transform feetLevel;

    private void Awake(){
        #region Singleton
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
        #endregion

        Cursor.lockState = CursorLockMode.Locked;
    }

}
