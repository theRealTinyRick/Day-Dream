using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
    public class PlayerLedgeFinder : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float distanceToCheck;

        [HideInInspector]
        public float DistanceToCheck { get { return distanceToCheck; } }

        /// <summary>
        /// The actual position of the ledge
        /// </summary>
        /// <returns></returns>
        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private Vector3 ledge = new Vector3();
        public Vector3 Ledge { get { return ledge; } }

        private Quaternion ledgeRotation;

        /// <summary>
        /// The normal of the wall that we are detecting against.
        /// It should be kept in mind that if you use this yo should assume to only use this if there is a validLedge
        /// </summary>
        /// <returns></returns>
        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private Vector3 wallNormal = new Vector3();
        public Vector3 WallNormal { get { return wallNormal; } }

        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private Vector2 playerOffset = new Vector3();
        public Vector2 PlayerOffset { get { return wallNormal; } }

        private Vector3 WallCheckPoint;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maxLedgeShimyHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minLedgeShimyHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maxMountHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float maxAirMountHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minMountHeight;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumWallSlope;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float minimumFloorSlope;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private float ledgeClimbDistance;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        [Range(0, 1)]
        private float climbSpeed;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        [Range(0, 1)]
        private float mountSpeed;

        [TabGroup(Tabs.Properties)]
        [ShowInInspector]
        private bool isClimbing = false;
        public bool IsClimbing 
        {
            get 
            {
                return isClimbing;
            }
        }

        [TabGroup(Tabs.Properties)]
        [ShowInInspector]
        private bool isInPosition = false;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private bool canClimbUp 
        {
            get 
            {
                return IsAtNextClimbPoint();
            }
        }

        private bool isClimbingUp = false;  

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Vector3 climbUpPosition = new Vector3();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        LedgeClimbStartEvent ledgeClimbStarted = new LedgeClimbStartEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        LedgeClimbStoppedEvent ledgeClimbStopped = new LedgeClimbStoppedEvent();

        private LayerMask layerMask = 1 << 8;
        Vector3 floorPoint = new Vector3();

        private Rigidbody _rigidbody;
        private PlayerElevationDetection playerElevationDetection;
        private PlayerGroundedComponent playerGroundedComponent;
        private PlayerStateComponent playerStateComponent;
        private PlayerLedgeAnimHook playerLedgeAnimHook;

        private void Start()
        {
            ledge = Vector3.zero;

            layerMask = ~layerMask;

            _rigidbody = GetComponent<Rigidbody>();
            playerElevationDetection = GetComponent<PlayerElevationDetection>();
            playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
            playerStateComponent = GetComponent<PlayerStateComponent>();
            playerLedgeAnimHook = GetComponent<PlayerLedgeAnimHook>();

            InputDriver.jumpButtonEvent.AddListener(InputResponse);
        }

        private void OnDisable()
        {
            InputDriver.jumpButtonEvent.RemoveListener(InputResponse);
        }

        private void Update()
        {
        }

        private void FixedUpdate()
        {
            DetectLedgePoint();

            if (!isClimbingUp)
            {
                MoveToPosition();
            }

            if(isInPosition && isClimbing && playerGroundedComponent.IsGrounded)
            {
                Dismount();
            }
        }

        private void InputResponse()
        {
            if(isInPosition && isClimbing)
            {
                if(InputDriver.LocomotionDirection.normalized.z > 0)
                {
                    StartCoroutine(ClimbupLedge());
                }
                else
                {
                    // added this to make sure the player has finished an animation before an manual dismount
                    if(IsAtNextClimbPoint())
                    {
                        Dismount();
                    }
                }
            }
            else
            {
                InitClimb();
            }
        }

        private void InitClimb()
        {
            if(CheckValidLedge())
            {
                isClimbing = true;
                _rigidbody.isKinematic = true;

                SetLedge(playerElevationDetection.Ledge, playerElevationDetection.WallNormal);

                playerLedgeAnimHook.PlayMountAnim();

                wallNormal = playerElevationDetection.WallNormal;

                StartCoroutine(GetInPosition(LedgeWithPlayerOffset(playerElevationDetection.Ledge), playerElevationDetection.WallNormal));

                playerStateComponent.SetStateHard(PlayerState.Traversing);

                if(ledgeClimbStarted != null)
                {
                    ledgeClimbStarted.Invoke();
                }
            }
        }

        private void Dismount()
        {
            isClimbingUp = false;
            isClimbing = false;
            isInPosition = false;
            _rigidbody.isKinematic = false;
            ledge = Vector3.zero;
            playerLedgeAnimHook.Dismount();

            ResetRotation();
            
            if (playerStateComponent.CurrentState == PlayerState.Traversing)
            {
                playerStateComponent.SetStateHard(PlayerState.Normal);
            }

            if(ledgeClimbStopped != null)
            {
                ledgeClimbStopped.Invoke();
            }
        }

        private void ResetRotation()
        {
            Quaternion _rotation = Quaternion.Euler(0, transform.root.rotation.y, 0);
        }

        public bool CheckValidLedge()
        {
            //check the elevation detector
            if(playerElevationDetection.ValidLedge)
            {
                Vector3 _rayCastOrigin = transform.position;
                _rayCastOrigin.y += 0.5f;
                RaycastHit _hitResult;
                if(Physics.Raycast(_rayCastOrigin, Vector3.down, out _hitResult, 100))
                {
                    floorPoint = _hitResult.point;

                    float _floorHit = _hitResult.point.y;
                    float _ledgeHeight = playerElevationDetection.Ledge.y;

                    if(_floorHit < _ledgeHeight)
                    {
                        float _heightDifference = _ledgeHeight - _floorHit;

                        float _maxHeight = playerGroundedComponent.IsGrounded ? maxMountHeight : maxAirMountHeight;
                        float _minHeight = playerGroundedComponent.IsGrounded ? minMountHeight : 0;

                        if(_heightDifference > _minHeight && _heightDifference < _maxHeight)
                        {
                            return true;
                        }
                    }
                }
            }

            // return true if the player is so high in the air that the ray doesn't hit anymore
            return false;
        }

        private IEnumerator GetInPosition(Vector3 position, Vector3 wallNormal)
        {
            // I need to add something here to check if the mount failed for some reason then call the dismount method

            Quaternion _rotation = Quaternion.LookRotation(-wallNormal);
            while(Vector3.Distance(transform.position, position) > 0.1f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, 0.5f);
                transform.position = Vector3.MoveTowards(transform.position, position, mountSpeed);

                yield return new WaitForFixedUpdate();
            }

            isInPosition = true;

            yield break;
        }

        private IEnumerator ClimbupLedge()
        {
            if(canClimbUp)
            {
                isClimbingUp = true;

                Vector3 position = climbUpPosition;
                while (Vector3.Distance(transform.position, position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, position, mountSpeed);
                    yield return new WaitForFixedUpdate();
                }

                //ROOT MOTION
                //playerLedgeAnimHook.PlayClimbUpAnimation();

                Dismount();

                yield break;
            }
        }

        private bool IsAtNextClimbPoint()
        {
            if (isInPosition && isClimbing)
            {
                float _distance = Vector3.Distance(transform.position, LedgeWithPlayerOffset(ledge));
                if(_distance < 0.01f)
                {
                    if(climbUpPosition != Vector3.zero)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsPlayerInTheIdleState()
        {
            return GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LedgeIdle");
        }

        private void DetectLedgePoint()
        {
            if(isInPosition && isClimbing && InputDriver.LocomotionDirection.x != 0 && IsAtNextClimbPoint() && !isClimbingUp && IsPlayerInTheIdleState())
            {
                Vector3 _origin = transform.position;
                _origin.y += 0.3f;

                RaycastHit _hit;

                //Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red, 5);
                if (Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck, layerMask))
                {
                    //get the reverse of the angle we shot the ray from to get an accurate angle calculation
                    Vector3 _hitAngle = -transform.forward;

                    //we store this distance so we can offset our downward ray cast and not over shoot it
                    float _distanceToCheckDown = Vector3.Distance(_origin, _hit.point) + 0.1f;

                    if (CheckWallSlope(_hit))
                    {
                        ///TODO: add a check on if we find a spot to climb to - MAYBE ADD 90 DEGREE TURNS IN TOWARDS THE WALL

                        float horizontalInput = InputDriver.LocomotionDirection.normalized.x < 0 ? -1 : 1;

                        Vector3 _obsticalDirection = transform.right * horizontalInput;
                        Vector3 _targetOrigin = transform.position;

                        bool _useObstical = false;

                        if (Physics.Raycast(_origin, _obsticalDirection, out _hit, ledgeClimbDistance + 0.5f, layerMask))
                        {
                            if(CheckWallSlope(_hit))
                            {
                                _useObstical = true;
                            }
                        }

                        if(_useObstical)
                        {
                            _targetOrigin = _hit.point + _hit.normal;
                            _obsticalDirection = -_hit.normal;
                         //   Debug.DrawRay(_origin, _obsticalDirection * ledgeClimbDistance, Color.green, 3);
                        }
                        else
                        {
                            _targetOrigin += transform.right * (horizontalInput * ledgeClimbDistance);
                            _obsticalDirection = transform.forward;
                        }

                        //_targetOrigin += _hit.normal * 0.3f;

                        _origin = _targetOrigin;
                        WallCheckPoint = _targetOrigin;

                   //     Debug.DrawRay(_origin, _obsticalDirection * (distanceToCheck + 1), Color.red, 5);
                        if (Physics.Raycast(_origin, _obsticalDirection, out _hit, distanceToCheck + 1, layerMask))
                        {
                            Vector3 _ledge = _hit.point;
                            Vector3 _normal = _hit.normal;

                            _origin = _hit.point;
                            _origin.y += maxLedgeShimyHeight;
                            _origin += -_normal * _distanceToCheckDown;

                         //   Debug.DrawRay(_origin, Vector3.down * maxLedgeShimyHeight, Color.red, 5);

                            if (Physics.Raycast(_origin, Vector3.down, out _hit, maxLedgeShimyHeight, layerMask))
                            {
                               _ledge.y = _hit.point.y;

                               if (CheckFloorSlope(_hit))
                               {
                                    SetLedge(_ledge, _normal);

                                    playerLedgeAnimHook.PlayClimbAnimation(LedgeWithPlayerOffset(ledge), horizontalInput);

                                    return;
                               }
                            }
                        }
                    }
                }
            }
        }

        private void SetLedge(Vector3 ledgePoint, Vector3 normal)
        {
            ledge = ledgePoint;
            wallNormal = normal;
            Quaternion _rot = Quaternion.LookRotation(-normal);
            ledgeRotation = _rot;

            climbUpPosition = CalculateClimbUpPosition(ledgePoint, normal);
        }

        private Vector3 CalculateClimbUpPosition(Vector3 ledgePoint, Vector3 normal)
        {
            ledgePoint -= normal;

            Debug.Log("DO SOMETHING HERE TO MAKE SURE WE HAVE A CLEAR POSITION");

            return ledgePoint;
        }

        private void MoveToPosition()
        {
            if(isClimbing && isInPosition && ledge != Vector3.zero)
            {
                transform.position = Vector3.MoveTowards(transform.position, LedgeWithPlayerOffset(ledge), climbSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, ledgeRotation, 0.2f);
            }
        }

        //Checks the slope of the wall we want to vault/ climb over
        private bool CheckWallSlope(RaycastHit hit)
        {
            float _angle = Vector3.Angle(hit.normal, Vector3.up);

            if (_angle >= minimumWallSlope)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks the slop of the floor we are trying to mount
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
		private bool CheckFloorSlope(RaycastHit hit)
        {
            float _angle = Vector3.Angle(hit.normal, Vector3.up);

            if (_angle < minimumFloorSlope)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ledge"></param>
        /// <returns></returns>
        private Vector3 LedgeWithPlayerOffset(Vector3 ledge)
        {
            ledge += WallNormal * playerOffset.x;
            ledge += Vector3.up * playerOffset.y;
            return ledge;
        }

        /// <summary>
        /// Draw the ledge positno if its good
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(WallCheckPoint, Vector3.one * 0.15f);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(ledge, Vector3.one * 0.2f);

            Gizmos.color = Color.blue;

            Vector3 maxHeightPos = transform.position;
            maxHeightPos.y += maxMountHeight;
            Gizmos.DrawSphere(maxHeightPos, 0.1f);

            Vector3 minHieghtPos = floorPoint;
            minHieghtPos.y += minMountHeight;
            Gizmos.DrawSphere(minHieghtPos, 0.1f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(floorPoint, 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(climbUpPosition, 0.1f);
        }
    }
}