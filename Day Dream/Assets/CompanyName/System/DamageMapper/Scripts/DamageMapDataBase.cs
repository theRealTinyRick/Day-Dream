using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.System.DamageMap
{
    public class DamageMapDataBase : Singleton_SerializedMonobehaviour<DamageMapDataBase>
    {
        [TabGroup(Tabs.Properties)]
        public Dictionary<IdentityType, List<IdentityType>> damageMap;

        /// <summary>
        /// Determine wheter or not you can damage the entity being targeted by passing in the identity typesz
        /// </summary>
        /// <param name="damager"> The Identity that is trying to deal damage</param>
        /// <param name="beingDamaged"> The Identity that I want to target </param>
        /// <returns></returns>
        public static bool CanDamage(IdentityType damager, IdentityType beingDamaged)
        {
            List<IdentityType> _listOfDamageableEntities;

            if(Instance.damageMap.TryGetValue(damager, out _listOfDamageableEntities))
            {
                return _listOfDamageableEntities.Contains(beingDamaged);
            }

            return false;
        }

        /// <summary>
        /// You can determine if your component can damage another just by passing the objects.
        /// This can be used to clean up code and remove a ton of get components if you dont already have access to the entity component on both
        /// </summary>
        /// <param name="damager">The gameobject or component trying to deal damage</param>
        /// <param name="beingDamaged">The gameobject or component trying to deal damage</param>
        /// <returns></returns>
        public static bool CanDamage(GameObject damager, GameObject beingDamaged)
        {
            Entity _damager = damager.transform.root.GetComponentInChildren<Entity>();
            Entity _beingDamage = beingDamaged.transform.root.GetComponentInChildren<Entity>();

            if(_damager != null && _beingDamage != null)
            {
                List<IdentityType> _listOfDamageableEntities;
             
                if (Instance.damageMap.TryGetValue(_damager.IdentityType, out _listOfDamageableEntities))
                {
                   return _listOfDamageableEntities.Contains(_beingDamage.IdentityType);
                }
            }

            Debug.Log("One of the GameObjects you passed in, does not have an identity type");

            return false;
        }

        public static List<IdentityType> GetWhatICanDamage(IdentityType damager)
        {
            return Instance.damageMap[damager];
        }

        /// <summary>
        /// This will get every entity in the scene that you can deal damage to.
        /// NOTE: the return entities are currently in the scene
        /// </summary>
        /// <param name="_damager"></param>
        /// <returns></returns>
        public static List<Entity> GetWhatICanDamage(Entity _damager, bool getOnlyAlive = false)
        {
            List<IdentityType> _damageableIdentities;

            if(Instance.damageMap.TryGetValue(_damager.IdentityType, out _damageableIdentities))
            {
                return EntityManager.GetEntities(_damageableIdentities);
            }

            return null;
        }

        //TODO: add an option to get only the ones that are alive
    }
}
