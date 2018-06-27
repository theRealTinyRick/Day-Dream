using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAttack))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour {

    public static PlayerManager instance;
    
    public enum PlayerState { FreeMovement, CanNotMove, Traversing, Attacking, Blocking, Dead};
    public PlayerState currentState = PlayerState.FreeMovement;

    public bool isLockedOn = false; //stay
    public bool isVulnerable = true;

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
