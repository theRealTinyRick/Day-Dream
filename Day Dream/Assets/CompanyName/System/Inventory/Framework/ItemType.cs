using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.System.Inventory
{
    [CreateAssetMenu(fileName = "New Identity Type", menuName = "CompanyName/IdentityType", order = 1)]
    public class ItemType : ScriptableObject
    {
        public ItemTypes itemType;
        public string name;
    }
}
