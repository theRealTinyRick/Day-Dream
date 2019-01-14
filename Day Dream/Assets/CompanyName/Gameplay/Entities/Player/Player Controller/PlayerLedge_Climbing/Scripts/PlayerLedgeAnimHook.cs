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
    public const string ClimbUpAnimation = "Ledge_Hang_ToStand_Up";

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
        Vector3 _up = transform.up;
        Vector3 _toPosition = ledgePoint - transform.position;

        float xValue = inputXValue;
        float yValue = Vector3.Dot(_up, _toPosition);

        animator.SetFloat(LedgeXAnimationFloat, xValue);
        animator.SetFloat(LedgeYAnimationFloat, yValue);

        animator.Play(LedgeMoveAnim);
    }

    public void PlayClimbUpAnimation()
    {
        animator.Play(ClimbUpAnimation);
    }

    public void Dismount()
    {
        animator.SetBool(IsClimbingAnimBool, false);
    }
}
