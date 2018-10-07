using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay.AI.BruteEnemy
{
	public class BruteEnemy : AIEntity<BruteEnemy>, IEnemy
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

		//this is a read only property. Only this class and the base class can change state
		[HideInInspector]
		public override AIStates State
		{	
			get { return _state; }
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
			// do stuff here to initialize the game
			
		}

		protected override bool CheckAggro()
		{
			if( _state != AIStates.Aggro )
			{
				//check every way the enemy should look for the player
				// if(CheckRangeSquared(aggroRange, transform.position, ))
				return true;
			}
			return false;
		}

		private void Update () 
		{
			Tick();
		}	

		protected override void Tick()
		{
			if( !EntityManager.Instance.Player ) return;

			if(CheckAggro())
			{
				SetStateHard(AIStates.Aggro, this);
			}

			switch( State )
			{
				case AIStates.Stationary:
					break;
			
				case AIStates.Patrol:
					break;
			
				case AIStates.Aggro:
					Debug.Log( "Hey I'm super aggro right now" );
					break;
			}
		}
	}

}
