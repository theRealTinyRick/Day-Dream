using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {
        
    private enum AttackState { NotAttacking, Swing1, Swing2};
    [SerializeField] private AttackState currentAtkState = AttackState.NotAttacking;

    private float timeToAtk = 0.5f;//the amount of time between clicks the player will stop attacking
    private float _time;

    private void Update(){
        if ((Time.time - _time) > timeToAtk && PlayerManager.instance.currentState != PlayerManager.PlayerState.Traversing){
            currentAtkState = AttackState.NotAttacking;
            PlayerManager.instance.currentState = PlayerManager.PlayerState.FreeMovement;
        }
    }

    public void Attack(){
        Cursor.lockState = CursorLockMode.Locked;
        switch ((int)currentAtkState){
            case 0://not attacking
                Swing1();
                break;
            case 1://swing 1
                Swing2();
                break;
        }
    }

    private void Swing1(){
        _time = Time.time;
        PlayerManager.instance.anim.SetBool("Swing2", false);
        currentAtkState = AttackState.Swing1;
        PlayerManager.instance.anim.Play("Swing1");
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
    }
    
    private void Swing2(){
        _time = Time.time;
        currentAtkState = AttackState.Swing2;
        PlayerManager.instance.anim.SetBool("Swing2",true);
        PlayerManager.instance.currentState = PlayerManager.PlayerState.Attacking;
    }
}
