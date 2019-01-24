using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerAttackAnimationController : MonoBehaviour 
	{
		[TabGroup(Tabs.Properties)]
		[ShowInInspector]
		private bool isAttacking = false;
		public bool IsAttacking{ get{ return isAttacking; } }

		[TabGroup(Tabs.Debug)]
		[Tooltip("This field is only used for debuging and has no barring on the functionality of queuing animations. That is done in the animator controller")]
		[SerializeField]
		private List <string> queue = new List<string>();

        private int maxNumberOfClicks 
        {
            get 
            {
                return currentAnimSet.Length;
            }
        }
		
		[TabGroup(Tabs.Properties)]
		[ShowInInspector]
		private int currentNumberOfClicks = 0;

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float timeToClick;

		[TabGroup(Tabs.Properties)]
		[ShowInInspector]
		private float time = 0;

        [SerializeField]
		private string[] swordAnimations;

        [SerializeField]
        private string[] twinSwordAnimations;

        [SerializeField]
        private string[] heavySwordAnimations;

        private string[] currentAnimSet;

        [HideInInspector]
        public string[] swingBooleans = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};

        [SerializeField]
        private PlayerState[] availableStates;
		
		private Animator animator;
		private PlayerStateComponent playerStateComponent;
        private PlayerGroundedComponent playerGroundedComponent;

		void Start () 
		{
			animator = GetComponent<Animator>();
			playerStateComponent = GetComponent<PlayerStateComponent>();
            playerGroundedComponent = GetComponent<PlayerGroundedComponent>();

            currentAnimSet = swordAnimations;
		}

		private void OnEnable()
		{
			InputDriver.lightAttackButtonEvent.AddListener(QuereyAttack);
		}

		private void OnDisable()
		{
			InputDriver.lightAttackButtonEvent.RemoveListener(QuereyAttack);
		}

		private void Update()
		{
			AttackTimer();
			CurrentlyInAttackState();
		}

        [Button]
        public void UseTwinSwords()
        {
            if(!IsAttacking)
            {
                currentAnimSet = twinSwordAnimations;
            }
        }

        [Button]
        public void UseSwordAndShield()
        {
            if (!IsAttacking)
            {
                currentAnimSet = swordAnimations;
            }
        }

        [Button]
        public void UseHeavySword()
        {
            if (!IsAttacking)
            {
                currentAnimSet = heavySwordAnimations;
            }
        }

        // this method simply determines if the player is still clicking. 
        // if the player keeps clicking then stops then the attacks should stop as well. 
        private void AttackTimer()
		{
			if(queue.Count > 0)
			{
				time += Time.deltaTime;

				if(time > timeToClick)
				{
					StopAttacking();
				}
			}
		}

		public void StopAttacking()
		{
			// clear out the queue and stop attacking
			foreach(string _animation in swingBooleans)
			{
				if(_animation != swingBooleans[0])
				{
					animator.SetBool(_animation, false);
				}
			}

			queue.Clear();
			currentNumberOfClicks = 0;
			time = 0;
			isAttacking = false;
		}
		
		///<Summary>
		/// The input reciever for the standar attack in the game
		///</Summary>
		private void QuereyAttack()
		{
			if(EvaluateQueueConditions())
			{
				// SET THE PLAYER STATE
				playerStateComponent.SetStateHard(PlayerState.Attacking);

				currentNumberOfClicks ++;
				time = 0;

				int _index = currentNumberOfClicks - 1;	

				queue.Add(currentAnimSet[_index]);

				if(_index == 0)
				{
					animator.Play(currentAnimSet[0]);
				}
				else
				{
					animator.SetBool(swingBooleans[_index], true);
				}

				isAttacking = true;
				playerStateComponent.SetStateHard(PlayerState.Attacking);
			}
		}

		private bool EvaluateQueueConditions()
		{
            bool _inProperState = false;

            foreach (PlayerState _state in availableStates)
            {
                if (playerStateComponent.CheckState(_state))
                {
                    _inProperState = true;
                }
            }

            if(!playerGroundedComponent.IsGrounded)
            {
                return false;
            }

            if(!_inProperState)
            {
                return false;
            }

            if (currentNumberOfClicks < maxNumberOfClicks)
			{
				return true;
			}
			else
			{
				time = 0;
			}

            return false;
		}

		public bool CurrentlyInAttackState()
		{
			foreach(var _swordAnimation in currentAnimSet)
			{
				foreach(var thing in animator.GetCurrentAnimatorClipInfo(0))
				{
					if(_swordAnimation == thing.clip.name)
					{
						return true;
					}
				}
			}

			if(playerStateComponent.CurrentState == PlayerState.Attacking)
			{
				playerStateComponent.ResetState();
			}			

			return false;
		}

		public void AttackEndEvent()
		{

		}
	}
}
