using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

///<Summary>
///The base for every "Actor" in the scene.
///</Summary>
namespace AH.Max
{
    public class Entity : MonoBehaviour, IEntity
    {
        [SerializeField]
        private EntityType entityType;
        public EntityType EntityType
        {
            get { return entityType; }
        }
    }
}
