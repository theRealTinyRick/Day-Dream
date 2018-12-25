using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.Gameplay;

namespace AH.Max.Gameplay
{
	public class PlayerStateComponent : MonoBehaviour 
	{
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private PlayerState currentState = PlayerState.Normal;
		public PlayerState CurrentState
		{
			get
			{
				return currentState;
			}
			private set
			{
				currentState = value;
			}
		}

		private PlayerAttackAnimationController playerAttackAnimationController;
		private PlayerGroundedComponent playerGroundedComponent;
		private PlayerVault playerVault;

		private void Start() 
		{
			playerAttackAnimationController = GetComponent<PlayerAttackAnimationController>();
			playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
			playerVault = GetComponent<PlayerVault>();
		}

		public void SetStateHard(PlayerState state)
		{
			currentState = state;
		}

		public void ResetState()
		{
			currentState = PlayerState.Normal;
		}

        public bool CheckState(PlayerState state)
        {
            if(currentState == state)
            {
                return true;
            }

            return false;
        }

	}
}
