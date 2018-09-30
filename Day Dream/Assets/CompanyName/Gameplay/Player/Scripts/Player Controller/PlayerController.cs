using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Sirenix.OdinInspector;

namespace AH.Max.Gameplay
{	
	[RequireComponent(typeof(AH.Max.Gameplay.PlayerLocomotion))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Animator))]
	public class PlayerController : AH.Max.Entity
	{
		[TabGroup("Set Up")]
		[SerializeField]
		private Transform playerCamera;

		public const string LeftStickHorizontal = "LeftStickHorizontal";
		public const string LeftStickVertical = "LeftStickVertical";

		public const string RightStickHorizontal  = "RightStickHorizontal";
		public const string RightStickVertical = "RightStickVertical";

		public const string KeyBoardHorizontal = "Horizontal";
		public const string KeyBoardVertical = "Vertical";

		public const string MouseHorizontal = "Mouse X";
		public const string MouseVertical = "Mouse Y";
		
		private const string RightShoulder_1 = "RightBumper";
		private const string RightShoulder_2 = "RightShoulderTwo";
		private const string LeftShoulder_1= "LeftShoulderOne";
		private const string LeftShoulder_2 = "LeftShoulderTwo";

		private const string ActionButtonBottomRow_1 = "AButton";
		private const string ActionButtonBottomRow_2 = "BButton";
		private const string ActionButtonTopRow_1 = "XButton";
		private const string ActionButtonTopRow_2 = "YButton";

		private Player input;
		public Player _input{get{return input;}}
		private PlayerAttack playerAttack;
		private PlayerStateManager playerStateManager;
		private PlayerCamera playerCameraComponent;
		private PlayerLocomotion playerLocomotion;
		private PlayerFreeClimb playerFreeClimb;

		private bool isGrounded = true;
		public bool IsGrounded { get { return isGrounded; } }

		private void Awake () 
		{
			InputSetUp();
			ComponentInitialization();
		}
		
		private void Update () 
		{
			PlatformingInput();
			CameraInput();
			CombatInput();
		}

		private void FixedUpdate ()
		{
			LocomotionInput();
		}

		private void LateUpdate () 
		{
			// CameraInput();
		}

		private void ComponentInitialization()
		{
			playerAttack = GetComponent<PlayerAttack>();
			playerStateManager = GetComponent <PlayerStateManager> ();
			playerCameraComponent = playerCamera.GetComponent <PlayerCamera> ();
			playerLocomotion = GetComponent <PlayerLocomotion> ();
			playerFreeClimb = GetComponent <PlayerFreeClimb> ();
		}

		private void InputSetUp()
		{
			input = ReInput.players.GetPlayer(0);
		}

		private void LocomotionInput()
		{
			//Check Player State
			if( playerStateManager.CurrentState != PlayerState.FreeMove) return;
			if( playerAttack.IsAttacking ) return;

			float h = input.GetAxis(LeftStickHorizontal);
			float v = input.GetAxis(LeftStickVertical);

			if(h == 0)
				h = Input.GetAxis(KeyBoardHorizontal);

			if(v == 0)
				v = Input.GetAxis(KeyBoardVertical);

			var moveDirection = new Vector3(h, 0, v);
			playerLocomotion.PlayerMove(playerCamera, moveDirection);
		}

		private void PlatformingInput(){
			if( input.GetButtonDown(ActionButtonBottomRow_1) || Input.GetKeyDown(KeyCode.Space) )
			{
				if(!playerFreeClimb.isClimbing)
				{
					if( playerFreeClimb.CheckForClimb() ) return;

					if( playerStateManager.CurrentState == PlayerState.FreeMove )
					{
						playerLocomotion.Jump();
					}
				}
			}
		}

		private void CameraInput()
		{
			float _x = input.GetAxis(RightStickHorizontal);
			float _y = input.GetAxis(RightStickVertical);

			if( _x == 0 )
			{
				_x = Input.GetAxis(MouseHorizontal);
			}

			if( _y == 0 )
			{
				_y = Input.GetAxis(MouseVertical);
			}

			playerCameraComponent.MouseOrbit(_x, _y);
		}

		private void CombatInput()
		{
			if(playerStateManager.CurrentState == PlayerState.Traversing) return;

			if(input.GetButtonDown(ActionButtonTopRow_1))
			{
				playerAttack.Attack();
			}
			else if(Input.GetMouseButtonDown(0))
			{
				playerAttack.Attack();
			}
		}
	}
}
