using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.AI.BruteEnemy
{
	public class BruteEnemy : AIEntity, IEnemy 
	{	
		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		private Transform targetPlayer;
		[HideInInspector]
		public Transform TargetPlayer 
		{
			get { return targetPlayer; } 
			private set { targetPlayer = value; }
		}

		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		private EntityType entityType;
		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		public EntityType EntityType
		{
			get { return entityType; } 
			private set { entityType = value; }
		}


		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		private EnemyType enemyType;
		[HideInInspector]		
		public EnemyType EnemyType 
		{
			get { return enemyType; }
			private set { enemyType = value; }  
		}

		[SerializeField]
		[TabGroup(Tabs.EnemyInformation)]
		private AIStates state;
		[HideInInspector]
		public AIStates State
		{
			get { return state; }
			private set { state = value; } 
		}

		[SerializeField]
		[TabGroup(Tabs.Preferences)]
		private float maxFieldOfViewAngle;
		[HideInInspector]
		public float MaxFieldOfViewAngle
		{
			get { return maxFieldOfViewAngle; } 
			private set { maxFieldOfViewAngle = value; }
		}

		[SerializeField] 
		[TabGroup(Tabs.Preferences)]
		private float maxHeightDifference;
		[HideInInspector]
		public float MaxHeightDifference
		{
			get { return maxHeightDifference; }
			private set { maxHeightDifference = value; }
		}

		[SerializeField]
		[TabGroup(Tabs.Preferences)]
		private float aggroRange;
		[HideInInspector]
		public float AggroRange
		{
			get { return aggroRange; }
			private set { aggroRange = value; }
		}

		[SerializeField]
		[TabGroup(Tabs.Preferences)]
		private float[] attackRange;
		[HideInInspector]
		public float[] AttackRange 
		{
			get { return attackRange; }  
			private set { attackRange = value; }
		}

		public override void Initialize()
		{

		}

		void Update () {
			
		}

		protected override bool CheckAggro()
		{
			return false;
		}
	}

}
