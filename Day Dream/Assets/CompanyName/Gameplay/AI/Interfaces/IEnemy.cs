using System;
using UnityEngine;

namespace AH.Max.Gameplay.AI
{
	public interface IEnemy
	{
		Transform TargetPlayer { get; }

		EntityType EntityType {get; }

		EnemyType EnemyType { get; }

		float MaxFieldOfViewAngle { get; }

		float MaxHeightDifference { get; }

		float AggroRange { get;  }

		float[] AttackRange { get; }
	} 
}
