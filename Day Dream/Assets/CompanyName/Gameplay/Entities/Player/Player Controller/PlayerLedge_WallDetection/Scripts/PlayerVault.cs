using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

/* TODO you need to make sure you cant move in the vault state as well as others. Its because its overriding your warp DO NOT FORGET */

namespace AH.Max.Gameplay
{
	public enum VaultType
	{
		Over, /* This is when the player is vault OVER something like a fence or low wall. The key is that they are landing at the same height they started. */
		Mount /* The is when the player is moving up to another platform. The key is that they are landing on a higher plane */
	}

	[Serializable]
	public class VaultData
	{
		/// <summary>
		/// Name of the animation
		/// </summary>
		public string animationName;

		/// <summary>
		/// The maximum Hieght of the ledge/wall that this vault can accommodate.
		/// </summary>
		public float maxHeight;
		
		/// <summary>
		/// The start time in the animation that we want to start warping
		/// </summary>
		[SerializeField]
		private float startTime;
		public float startTimeDelta
		{
			get
			{
 				return startTime / 100;
			}
		}

        /// <summary>
        /// The start time in the animation that we want to start warping
        /// </summary>
        [SerializeField]
        private float wallOffset;
        public float WallOffset {
            get 
            {
                return wallOffset;
            }
        }

        /// <summary>
        /// The end time in the animation that we want to stop warping
        /// </summary>
        [SerializeField]
		private float endTime;
		public float endTimeDelta
		{
			get
			{
				return endTime / 100;
			}
		}

		/// <summary>
		/// the type vault that is 
		/// </summary>
		[SerializeField]
		private VaultType vaultType;
		public VaultType VaultType
		{
			get
			{
				return vaultType;
			}
		}

		/// <summary>
		/// The body part that we want to target with a warp
		/// </summary>
		[Tooltip("The body part that we want to target with a warp")]
		[SerializeField]
		private AvatarTarget avatarTarget;
		public AvatarTarget AvatarTarget
		{
			get
			{
				return avatarTarget;
			}
		}
	}

	public class PlayerVault : MonoBehaviour 
	{
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float wallOffset;

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float yOffset;


		[SerializeField]
		private List <VaultData> vaultDatas = new List <VaultData>();

		[ShowInInspector]		
		private bool isVaulting;
		public bool IsVaulting
		{
			get
			{
				return isVaulting;
			}
		}

        /// <summary>
        /// The states where the vault action can be done
        /// </summary>
        [Tooltip("The states where the vault action can be done")]
        public PlayerState[] availableStates;

        private Animator animator;
		private Transform helper;
		private PlayerElevationDetection playerElevationDetection;
        private PlayerStateComponent playerStateComponent;

        [SerializeField]
        float delta;

		private void Start()
		{
			animator = GetComponent<Animator>();
			playerElevationDetection = GetComponent <PlayerElevationDetection>();
            playerStateComponent = GetComponent<PlayerStateComponent>();

			helper = new GameObject().transform;
			helper.name = "Vault Helper";

			vaultDatas = SortVaultData(vaultDatas);
		}

		private void OnEnable()
		{
			InputDriver.jumpButtonEvent.AddListener(Vault);
		}

		private void OnDisable() 
		{
			InputDriver.jumpButtonEvent.RemoveListener(Vault);
		}

		private void FixedUpdate()
		{
			VaultAniamtionWarp();
		} 

		private void Vault()
		{
            if(!CheckVaultConditions())
            {
                return;
            }

			if(playerElevationDetection.ValidLedge)
			{
				helper.position = playerElevationDetection.Ledge;
				helper.rotation = Quaternion.LookRotation(-playerElevationDetection.WallNormal);

                playerStateComponent.SetStateHard(PlayerState.Traversing);

				PlayVaultAnimation();
			}
		}

