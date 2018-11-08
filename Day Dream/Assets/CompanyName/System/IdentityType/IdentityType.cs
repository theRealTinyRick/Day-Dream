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
		
		public IdentityTypes type;

		public GameObject prefab;

		public GameObject instancePrefab;
	}
}
