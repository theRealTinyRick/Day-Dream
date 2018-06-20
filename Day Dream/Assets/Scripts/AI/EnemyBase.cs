﻿using System;
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
    private float heightDiff = 2;

    public bool isAggro = false;

    [HideInInspector] public Animator anim;
    [HideInInspector] public NavMeshAgent nav;

    public Transform currentPoint;
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] private float switchTime;

    private void Awake(){
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = moveSpeed;
        currentHealth = startingHealth;
        anim.speed = .75f;

        StartCoroutine(SwitchPatrolPoints());
    }

    private void Update(){
        CheckAggro();
        PathFinding();
    }

    private void PathFinding(){
        if(currentState != State.Dead && isAggro){
            if(currentState != State.Attacking && currentState != State.Stunned && !CheckRange(attackRange, PlayerManager.instance.transform.position)){
                nav.SetDestination(PlayerManager.instance.transform.position);
                nav.isStopped = false;
                anim.SetBool("isMoving", true);
                if(CheckLineOfSight(PlayerManager.instance.transform.position)){
                    RotateTowardsPlayer(PlayerManager.instance.transform.position);
                }
            }else{
                nav.isStopped = true;
                anim.SetBool("isMoving", false);
                if(currentState != State.Attacking && currentState != State.Stunned)
                    RotateTowardsPlayer(PlayerManager.instance.transform.position);
            }
        }
        else{/////////////////PATROLING
            // if(patrolPoints.Length > 0){
            //     if(!CheckRange(.1f, currentPoint.position)){
            //         nav.SetDestination(currentPoint.position);
            //         nav.isStopped = false;
            //         anim.SetBool("isMoving", true);
            //     }else{
            //         nav.isStopped = true;
            //         anim.SetBool("isMoving", false);
            //     }
            // }
        }
    }

    private IEnumerator SwitchPatrolPoints(){
        if(patrolPoints.Length > 0){
            currentPoint = patrolPoints[0];
            while(!isAggro){
                int index = Array.IndexOf(patrolPoints, currentPoint);
                if(index < (patrolPoints.Length - 1)){
                    currentPoint = patrolPoints[++index];
                }else{
                    currentPoint = patrolPoints[0];
                }
                yield return new WaitForSeconds(switchTime);
            }
        }
        yield return null;
    }

    //utility functions
    public bool CheckAggro(){
        if(!isAggro){
            if (CheckRange(aggroRange, PlayerManager.instance.transform.position) && CheckLineOfSight(PlayerManager.instance.transform.position) && CheckHieghtDifferential(PlayerManager.instance.transform.position) 
            && CheckFieldOfView(PlayerManager.instance.transform.position)){
                isAggro = true;
            }
        }else{
            if (CheckRange(aggroRange, PlayerManager.instance.transform.position) && CheckLineOfSight(PlayerManager.instance.transform.position) 
            && CheckHieghtDifferential(PlayerManager.instance.transform.position)){
                isAggro = false ;
            }
        }
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
        if (angle <= 60f){
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
            if(hit.transform.tag == "player")
                return true;
            else if(hit.transform.tag == "Grass" || hit.transform.tag == "Environment"){
                return false;
            }else
                return true;
        }
        return false;
    }

    private void RotateTowardsPlayer(Vector3 playerPos){
        Vector3 dir = playerPos-transform.position;
        dir.y = transform.localPosition.y;
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
