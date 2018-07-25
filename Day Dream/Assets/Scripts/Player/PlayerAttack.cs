using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    
    public bool isAttacking = false;
    public bool IsAttacking{
        get{return isAttacking;}
    }

    string[] attackAnimations = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};
    public List <string> attackQueue = new List <string>();
    public int numberOfClicks = 0;
    int maxNumberOfClicks = 0;
    private float timeToAtk = .75f;//the amount of time between clicks the player will stop attacking
    private float _time;

    private float timeToEnEquip = 5f;

    [SerializeField]
    float attackRangeMin  = 5f;

    [SerializeField]
    float attackRangeMax = 9f;

    private PlayerManager pManager;
    private PlayerController pController;
    private PlayerInventory pInventory;
    private PlayerTargeting pTargeting;
    private Animator anim;

    private void Start(){
        pController = GetComponent<PlayerController>();
        pInventory = GetComponent<PlayerInventory>();
        anim = GetComponent<Animator>();

        pManager = PlayerManager.instance;
        pTargeting = pController.PTargeting;

        maxNumberOfClicks = attackAnimations.Length;

    }

    private void Update(){
        ClickTimer();
    }

    public void ClickTimer(){
        if((Time.time - _time) > timeToAtk){
            foreach(string animation in attackAnimations){
                if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation)){
                    isAttacking = true;
                    return;
                }
            }

            foreach(string a in attackAnimations){
                anim.SetBool(a, false);
            }

            isAttacking = false;
            numberOfClicks = 0;
        }else{
            isAttacking = true;
        }
    }

    public void Attack(){
        if(!pInventory.Equipped){
            pInventory.EquipWeapons();
        }

        numberOfClicks++;
        if(numberOfClicks > maxNumberOfClicks){
            numberOfClicks = maxNumberOfClicks;
            return;   
        }

        string a = "";
        foreach(string animation in attackAnimations){
            if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation)){
                a = animation;
            }
        }

        int i = Array.IndexOf(attackAnimations, a);

        if(i > numberOfClicks)
            return;

        _time = Time.time;
        anim.SetBool(attackAnimations[numberOfClicks - 1], true);
    }

    public void AttackStart(){
        BoxCollider collider = GetComponentInChildren<BoxCollider>();
        if(collider)
            collider.enabled = true;
    }

    public void EndAttack(){
        BoxCollider collider = GetComponentInChildren<BoxCollider>();
        if(collider)
            collider.enabled = false;

        isAttacking = false;

        numberOfClicks--;
        if(numberOfClicks < 0)
            numberOfClicks = 0;

        foreach(string animation in attackAnimations){
            if(anim.GetCurrentAnimatorStateInfo(0).IsName(animation)){
                int animIndex = Array.IndexOf(attackAnimations, animation);
                anim.SetBool(attackAnimations[animIndex], false);
                return;
            }
        }
    }

    //Utilities
    private bool CheckLunge(){
        if(pManager.isLockedOn){
            Vector3 tp = pTargeting.currentTarget.transform.position;
            float dist = Vector3.Distance(transform.position, tp);
            if(dist > attackRangeMin && dist < attackRangeMax){
                return true;
            }
        }
        return false;
    }
}
