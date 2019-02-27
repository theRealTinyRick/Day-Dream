using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerWallClimber : MonoBehaviour
{
    public float positionOffSet;
    public float offsetFromWall = 0.3f;
    public bool isLerping = false;
    public bool inPosition;

    private float climbSpeed = 3f;

    private float time = 0.0f;

    private float horitontalInput;
    private float verticalInput;

    private bool hasPlayedAnim = false;
    public bool isClimbing = false;

    private Rigidbody rb;

    public Transform helper;
    public Vector3 startPos;
    public Vector3 targetPos;

    private Transform climbStart;
    private Transform climbEnd;

    public float yOffset;

    [TabGroup(Tabs.Properties)]
    [SerializeField]
    public LayerMask layerMask;

    //events
    [TabGroup(Tabs.Events)]
    public WallClimbingStartedEvent wallClimbingStartedEvent = new WallClimbingStartedEvent();

    [TabGroup(Tabs.Events)]
    public WallClimbingEndedEvent wallClimbingEndedEvent = new WallClimbingEndedEvent();

    private void Start()
    {
        helper = new GameObject().transform;
        helper.name = "Climb Helper";

        climbStart = new GameObject().transform;
        climbEnd = new GameObject().transform;
        rb = GetComponent<Rigidbody>();
    }

    private void OnDrawGizmos()
    {
        if (climbEnd != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(climbEnd.position, Vector3.one * 0.2f);
        }

        if (climbStart != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(climbStart.position, Vector3.one * 0.1f);
        }
    }

    [Button]
    public bool CheckForClimb()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;

        if (Physics.Raycast(origin, transform.forward, out hit, 1, layerMask))
        {
            if (hit.transform.tag == "Climbable")
            {
                InitForClimb(hit);
                return true;
            }
        }
        return false;
    }

    public void InitForClimb(RaycastHit hit)
    {
        isClimbing = true;
        rb.isKinematic = true;
        helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        startPos = transform.position;
        targetPos = hit.point + (hit.normal * offsetFromWall);
        time = 0;
        inPosition = false;

        wallClimbingStartedEvent.Invoke();
    }

    private void Update()
    {
        if (isClimbing)
        {
            Tick(Time.deltaTime);
        }
    }

    public void Tick(float delta)
    {
        if (!inPosition)
        {
            GetInPosition();
            return;
        }

        horitontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (!isLerping)
        {
            hasPlayedAnim = false;

            Vector3 horizontal = helper.right * horitontalInput;
            Vector3 vertical = helper.up * verticalInput;
            Vector3 moveDir = (horizontal + vertical).normalized;

            if (!CanMove(moveDir) || moveDir == Vector3.zero)
            {
                Debug.Log("adsfasdfasdfadsfasdf");
                return;
            }

            time = 0;
            isLerping = true;
            startPos = transform.position;
            targetPos = helper.position;
        }
        else
        {
            time += delta * climbSpeed;
            if (time > 1)
            {
                time = 1;
                isLerping = false;
            }

            if (!hasPlayedAnim)
            {
                hasPlayedAnim = true;
            }

            Vector3 cp = Vector3.Lerp(startPos, targetPos, time);
            transform.position = cp;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, delta * 5);
        }
    }

    private bool CanMove(Vector3 moveDir)
    {
        if (moveDir.y > 0)
        {
            Vector3 o = transform.position;
            o.y += 2.5f;
            RaycastHit ledgeHit;
            Debug.DrawRay(o, transform.forward, Color.green, 5);
            if (Physics.Raycast(o, transform.forward, out ledgeHit, 5, layerMask))
            {
                if (ledgeHit.transform.tag != "Climbable")
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        if (moveDir.y < 0)
        {
            Vector3 origin1 = transform.position;
            RaycastHit hitFloor;
            if (Physics.Raycast(origin1, -Vector3.up, out hitFloor, positionOffSet, layerMask))
            {
                Drop();
                return false;
            }
        }

        Vector3 origin = transform.position;
        float dis = positionOffSet;
        Vector3 dir = moveDir;
        Debug.DrawRay(origin, dir * dis, Color.red, 5);
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, dis, layerMask))
        {

            if (hit.transform.tag != "Climbable")
            {
                return false;
            }

            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        origin += moveDir * dis;
        dir = helper.forward;
        float dis2 = 1;

        Debug.DrawRay(origin, dir * dis2, Color.blue, 5);
        if (Physics.Raycast(origin, dir, out hit, dis2))
        {

            if (hit.transform.tag != "Climbable")
            {
                return false;
            }

            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        return false;
    }

    private void GetInPosition()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            time = 1;
            inPosition = true;
        }

        Vector3 tp = Vector3.Lerp(startPos, targetPos, time * 9);
        transform.position = tp;

        tp.y = transform.position.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, Time.deltaTime * 5);
    }

    private Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        Vector3 direction = origin - target;
        direction.Normalize();
        Vector3 offset = direction * offsetFromWall;
        return target + offset;
    }

    private Vector3 LedgeWithOffset(Vector3 ledge)
    {
        ledge += -transform.forward * 0.1f;
        ledge.y -= yOffset;
        return ledge;
    }

    public void Drop()
    {
        rb.isKinematic = false;
        isClimbing = false;
        inPosition = false;
        isLerping = false;

        wallClimbingEndedEvent.Invoke();
    }
}

[System.Serializable]
public class WallClimbingStartedEvent : UnityEngine.Events.UnityEvent
{
}

[System.Serializable]
public class WallClimbingEndedEvent : UnityEngine.Events.UnityEvent
{
}