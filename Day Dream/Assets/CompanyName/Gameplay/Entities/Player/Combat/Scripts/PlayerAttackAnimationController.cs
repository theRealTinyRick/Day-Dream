using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

using AH.Max.Gameplay.System.Components;

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
                if(currentAnimSet == null)
                {
                    return 0;
                }

                return currentAnimSet.Length;
            }
        }
		
		private int currentNumberOfClicks = 0;

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float timeToClick;

		[TabGroup(Tabs.Properties)]
		[ShowInInspector]
		private float time = 0;

        [TabGroup(Tabs.Properties)]
        [ShowInInspector]
        private List<string> nonAttackableStates = new List<string>();

        [TabGroup(Tabs.Properties)]
        [ShowInInspector]
        private string groundedState = "IsGrounded";

        [HideInInspector]
        public string[] swingBooleans = new string[] {"Swing1", "Swing2", "Swing3", "Swing4", "Swing5", "Swing6"};

        private string[] currentAnimSet;
        private WeaponType currentWeaponType;
        private bool hasAttacked;

        private Animator animator;
        private StateComponent stateComponent;
        private PlayerGroundedComponent playerGroundedComponent;

        //events
        [TabGroup(Tabs.Events)]
        [SerializeField]
        public AttackStartedEvent attackStartedEvent = new AttackStartedEvent();

        [TabGroup(Tabs.Events)]
        [SerializeField]
        public AttackEndedEvent attackEndedEvent = new AttackEndedEvent();

        void Start () 
		{
			animator = GetComponent<Animator>();
            playerGroundedComponent = GetComponent<PlayerGroundedComponent>();
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

        // this method simply determines if the player is still clicking. 
        // if the player keeps clicking then stops then the attacks should stop as well. 
        private void AttackTimer()
		{
            if(IsAttacking)
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
		}

		public void StopAttacking()
		{
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
            hasAttacked = false;
		}
		
		///<Summary>
		/// The input reciever for the standar attack in the game
		///</Summary>
		private void QuereyAttack()
		{
            if(currentWeaponType == null)
            {
                return;
            }

            if(currentWeaponType.handedness == Handedness.EmptyHands)
            {
                return;
            }

            if(currentAnimSet == null)
            {
                currentAnimSet = currentWeaponType.animations;
            }

			if(EvaluateQueueConditions())
			{
                hasAttacked = true;

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

                attackStartedEvent.Invoke();
				isAttacking = true;
			}
		}

		private bool EvaluateQueueConditions()
		{
            bool _inProperState = !stateComponent.AnyStateTrue(nonAttackableStates) && stateComponent.GetState(groundedState);

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
            if(currentAnimSet != null)
            {
			    foreach(var _animation in currentAnimSet)
			    {
				    foreach(var thing in animator.GetCurrentAnimatorClipInfo(0))
				    {
					    if(_animation == thing.clip.name)
					    {
						    return true;
					    }
				    }
			    }

                attackEndedEvent.Invoke();
            }

            return false;
		}

        /// <summary>
        /// A response to the tool component getting a weapon
        /// </summary>
        /// <param name="weaponType"></param>
        public void OnToolEquipped(WeaponType weaponType)
        {
            currentWeaponType = weaponType;
            currentAnimSet = currentWeaponType.animations;
        }

		public void AttackEndEvent()
		{

		}
	}
}
