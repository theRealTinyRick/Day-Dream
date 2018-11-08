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

		[TabGroup(Tabs.Preferences)]
		[SerializeField]
		private float yOffset;

		public const string VaultLow = "VaultLow";
		public const string VaultMed = "VaultMed";
		public const string VaultHigh = "VaultHigh";
		public const string VaultHighest = "VaultHighest";

		private Animator animator;
		private PlayerController playerController;
		private PlayerStateManager playerStateManager;

		private Transform helper;

		private void Start()
		{
			animator = GetComponent<Animator>();
			playerController = GetComponent<PlayerController>();
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
				const float startTime = 01.0f / 100;
				const float endTime = 025.0f / 100;
				animator.MatchTarget( helper.position, helper.rotation, AvatarTarget.RightFoot, new MatchTargetWeightMask( Vector3.one, 0 ), startTime, endTime );
			}
			else if( animator.GetCurrentAnimatorStateInfo(0).IsName(VaultMed) )
			{
				const float startTime = 01.0f / 100;
				const float endTime = 024.0f / 100;
				animator.MatchTarget( helper.position, helper.rotation, AvatarTarget.LeftFoot, new MatchTargetWeightMask( Vector3.one, 0 ), startTime, endTime );
			}
			else if( animator.GetCurrentAnimatorStateInfo(0).IsName(VaultHigh) )
			{
				const float startTime = 01.0f / 100;
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
			if( heightDifference <= 0.7f )
			{
				animator.Play( VaultLow );
			}
			else if( heightDifference <= 2)
			{
				animator.Play( VaultMed );
			}
			else if( heightDifference <= 3.5 && playerController.IsGrounded )
			{
				animator.Play( VaultHigh );
			}
		}

		private Vector3 PositionWithOffset( Vector3 tp, Vector3 wallNormal )
		{
			tp -= wallNormal * wallOffset;
			tp.y += yOffset;

			return tp;
		} 
	}
}

