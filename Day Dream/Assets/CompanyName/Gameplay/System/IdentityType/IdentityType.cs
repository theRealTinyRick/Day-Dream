using UnityEngine;

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
		public UsageType type;

        /// <summary>
        /// the handedness of the possible tool
        /// </summary>
        public Handedness handedness;

		///<Summary>
		///The prefab associated with the Identity
		///</Summary>
		public GameObject prefab;

        /// <summary>
        /// 
        /// </summary>
        public IdentityType parent;
	}
}
