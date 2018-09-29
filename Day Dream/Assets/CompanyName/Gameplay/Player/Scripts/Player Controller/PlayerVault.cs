using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerVault : MonoBehaviour 
	{
		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float wallOffset;

		public const string VaultLow = "VaultLow";
		public const string VaultMed = "VaultMed";
		public const string VaultHigh = "VaultHigh";

		private Animator animator;
		private PlayerStateManager playerStateManager;

		private Transform helper;

		private void Start()
		{
			animator = GetComponent<Animator>();
			playerStateManager = GetComponent<PlayerStateManager>();

			helper = new GameObject().transform;
			helper.name = "Vault Helper";
		}

		private void Update()
		{
			VaultAniamtionWarp();
		}

		private void VaultAniamtionWarp()
		{
			if( animator.GetCurrentAnimatorStateInfo(0).IsName(VaultLow) )
			{
				const float startTime = 012.9f / 100;
				const float endTime = 027.0f / 100;
				animator.MatchTarget( helper.position, helper.rotation, AvatarTarget.RightFoot, new MatchTargetWeightMask( Vector3.one, 0 ), startTime, endTime );
			}
			else if( animator.GetCurrentAnimatorStateInfo(0).IsName(VaultMed) )
			{
				const float startTimeOne = 1 / 100;
				const float endTimeOne = 06.4f / 100;
				animator.MatchTarget( helper.position, helper.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask( Vector3.one, 0 ), startTimeOne, endTimeOne );

				const float startTime = 06.5f / 100;
				const float endTime = 016.9f / 100;
				animator.MatchTarget( helper.position, helper.rotation, AvatarTarget.LeftFoot, new MatchTargetWeightMask( Vector3.one, 0 ), startTime, endTime );
			}
			else if( animator.GetCurrentAnimatorStateInfo(0).IsName(VaultHigh) )
			{
				const float startTime = 002.6f / 100;
				const float endTime = 020.3f / 100;
				animator.MatchTarget( helper.position, helper.rotation, AvatarTarget.RightHand, new MatchTargetWeightMask( Vector3.one, 0 ), startTime, endTime );
			}
		}

		public void FindActualPosition( Vector3 vaultPosition, LayerMask layerMask, PlayerElevationDetection source )
		{
			playerStateManager.SetStateHard(PlayerState.Traversing);
			
			Vector3 origin = transform.position;
			origin.y += 0.3f;

			RaycastHit hit;

			if(Physics.Raycast( origin, transform.forward, out hit, source.DistanceToCheck, layerMask ))
			{
				Vector3 tp = new Vector3( hit.point.x, vaultPosition.y, hit.point.z );
				helper.position = PositionWithOffset( tp, hit.normal );

				PlayVaultAnimation();
			}
		}

		public void PlayVaultAnimation()
		{
			float heightDifference = ( helper.position.y > transform.position.y ? helper.position.y : transform.position.y) - ( helper.position.y > transform.position.y ? transform.position.y : helper.position.y);
			Debug.Log( heightDifference );
			if( heightDifference <= 0.7f )
			{
				Debug.Log("Low Vault");
				animator.Play( VaultLow );
				// animator.Play(VaultMed);
			}
			else if( heightDifference <= 2)
			{
				Debug.Log("Med vault");
				animator.Play( VaultMed );
			}
			else if( heightDifference <= 3.5 )
			{
				animator.Play( VaultHigh );
			}
		}

		private Vector3 PositionWithOffset( Vector3 tp, Vector3 wallNormal )
		{
			tp -= wallNormal * wallOffset;

			return tp;
		} 
	}
}

