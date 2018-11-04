using System.Collections;
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
		
		///<Summary>
		/// This is the input values, not the actual direction from the player
		///</Summary>
		private Vector3 inputDirection;
		public Vector3 InputDirection
		{
			get { return inputDirection; }
		}

		///<Summary>
		/// This is the actual direction the player should be moving. Becuase the player is moving with animation a more 
		/// accuarate number would be to get the players current velocity from the rigidbody
		///</Summary>
		private Vector3 moveDirection;
		public Vector3 MoveDirection
		{
			get { return moveDirection; }
		}

		private Player input;
		public Player _input{get{return input;}}

		private PlayerAttack playerAttack;
		private PlayerStateManager playerStateManager;
		private PlayerCamera playerCameraComponent;
		private PlayerLocomotion playerLocomotion;
		private PlayerFreeClimb playerFreeClimb;
		private PlayerEvade playerEvade;

		private bool isGrounded = true;
		public bool IsGrounded { get { return isGrounded; } }

		private bool isSprinting = false;
		public bool IsSprinting { get {return isSprinting; } }

		private void Awake () 
		{
			InputSetUp();
			ComponentInitialization();
		}

		private void OnEnable()
		{
			EntityManager.Instance.SetPlayer(this);
		}

		private void OnDisable()
		{
			EntityManager.Instance.SetPlayer(null);
		}
		
		private void Update () 
		{
			PlatformingInput();
			CombatInput();
			LocomotionInput();
			SprintingInput();
			EvadingInput();
		}

		private void FixedUpdate ()
		{
		}

		private void LateUpdate () 
		{
			CameraInput();
		}

		private void ComponentInitialization()
		{
			playerAttack = GetComponent<PlayerAttack>();
			playerStateManager = GetComponent <PlayerStateManager> ();
			playerCameraComponent = playerCamera.GetComponent <PlayerCamera> ();
			playerLocomotion = GetComponent <PlayerLocomotion> ();
			playerFreeClimb = GetComponent <PlayerFreeClimb> ();
			playerEvade = GetComponent<PlayerEvade>();
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

			this.inputDirection = moveDirection;
			CalculateMoveDirection();
		}

		///<Summary>
		/// Simply comes up with the actual direction the player "should be moving".
		///</Summary>
		private void CalculateMoveDirection()
		{
			Vector3 direction = transform.position + moveDirection;
			direction = direction - transform.position;

			moveDirection = direction;
		}

		private void SprintingInput()
		{
			if(Input.GetKey(KeyCode.LeftShift))
			{
				isSprinting = true;
			}
			else
			{
				isSprinting = false;
			}
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

		private void EvadingInput()
		{
			if( playerStateManager.CurrentState != PlayerState.FreeMove) return;
			if( playerAttack.IsAttacking ) return;
			if(!playerStateManager.IsGrounded) return;

			if(input.GetButtonDown(ActionButtonBottomRow_2) || Input.GetKeyDown(KeyCode.E))
			{
				playerEvade.CombatRoll(moveDirection);
			}
		}

		///<Summary>
		/// Use this method to set player rotation so it is easy to see where we are doing this downt the line
		///</Summary>
		public void SetPlayerRotation(Quaternion rotation)
		{
			transform.rotation = rotation;
		}

		///<Summary>
		/// Use this method to set player position so it is easy to see where we are doing this downt the line
		///</Summary>
		public void SetPlayerPosition(Vector3 position)
		{
			transform.position = position;
		}
	}
}
