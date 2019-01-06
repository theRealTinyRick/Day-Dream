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
        /// The master bool to tell other systems if the player has hit a ledge
        /// Use this in other scripts to determine if we should apply and game logic related to elevation changes
        /// </summary>
        [SerializeField]
        [TabGroup(Tabs.Properties)]
        private bool validLedge = false;
        public bool ValidLedge { get { return validLedge; } }

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
        [ShowInInspector]
        private bool isClimbing = false;

        [TabGroup(Tabs.Properties)]
        [ShowInInspector]
        private bool isInPosition = false;

        private LayerMask layerMask = 1 << 8;
        Vector3 floorPoint = new Vector3();

        private Rigidbody _rigidbody;
        private PlayerElevationDetection playerElevationDetection;
        private PlayerGroundedComponent playerGroundedComponent;
        private PlayerStateComponent playerStateComponent;

        private void Start()
        {
            ledge = Vector3.zero;

            layerMask = ~layerMask;

            _rigidbody = GetComponent<Rigidbody>();
            playerElevationDetection = GetComponent<PlayerElevationDetection>();
            playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
            playerStateComponent = GetComponent<PlayerStateComponent>();

            InputDriver.jumpButtonEvent.AddListener(InputResponse);
        }

        private void OnDisable()
        {
            InputDriver.jumpButtonEvent.RemoveListener(InputResponse);
        }

        private void Update()
        {
            MoveToPosition();

            if(isInPosition && isClimbing && GetComponent<PlayerGroundedComponent>().IsGrounded)
            {
                Dismount();
            }
        }

        private void FixedUpdate()
        {
            DetectElevation();
        }

        private void InputResponse()
        {
            if(isInPosition && isClimbing)
            {
                Dismount();
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
                wallNormal = playerElevationDetection.WallNormal;
                StartCoroutine(GetInPosition(LedgeWithPlayerOffset(playerElevationDetection.Ledge), wallNormal));
                _rigidbody.isKinematic = true;
                playerStateComponent.SetStateHard(PlayerState.Traversing);
            }
        }

        private void Dismount()
        {
            isClimbing = false;
            isInPosition = false;
            _rigidbody.isKinematic = false;
            ledge = Vector3.zero;
            if(playerStateComponent.CurrentState == PlayerState.Traversing)
            {
                playerStateComponent.SetStateHard(PlayerState.Normal);
            }
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
            Quaternion _rotation = Quaternion.LookRotation(-wallNormal);
            while(Vector3.Distance(transform.position, position) > 0.1f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, 0.5f);
                transform.position = Vector3.MoveTowards(transform.position, position, 0.1f);
                yield return new WaitForEndOfFrame();
            }

            isInPosition = true;

            yield break;
        }

        private void DetectElevation()
        {
            if(!isInPosition || !isClimbing || InputDriver.LocomotionDirection.x == 0)
            {
                return;
            }

            Vector3 _origin = transform.position;
            _origin.y += 0.3f;

            RaycastHit _hit;

            Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
            if (Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck, layerMask))
            {
                GetComponent<PlayerStateComponent>().SetStateHard(PlayerState.Traversing);

                //get the reverse of the angle we shot the ray from to get an accurate angle calculation
                Vector3 _hitAngle = -transform.forward;

                //we store this distance so we can offset our downward ray cast and not over shoot it
                float _distanceToCheckDown = Vector3.Distance(_origin, _hit.point) + 0.1f;

                if (CheckWallSlope(_hit))
                {
                    Vector3 _targetOrigin = transform.position;

                    float horizontalInput = InputDriver.LocomotionDirection.normalized.x < 0 ? -1 : 1;

                    _targetOrigin += transform.right * (horizontalInput * ledgeClimbDistance);
                    _targetOrigin += -transform.forward * 0.3f;

                    _origin = _targetOrigin;
                    WallCheckPoint = _targetOrigin;

                    ///TODO : Add a chaeck to make sure there is no wall blocking the path

                    Debug.DrawRay(_origin, transform.forward * distanceToCheck, Color.red);
                    if (Physics.Raycast(_origin, transform.forward, out _hit, distanceToCheck, layerMask))
                    {
                        Vector3 _ledge = _hit.point;
                        Vector3 _normal = _hit.normal;

                        _origin = _hit.point;
                        _origin.y += maxLedgeShimyHeight;
                        _origin += -_normal * _distanceToCheckDown;

                        Debug.DrawRay(_origin, Vector3.down * maxLedgeShimyHeight, Color.red);

                        if (Physics.Raycast(_origin, Vector3.down, out _hit, maxLedgeShimyHeight, layerMask))
                        {
                           _ledge.y = _hit.point.y;

                           if (CheckFloorSlope(_hit))
                           {
                                SetLedge(_ledge, _normal);
                                return;
                           }
                        }
                    }
                }

                ledge = Vector3.zero;
            }
        }

        private void SetLedge(Vector3 ledgePoint, Vector3 normal)
        {
            ledge = ledgePoint;
            wallNormal = normal;
            Quaternion _rot = Quaternion.LookRotation(-normal);
            ledgeRotation = _rot;
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

            if (_angle > minimumWallSlope)
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
        }
    }
}

