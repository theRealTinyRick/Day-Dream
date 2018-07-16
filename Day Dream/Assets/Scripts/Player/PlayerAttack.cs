using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
        
    public enum AttackState { NotAttacking, Equip, Lunge, Swing1, Swing2, Swing3};
    [SerializeField] public AttackState currentAtkState = AttackState.NotAttacking;

    private float timeToAtk = 0.9f;//the amount of time between clicks the player will stop attacking
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
    private TrailRenderer trail;

    private void Start(){
        pController = GetComponent<PlayerController>();
        pInventory = GetComponent<PlayerInventory>();
        anim = GetComponent<Animator>();
        trail = GetComponentInChildren<TrailRenderer>();

        pManager = PlayerManager.instance;
        pTargeting = pController.PTargeting;

    }

    private void Update(){
        if((Time.time - _time) > timeToAtk && PlayerManager.instance.currentState != PlayerManager.PlayerState.Traversing && currentAtkState != AttackState.Lunge){
            currentAtkState = AttackState.NotAttacking;
            PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
            trail.enabled = false;

            BoxCollider collider = GetComponentInChildren<BoxCollider>();
            if(collider){
                collider.enabled = false;
            }
        }
    }

    public void Attack(){
        switch ((int)currentAtkState){
            case 0://not attacking
                if(CheckLunge()){
                    StartCoroutine(LungeAttack());
                    if(!pInventory.Equipped)
                        pInventory.EquipWeapons(0.35f);
                    break;
                }

                if(!pInventory.Equipped){
                    pInventory.EquipWeapons(0.35f);
                    EquipedAttack();
                    break;
                }

                Swing1();
                break;
            case 3:
                Swing2();
                break;
        }
    }

    private void EquipedAttack(){
        _time = Time.time;
        anim.Play("EquipedAttack");
        currentAtkState = AttackState.Equip;
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
        trail.enabled = true;
    }

    private IEnumerator LungeAttack(){
        anim.SetTrigger("Lunge");
        trail.enabled = true;

        Vector3 tp = pTargeting.currentTarget.transform.position;
        tp.y = transform.position.y;

        currentAtkState = AttackState.Lunge;
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
        transform.LookAt(tp);

        while(Vector3.Distance(transform.position, tp) > 1){
            transform.position = Vector3.Lerp(transform.position, tp, 0.2f);
            _time = Time.time;
            yield return new WaitForEndOfFrame();
        }

        PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
        yield return null;
    }

    private void Swing1(){
        _time = Time.time;
        anim.SetTrigger("Swing1");
        currentAtkState = AttackState.Swing1;
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
        trail.enabled = true;
    }
    
    private void Swing2(){
        _time = Time.time;
        anim.SetTrigger("Swing2");
        currentAtkState = AttackState.Swing2;
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
        trail.enabled = true;
    }

    public void AttackStart(){
        BoxCollider collider = GetComponentInChildren<BoxCollider>();
        collider.enabled = true;
    }

    public void EndAttack(){
        BoxCollider collider = GetComponentInChildren<BoxCollider>();
        collider.enabled = false;

        currentAtkState = AttackState.NotAttacking;
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
