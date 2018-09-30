using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max.Gameplay.AI
{
	public interface IAgent
	{
		bool CheckLineOfSight();
		bool CheckFieldOfView();
		bool CheckHeightDifference();

		void Move();
	} 
}
