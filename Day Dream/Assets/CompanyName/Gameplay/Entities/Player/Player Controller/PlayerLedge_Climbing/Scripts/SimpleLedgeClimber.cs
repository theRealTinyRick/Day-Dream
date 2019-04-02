using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class SimpleLedgeClimber : MonoBehaviour
{
    private const string ClimbAnim = "ClimbLedge";

    [SerializeField]
    private LayerMask climbLayer;

    [SerializeField]
    private float checkDistance;

    [SerializeField]
    private float maxClimbHeight;

    [SerializeField]
    private float minClimbHeight;

    [SerializeField]
    private float maxCheckAngle;

    //[HideInInspector]
    public bool isHoldingJumpButton = false;

    //[HideInInspector]
    public bool hasValidLedge = false;

    //[HideInInspector]
    public bool isClimbing = false;

    private Coroutine climbCoroutine;
    private Animator animator;

    private Vector3 ledgePoint;
    private Vector3 ledgeNormal;

    [MinMaxSlider(0, 1)]
    [SerializeField]
    Vector2 animationTimesOne;

    [MinMaxSlider(0, 1)]
    [SerializeField]
    Vector2 animationTimesTwo;


    public AnimationCurve matchTargetSpeed;

    private Rigidbody rigidBody;

    private void OnEnable()
    {
        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponentInChildren<Rigidbody>();
    }

    void Update()
    {
        FindLedge();
        InputResponse();
    }

    public void SetInputStatus(bool value)
    {
        isHoldingJumpButton = value;
    }

    private void FindLedge()
    {
        if(!isClimbing)
        {
            Vector3 _origin = transform.position + Vector3.up;
            Vector3 _direction = transform.forward;
            Vector3 _normal = Vector3.zero;
            Vector3 _ledge = Vector3.zero;

            RaycastHit _raycastHit;

            if (Physics.Raycast(_origin, _direction, out _raycastHit, checkDistance, climbLayer))
            {
                // check for space above
                _origin = transform.position + (Vector3.up * maxClimbHeight);
                _normal = _raycastHit.normal;
                _ledge = _raycastHit.point;

                // dont record the raycast
                if (Physics.Raycast(_origin, _direction, checkDistance)) return;

                _origin = (new Vector3(_raycastHit.point.x, _origin.y, _raycastHit.point.z) + (transform.forward * 0.2f));
                _direction = Vector3.down;

                if (Physics.Raycast(_origin, _direction, out _raycastHit, checkDistance, climbLayer))
                {
                    _ledge.y = _raycastHit.point.y;

                    ledgeNormal = _normal;
                    ledgePoint = _ledge;
                    hasValidLedge = true;

                    return;
                }
            }

            ledgeNormal = _normal;
            ledgePoint = _ledge;
            hasValidLedge = false;
        }
    }

    private void InputResponse()
    {
        if(isHoldingJumpButton == true &&  isClimbing == false && climbCoroutine == null && hasValidLedge)
        {
            climbCoroutine = StartCoroutine(Climb());
        }
    }

    private IEnumerator Climb()
    {
        animator.Play(ClimbAnim);
        isClimbing = true;
        rigidBody.useGravity = false;
        rigidBody.isKinematic = true;

        yield return new WaitForEndOfFrame();

        while (animator.GetCurrentAnimatorStateInfo(0).IsName(ClimbAnim))
        {
            AnimatorUtilites.MatchTarget(animator, HumanBodyBones.RightHand, ClimbAnim, transform, ledgePoint + (Vector3.up * 0.1f), Quaternion.LookRotation(-ledgeNormal), animationTimesOne.x, animationTimesOne.y, matchTargetSpeed.Evaluate(animator.GetCurrentAnimatorStateInfo(0).normalizedTime), 1);
            yield return new WaitForEndOfFrame();
            //AnimatorUtilites.MatchTarget(animator, HumanBodyBones.LeftFoot, ClimbAnim, transform, ledgePoint + (Vector3.up * 0.1f), Quaternion.LookRotation(-ledgeNormal), animationTimesTwo.x, animationTimesTwo.y, matchTargetSpeed.Evaluate(animator.GetCurrentAnimatorStateInfo(0).normalizedTime), 1);
            //yield return null;
        }

        rigidBody.useGravity = true;
        rigidBody.isKinematic = false;
        isClimbing = false;
        climbCoroutine = null;
        yield break;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(ledgePoint, Vector3.one * 0.1f);
    }
}
