using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AH.Max
{
	public class GameManager : Singleton_MonoBehavior<GameManager>
	{
		private static GameStates gameState;
		public static GameStates GameState
		{
			get { return gameState; }
		}

		protected override void Enable() 
		{
		}

		protected override void Disable() 
		{
		}

		private void SetState(GameStates _state)
		{
			gameState = _state;
		}
	}
}
