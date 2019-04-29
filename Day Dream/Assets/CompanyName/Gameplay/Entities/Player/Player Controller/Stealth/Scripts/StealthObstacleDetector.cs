/*
Author: Aaron Hines
Edits By:
Description: 
 */
using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

namespace AH.Max.Gameplay.Stealth
{
    public enum StealthMode
    {
        Hidden,
        NotHidden
    }

    public class StealthObstacleDetector : MonoBehaviour
    {
        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public StealthMode stealthMode {get; private set;}

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        public StealthObstacle currentStealthObstacle {get; private set;}

        [TabGroup(Tabs.Properties)]
        [SerializeField]
        private LayerMask stealthObstacleLayerMask;
        
        private List <StealthObstacle> currentStealthObstacles = new List<StealthObstacle>();

        public void OnTriggerEnter(Collider other)
        {
            if(LayerMaskUtility.IsWithinLayerMask(stealthObstacleLayerMask, gameObject.layer))
            {
                StealthObstacle _stealthObstacle = other.GetComponentInChildren<StealthObstacle>();
                if(_stealthObstacle != null)
                {
                    currentStealthObstacles.Add(_stealthObstacle);
                }
            }    
        }
        
        public void OnTriggerExit(Collider other)
        {
            if(LayerMaskUtility.IsWithinLayerMask(stealthObstacleLayerMask, gameObject.layer))
            {
                StealthObstacle _stealthObstacle = other.GetComponentInChildren<StealthObstacle>();
                if(_stealthObstacle != null)
                {
                    currentStealthObstacles.Remove(_stealthObstacle);
                }
            }    
        }

        public void InitStealthMode()
        {
            StealthObstacle _stealthObstacle = FindBestStealthObstacle();
            if(_stealthObstacle != null)
            {
                EnterStealthMode(_stealthObstacle);
            }
        }

        public void EnterStealthMode(StealthObstacle stealthObstacle)
        {
            // TODO: add logic to make sure that we have not already been seen by an enemy clode by!!!!
            stealthMode = StealthMode.Hidden;
        }


        public void ExitStealthMode()
        {

        }

        public StealthObstacle FindBestStealthObstacle()
        {

            return null;
        }

    }
}
