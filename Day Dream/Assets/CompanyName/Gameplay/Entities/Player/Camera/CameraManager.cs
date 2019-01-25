using System;

using UnityEngine;

using Sirenix.OdinInspector;

using Cinemachine;

namespace AH.Max.Gameplay.Camera
{
    [Serializable]
    public class StateData
    {
        public Vector3 positionOffset;
        public Vector3 lookAtOffset;
        public float positionDamping;
        public float recenterDelay;
        public float recenterTime;
    }

    public class CameraManager : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private CinemachineFreeLook cm_cameraController;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Transform cameraFollow;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Transform cameraLookAt;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private TargetingManager targetingManager;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private PlayerLedgeFinder playerLedgeFinder;

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private Entity entity;

        [TabGroup("Normal")]
        [SerializeField]
        private StateData normalStateData;

        [TabGroup("Locked On")]
        [SerializeField]
        private StateData lockedOnStateData;

        [TabGroup("Climbing")]
        [SerializeField]
        private StateData climbingStateData;

        private const string MouseX = "Mouse X";
        private const string MouseY = "Mouse Y";

        private void Start()
        {
            if(entity == null)
            {
                entity = transform.root.GetComponentInChildren<Entity>();
            }

            if (targetingManager == null)
            {
                targetingManager = transform.root.GetComponentInChildren<TargetingManager>();
            }

            if(playerLedgeFinder == null)
            {
                playerLedgeFinder = transform.root.GetComponentInChildren<PlayerLedgeFinder>();
            }
        }

	    private void Update ()
        {
            if(cameraFollow != null)
            {
		        if(targetingManager.LockedOn)
                {
                    if(targetingManager.CurrentTarget != null)
                    {
                        Vector3 _targetDirection = targetingManager.CurrentTarget.transform.position - entity.transform.position;
                        Quaternion _targetRotation = Quaternion.LookRotation(_targetDirection);

                        _targetRotation.x = 0;
                        _targetRotation.z = 0;

                        cameraFollow.position = Vector3.Lerp(cameraFollow.position, entity.transform.position + lockedOnStateData.positionOffset, lockedOnStateData.positionDamping);
                        cameraFollow.rotation = _targetRotation;

                        cameraLookAt.position = Vector3.Lerp(cameraLookAt.position, transform.position + lockedOnStateData.lookAtOffset, lockedOnStateData.positionDamping);

                        cm_cameraController.m_XAxis.m_InputAxisName = "";
                        cm_cameraController.m_YAxis.m_InputAxisName = "";

                        cm_cameraController.m_XAxis.m_InputAxisValue = 0;
                        cm_cameraController.m_YAxis.m_InputAxisValue = 0;

                        cm_cameraController.m_RecenterToTargetHeading.m_WaitTime = lockedOnStateData.recenterDelay;
                        cm_cameraController.m_YAxisRecentering.m_WaitTime = lockedOnStateData.recenterDelay;

                        cm_cameraController.m_RecenterToTargetHeading.m_RecenteringTime = lockedOnStateData.recenterTime;
                        cm_cameraController.m_YAxisRecentering.m_RecenteringTime = lockedOnStateData.recenterTime;

                        return;
                    }
                }
                else if(playerLedgeFinder.IsClimbing)
                {
                    cm_cameraController.m_XAxis.m_InputAxisName = MouseX;
                    cm_cameraController.m_YAxis.m_InputAxisName = MouseY;

                    cameraFollow.position = Vector3.Lerp(cameraFollow.position, transform.position + climbingStateData.positionOffset, climbingStateData.positionDamping);
                    cameraFollow.rotation = transform.rotation;

                    cameraLookAt.position = Vector3.Lerp(cameraLookAt.position, transform.position + climbingStateData.lookAtOffset, climbingStateData.positionDamping);

                    cm_cameraController.m_RecenterToTargetHeading.m_WaitTime = climbingStateData.recenterDelay;
                    cm_cameraController.m_YAxisRecentering.m_WaitTime = climbingStateData.recenterDelay;

                    cm_cameraController.m_RecenterToTargetHeading.m_RecenteringTime = climbingStateData.recenterTime;
                    cm_cameraController.m_YAxisRecentering.m_RecenteringTime = climbingStateData.recenterTime;

                    return;
                }

                // reset the data
                cm_cameraController.m_XAxis.m_InputAxisName = MouseX;
                cm_cameraController.m_YAxis.m_InputAxisName = MouseY;

                cm_cameraController.m_RecenterToTargetHeading.m_WaitTime = 3f;
                cm_cameraController.m_YAxisRecentering.m_WaitTime = 3f;

                cm_cameraController.m_RecenterToTargetHeading.m_RecenteringTime = 2f;
                cm_cameraController.m_YAxisRecentering.m_RecenteringTime = 2f;

                cameraFollow.position = Vector3.Lerp(cameraFollow.position, transform.root.position, normalStateData.positionDamping);
                cameraFollow.rotation = transform.rotation;

                cameraLookAt.position = Vector3.Lerp(cameraLookAt.position, transform.position + normalStateData.lookAtOffset, normalStateData.positionDamping);
            }
	    }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(cameraFollow.position, Vector3.one * 0.2f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(cameraLookAt.position, Vector3.one * 0.3f);
        }

    }
}

