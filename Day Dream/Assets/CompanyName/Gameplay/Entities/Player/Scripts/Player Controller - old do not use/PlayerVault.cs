using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{
	public class PlayerVault : MonoBehaviour 
	{
		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float wallOffset;

		[TabGroup(Tabs.Properties)]
		[SerializeField]
		private float yOffset;

		public const string VaultLow = "VaultLow";
		public const string VaultMed = "VaultMed";
		public const string VaultHigh = "VaultHigh";
		public const string VaultHighest = "VaultHighest";

		private Animator animator;
		private PlayerElevationDetection playerElevationDetection;

		private Transform helper;

		private void Start()
		{
			animator = GetComponent<Animator>();
			playerElevationDetection = GetComponent <PlayerElevationDetection>();

			helper = new GameObject().transform;
			helper.name = "Vault Helper";
		}

		private void OnEnable()
		{
			InputDriver.jumpButtonEvent.AddListener(Vault);
		}

		private void OnDisable() 
		{
			InputDriver.jumpButtonEvent.RemoveListener(Vault);
		}

		private void Update()
		{
			VaultAniamtionWarp();
		} 

		private void Vault()
		{
			if(playerElevationDetection.ValidLedge)
			{
				helper.position = playerElevationDetection.Ledge;
				helper.rotation = Quaternion.LookRotation(-playerElevationDetection.WallNormal);

				PlayVaultAnimation();
			}
		}

		public void PlayVaultAnimation()
		{
			Debug.Log("Play");
			float heightDifference = ( helper.position.y > transform.position.y ? helper.position.y : transform.position.y) - ( helper.position.y > transform.position.y ? transform.position.y : helper.position.y);

			if( heightDifference <= 0.7f )
			{
			Debug.Log("Low");
			
				animator.Play( VaultLow );
			}
			else if( heightDifference <= 2)
			{
			Debug.Log("Med");

				animator.Play( VaultMed );
			}
			else if( heightDifference <= 3.5)
			{
				Debug.Log("High");

				animator.Play( VaultHigh );
			}
		}

		public void FindActualPosition( Vector3 vaultPosition, LayerMask layerMask, PlayerElevationDetection source )
		{
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

		private void VaultAniamtionWarp()
		{
			//TODO - REFACTOR - 
			// set an active animation from the play method and then warp during that one. 
			//Have 2 animation events fire- one for start warping and one to stop
			//start by rotating the player towards the helper


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

		private Vector3 PositionWithOffset( Vector3 tp, Vector3 wallNormal )
		{
			tp -= wallNormal * wallOffset;
			tp.y += yOffset;

			return tp;
		} 
	}
}

