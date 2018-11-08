using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.System
{
	[CreateAssetMenu(fileName = "New Identity Type", menuName = "CompanyName/IdentityType", order = 1)]
	public class IdentityType : ScriptableObject 
	{
		///<Summary>
		///The name of the identity type
		///</Summary>
		public string name;
		
		///<Summary>
		///The type of entity it is
		///</Summary>
		public IdentityTypes type;

		///<Summary>
		///The prefab associated with the Identity
		///</Summary>
		public GameObject prefab;
	}
}
