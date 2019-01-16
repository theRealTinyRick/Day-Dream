using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

using AH.Max.System;

///<Summary>
///The base for every "Actor" in the scene. This will be on every object that we want to interact with. 
///</Summary>
namespace AH.Max
{
    public class Entity : MonoBehaviour, IEntity
    {
        [TabGroup(Tabs.Entity)]
        [SerializeField]
        private IdentityType identityType;
        public IdentityType IdentityType
        {
            get { return identityType; }
        }

        [TabGroup(Tabs.Entity)]
        [SerializeField]
        private bool doNotDestroyOnLoad;

        public void OnEnable()
        {
            if(identityType == null)
            {
                Debug.LogError("All Entities must have a valid identity type" + gameObject);
                return;
            }

            EntityManager.Instance.RegisterEntity(this);

            if(doNotDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            Enable();
        }

        public void OnDisable() 
        {
            if(identityType == null)
            {
                Debug.LogError("All Entities must have a valid identity type");
                return;
            }

            EntityManager.Instance.RemoveEntity(this);

            Disable();
        }

        ///<Summary>
        ///Override this method to add additional OnEnable logic
        ///</Summary>
        protected virtual void Enable()
        {
        }

        ///<Summary>
        ///Override this method to add additional OnDisable logic.
        ///</Summary>
        protected virtual void Disable()
        {
        }
    }
}
