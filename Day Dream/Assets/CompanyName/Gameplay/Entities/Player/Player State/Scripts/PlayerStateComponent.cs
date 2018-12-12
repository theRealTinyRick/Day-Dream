using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class PlayerStateComponent : MonoBehaviour 
{
	[TabGroup(Tabs.Properties)]
	[SerializeField]
	private PlayerState currentState = PlayerState.FreeMove;
	public PlayerState CurrentState
	{
		get
		{
			return currentState;
		}
		private set
		{
			//could ad an event handle here
			currentState = value;
		}
	}

	private PlayerAttackAnimationController playerAttackAnimationController;
	private PlayerGroundedComponent playerGroundedComponent;

	private void Start() 
	{
		playerAttackAnimationController = GetComponent<PlayerAttackAnimationController>();
	}

	public void SetStateHard(PlayerState state)
	{
		currentState = state;
	}
}
