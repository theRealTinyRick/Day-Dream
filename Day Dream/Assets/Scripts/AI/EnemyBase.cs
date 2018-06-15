using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour {

	private enum EnemyType { GeneralMelee, ZombieMelee, GeneralRanged, Boss};
    [SerializeField] private EnemyType thisType = EnemyType.GeneralMelee;
    
    [HideInInspector] public enum State { Walking, Stunned, Attacking, Dead};
    public State currentState = State.Walking;

    [SerializeField] private float startingHealth;
    [SerializeField] private float currentHealth;
    [SerializeField] private float moveSpeed;

    public float attackRange;
    public float mobRange;
    public float aggroRange;
    [SerializeField] private float heightDiff;

    [HideInInspector] public Animator anim;
    [HideInInspector] public NavMeshAgent nav;

    public GameObject Player { get; set; }
    public GameObject CurrentTarget { get; set; }
    private Coroutine pathFinding;

    private void Start(){
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = moveSpeed;
        Player = PlayerManager.instance.gameObject;
        if (CurrentTarget == null)
            CurrentTarget = Player;

        currentHealth = startingHealth;
        StartCoroutine(PathFinding());
    }

    private IEnumerator PathFinding(){
        while (currentState != State.Dead){
            while (!CheckAggro() || !CheckFieldOfView(Player.transform.position)){
                yield return new WaitForEndOfFrame();
            }
            nav.SetDestination(CurrentTarget.transform.position);
            if(currentState != State.Attacking || currentState != State.Stunned){
                if (CheckRange(attackRange, CurrentTarget.transform.position)){
                    nav.isStopped = true;
                    anim.SetBool("isMoving", false);
                    if(currentState == State.Walking)
                        RotateTowardsPlayer(Player.transform.position);
                }
                else{
                    nav.isStopped = false;
                    anim.SetBool("isMoving", true);
                }

                if (!CheckAggro()){
                    nav.isStopped = true;
                    anim.SetBool("isMoving", false);
                    pathFinding = StartCoroutine(PathFinding());
                }
            }else{
                nav.isStopped = true;
                anim.SetBool("isMoving", false);
            }
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    //utility functions
    public bool CheckAggro(){
        if (CheckRange(aggroRange, Player.transform.position) 
            && CheckLineOfSight(Player.transform.position)
            &&CheckHieghtDifferential(Player.transform.position))
            return true;
        return false;
    }

    public bool CheckRange(float range, Vector3 toPos){
        if (Vector3.Distance(transform.position, toPos) <= range)
            return true;
        return false;
    }

    private bool CheckHieghtDifferential(Vector3 toPos){
        //this function check the level the player might be standing on such as a cliff. if the player is above or below the line of sight the enemy will not see the player
        float toY = toPos.y;
        float fromY = transform.position.y;
        float diff = Mathf.Abs(toY - fromY);

        if (diff > heightDiff)
            return false;
        return true;
    }

    private bool CheckFieldOfView(Vector3 toPos){
        Vector3 tp = transform.position - toPos;
        Transform fromPos = transform;

        float angle = Vector3.Angle(fromPos.forward, -tp);
        if (angle <= 75f){
            return true;
        }
        return false;
    }

    private bool CheckLineOfSight(Vector3 toPos){
        Vector3 adjustedToPos = toPos;
        adjustedToPos.y = toPos.y + 1; //do this so the ray isnt going through the ground
        Vector3 adjustedFrom = transform.position;
        adjustedFrom.y = transform.position.y + 1;

        Vector3 dir = adjustedToPos - adjustedFrom;
        RaycastHit hit;
        if (Physics.Raycast(adjustedFrom, dir, out hit, 100f)){
            if(hit.transform.tag == "Player")
                return true;
            else if(hit.transform.tag == "Grass"){
                Debug.Log("Hidden");
                return false;
            }
            else
                return false;
        }
        return false;
    }

    private void RotateTowardsPlayer(Vector3 playerPos){
        Vector3 dir = playerPos-transform.position;
        dir.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, 0.2f);
    }

    //anim events below this line ???????///////////////////////////////////////////////////////////////

    void MeleeStart(){

    }

    void MeleeEnd(){

    }

    void ProjectileStart(){

    }

    void ProjectileEnd(){

    }
}
