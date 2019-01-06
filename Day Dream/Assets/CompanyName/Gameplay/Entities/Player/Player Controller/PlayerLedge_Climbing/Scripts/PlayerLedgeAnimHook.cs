using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerLedgeAnimHook : MonoBehaviour
{
    public const string IsClimbingAnimBool = "IsClimbingLedge";
    public const string LedgeXAnimationFloat = "LedgeX";
    public const string LedgeYAnimationFloat = "LedgeY";
    public const string LedgeMoveAnim = "LedgeMove"; /*The actual climb animation for moving on the wall*/
    public const string StandingMountAnim = "StandingLedgeMount";
    public const string AirMountAnim = "AirLedgeMount";

    private Animator animator;
    private PlayerGroundedComponent playerGroundedComponent;

	private void Start ()
    {
        animator = GetComponent<Animator>();
        playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
    }
	
    public void PlayMountAnim()
    {
        animator.SetBool(IsClimbingAnimBool, true);
        string _mountAnim = playerGroundedComponent.IsGrounded ? StandingMountAnim : AirMountAnim;
        animator.Play(_mountAnim);
    }

    public void PlayClimbAnimation(Vector3 ledgePoint, float inputXValue)
    {
        float xValue = inputXValue;
        //float yValue;

        animator.SetFloat(LedgeXAnimationFloat, xValue);
        animator.Play(LedgeMoveAnim);
    }

    public void Dismount()
    {
        animator.SetBool(IsClimbingAnimBool, false);
    }
}