        private bool CheckVaultConditions()
        {
            foreach(var _state in availableStates)
            {
                if (playerStateComponent.CheckState(_state))
                {
                    return true;
                }
            }

            return false;
        }

		public void PlayVaultAnimation()
		{
			float heightDifference = ( helper.position.y > transform.position.y ? helper.position.y : transform.position.y) - ( helper.position.y > transform.position.y ? transform.position.y : helper.position.y);

			List <VaultData> _vaultDatas = playerElevationDetection.VaultType == VaultType.Mount ? GetVaultDatas(VaultType.Mount) : GetVaultDatas(VaultType.Over);

			foreach(VaultData _data in _vaultDatas)
			{
				if(heightDifference <= _data.maxHeight)
				{	
					isVaulting = true;
					animator.Play(_data.animationName);
                    GetComponent<Rigidbody>().isKinematic = true;

                    //StartCoroutine(MoveIntoPosition(helper.position, _data));

                    return;
				}
			}
		}

        private IEnumerator MoveIntoPosition(Vector3 ledgePosition, VaultData data)
        {
            ledgePosition = PositionWithOffset(ledgePosition, data.WallOffset);

            GetComponent<Rigidbody>().isKinematic = true;

            while (Vector3.Distance(transform.position, ledgePosition) > 0.01f)
            {
                transform.position = Vector3.Slerp(transform.position, ledgePosition, delta);
                yield return new WaitForEndOfFrame();
            }

            GetComponent<Rigidbody>().isKinematic = false;

            yield break;
        }

		private void VaultAniamtionWarp()
		{
			//TODO - REFACTOR - 
			// set an active animation from the play method and then warp during that one. 
			//Have 2 animation events fire- one for start warping and one to stop
			//start by rotating the player towards the helper

			foreach (VaultData _data in vaultDatas)
			{
		    	if(animator.GetCurrentAnimatorStateInfo(0).IsName(_data.animationName))
				{
                    Vector3 _targetPos = helper.position;
                    _targetPos.y += _data.WallOffset;

					animator.MatchTarget(_targetPos, helper.rotation, _data.AvatarTarget, new MatchTargetWeightMask( Vector3.one, 0), _data.startTimeDelta, _data.endTimeDelta);

                    break;
				}
                //StartCoroutine(SlightDelay());
                //GetComponent<Rigidbody>().isKinematic = false;
			}
		}

        IEnumerator SlightDelay()
        {
            yield return new WaitForSeconds(1);


            yield break;
        }

		/// <summary>
		/// A method that sorts the given list of vault data by their height, lowest to highest
		/// </summary>
		/// <param name="listToSort"></param>
		/// <returns></returns>
		private List <VaultData> SortVaultData(List <VaultData> listToSort)
		{
			int _targetCount = listToSort.Count;
			List <VaultData> sortedList = new List <VaultData>();

			while(sortedList.Count < _targetCount)
			{
				VaultData _currentLowest = listToSort[0];
				
				foreach(var _data in listToSort)
				{
					if(_data.maxHeight <+ _currentLowest.maxHeight)
					{
						_currentLowest = _data;
					}
				}

				sortedList.Add(_currentLowest);
				listToSort.Remove(_currentLowest);
			}

			return sortedList;
		}

		/// <summary>
		/// Gets every vault data of the given type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private List <VaultData> GetVaultDatas(VaultType type)
		{
			return vaultDatas.FindAll(_vaultData => _vaultData.VaultType == type);
		}

		private Vector3 PositionWithOffset( Vector3 tp, float offset/*, Vector3 wallNormal*/ )
		{
			//tp -= wallNormal * wallOffset;
			tp.y += offset;

			return tp;
		} 

		#region  Animation Events
		public void VaultEnd()
		{
			isVaulting = false;
		    GetComponent<Rigidbody>().isKinematic = false;

            if (playerStateComponent.CheckState(PlayerState.Traversing))
            {
                playerStateComponent.ResetState();
            }
        }
        #endregion
    }
}

