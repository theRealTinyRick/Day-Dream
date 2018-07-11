using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
        
    private enum AttackState { NotAttacking, Equip, Swing1, Swing2};
    [SerializeField] private AttackState currentAtkState = AttackState.NotAttacking;

    private float timeToAtk = 0.75f;//the amount of time between clicks the player will stop attacking
    private float _time;

    private float timeToEnEquip = 5f;

    private PlayerController pController;
    private PlayerInventory pInventory;
    private Animator anim;
    private TrailRenderer trail;

    private void Start(){
        pController = GetComponent<PlayerController>();
        pInventory = GetComponent<PlayerInventory>();
        anim = GetComponent<Animator>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void Update(){
        if((Time.time - _time) > timeToAtk && PlayerManager.instance.currentState != PlayerManager.PlayerState.Traversing){
            currentAtkState = AttackState.NotAttacking;
            PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
            trail.enabled = false;
        }else{
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void Attack(){
        switch ((int)currentAtkState){
            case 0://not attacking
                if(!pInventory.Equipped){
                    pInventory.EquipWeapons(0.35f);
                    EquipedAttack();
                    break;
                }
                Swing1();
                break;
            case 2://swing 1
                Swing2();
                break;
        }
    }

    private void EquipedAttack(){
        _time = Time.time;
        currentAtkState = AttackState.Equip;
        anim.Play("EquipedAttack");
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
        trail.enabled = true;
    }

    private void Swing1(){
        _time = Time.time;
        anim.SetBool("Swing2", false);
        currentAtkState = AttackState.Swing1;
        anim.Play("Swing1");
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
        trail.enabled = true;
    }
    
    private void Swing2(){
        _time = Time.time;
        currentAtkState = AttackState.Swing2;
        anim.SetBool("Swing2",true);
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
        trail.enabled = true;
    }
}
