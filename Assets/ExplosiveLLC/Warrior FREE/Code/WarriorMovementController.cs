﻿using UnityEngine;

namespace WarriorAnimsFREE
{
	public class WarriorMovementController : SuperStateMachine
	{
		[Header("Components")]
		private WarriorController warriorController;

		[Header("Movement")]
		public float movementAcceleration = 90.0f;
		public float runSpeed = 6f;
		private readonly float rotationSpeed = 10f;
		public float groundFriction = 50f;
		[HideInInspector] public Vector3 currentVelocity;

		[Header("Jumping")]
		public float gravity = 25.0f;
		public float jumpAcceleration = 5.0f;
		public float jumpHeight = 3.0f;
		public float inAirSpeed = 6f;

        float _timer = 0;

        [HideInInspector] public Vector3 lookDirection { get; private set; }

		bool isKnockBack = false;
		[SerializeField] GameObject gameManager;
		GameManager manager;

		private void Start()
		{
			warriorController = GetComponent<WarriorController>();
			manager = gameManager.GetComponent<GameManager>();
			// Set currentState to idle on startup.
			currentState = WarriorState.Idle;
		}

		#region Updates

		/*void Update () {
		 * Update is normally run once on every frame update. We won't be using it in this case, since the SuperCharacterController component sends a callback Update called SuperUpdate. 
		 * SuperUpdate is recieved by the SuperStateMachine, and then fires further callbacks depending on the state.
		}*/

		// Put any code in here you want to run BEFORE the state's update function. 
		// This is run regardless of what state you're in.
		protected override void EarlyGlobalSuperUpdate()
		{
		}

		// Put any code in here you want to run AFTER the state's update function.  
		// This is run regardless of what state you're in.
		protected override void LateGlobalSuperUpdate()
		{
			// Move the player by our velocity every frame.
			transform.position += currentVelocity * warriorController.superCharacterController.deltaTime;

			// If alive and is moving, set animator.
			if (warriorController.canMove) {
				if (currentVelocity.magnitude > 0 && warriorController.HasMoveInput()) {
					warriorController.isMoving = true;
					warriorController.SetAnimatorBool("Moving", true);
					warriorController.SetAnimatorFloat("Velocity", currentVelocity.magnitude);
				} else {
					warriorController.isMoving = false;
					warriorController.SetAnimatorBool("Moving", false);
					warriorController.SetAnimatorFloat("Velocity", 0);
				}
			}

			RotateTowardsMovementDir();

			// Update animator with local movement values.
			warriorController.SetAnimatorFloat("Velocity", transform.InverseTransformDirection(currentVelocity).z);
		}

        public void BulletFire()
        {
			manager.BulletShoot();
        }

        #endregion

        #region Gravity / Jumping

        public void RotateGravity(Vector3 up)
		{
			lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
		}

		// Calculate the initial velocity of a jump based off gravity and desired maximum height attained.
		private float CalculateJumpSpeed(float jumpHeight, float gravity)
		{
			return Mathf.Sqrt(2 * jumpHeight * gravity);
		}

		#endregion

		#region States

		// Below are the state functions. 
		// Each one is called based on the name of the state, so when currentState = Idle, we call Idle_EnterState. 
		// If currentState = Jump, we call Jump_SuperUpdate()
		private void Idle_EnterState()
		{
			warriorController.superCharacterController.EnableSlopeLimit();
			warriorController.superCharacterController.EnableClamping();
			warriorController.LockJump(false);
			warriorController.SetAnimatorInt("Jumping", 0);
			warriorController.SetAnimatorBool("Moving", false);
		}

		// Run every frame we are in the idle state.
		private void Idle_SuperUpdate()
		{
			if (!GameManager.cleared)
			{
                if (warriorController.takeExplodeDamage)
                {
                    currentState = WarriorState.Explode_Damage;
                    return;
                }
                if (warriorController.takeExplodeDamage && !warriorController.MaintainingGround())
                {
                    currentState = WarriorState.ExplodeFall;
                    return;
                }
                // If Jump.
                if (warriorController.canJump && warriorController.inputJump)
                {
                    currentState = WarriorState.Jump;
                    return;
                }
                // In air.
                if (!warriorController.takeExplodeDamage && !warriorController.MaintainingGround())
                {
                    currentState = WarriorState.Fall;
                    return;
                }
                if (warriorController.HasMoveInput() && warriorController.canMove)
                {
                    currentState = WarriorState.Move;
                    return;
                }
                // Apply friction to slow to a halt.
                currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction
                    * warriorController.superCharacterController.deltaTime);
            }
		}

		// Run once when exit the idle state.
		private void Idle_ExitState()
		{
		}

		// Run once when exit the idle state.
		private void Idle_MoveState()
		{
			warriorController.SetAnimatorBool("Moving", true);
		}

		private void Move_SuperUpdate()
		{
			if (!GameManager.cleared)
			{
                if (warriorController.takeExplodeDamage)
                {
                    currentState = WarriorState.Explode_Damage;
                    return;
                }
                if (warriorController.takeExplodeDamage && !warriorController.MaintainingGround())
                {
                    currentState = WarriorState.ExplodeFall;
                    return;
                }
                // If Jump.
                if (warriorController.canJump && warriorController.inputJump)
                {
                    currentState = WarriorState.Jump;
                    return;
                }
                // Fallling.
                if (!warriorController.takeExplodeDamage && !warriorController.MaintainingGround())
                {
                    currentState = WarriorState.Fall;
                    return;
                }
                // Set speed determined by movement type.
                if (warriorController.HasMoveInput() && warriorController.canMove)
                {
                    currentVelocity = Vector3.MoveTowards(currentVelocity, warriorController.moveInput
                        * runSpeed, movementAcceleration
                        * warriorController.superCharacterController.deltaTime);
                }
                else
                {
                    currentState = WarriorState.Idle;
                }
            }
		}

		private void Jump_EnterState()
		{
			warriorController.SetAnimatorInt("Jumping", 1);
			warriorController.SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);
			warriorController.superCharacterController.DisableClamping();
			warriorController.superCharacterController.DisableSlopeLimit();
			currentVelocity += warriorController.superCharacterController.up * CalculateJumpSpeed(jumpHeight, gravity);
			warriorController.LockJump(true);
			warriorController.Jump();
		}

		private void Jump_SuperUpdate()
		{
			Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(warriorController.superCharacterController.up, currentVelocity);
			Vector3 verticalMoveDirection = currentVelocity - planarMoveDirection;

			// Falling.
			if (warriorController.takeExplodeDamage) {
				currentState = WarriorState.Explode_Damage;
				return;
			}
			else if (currentVelocity.y < 0) {
				currentVelocity = planarMoveDirection;
				currentState = WarriorState.Fall;
				return;
			}

			planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, warriorController.moveInput * inAirSpeed, jumpAcceleration * warriorController.superCharacterController.deltaTime);
			verticalMoveDirection -= warriorController.superCharacterController.up * gravity * warriorController.superCharacterController.deltaTime;
			currentVelocity = planarMoveDirection + verticalMoveDirection;
		}

		private void Fall_EnterState()
		{
			warriorController.superCharacterController.DisableClamping();
			warriorController.superCharacterController.DisableSlopeLimit();
			warriorController.LockJump(false);
			warriorController.SetAnimatorInt("Jumping", 2);
			warriorController.SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);
		}

		private void Fall_SuperUpdate()
		{
			if (warriorController.takeExplodeDamage)
			{
				currentState = WarriorState.Explode_Damage;
				return;
			}
			// Landing.
			else if (warriorController.AcquiringGround()) {
				currentVelocity = Math3d.ProjectVectorOnPlane(warriorController.superCharacterController.up, currentVelocity);
				currentState = WarriorState.Idle;
				return;
			}

			// Normal gravity.
			currentVelocity -= warriorController.superCharacterController.up * gravity * warriorController.superCharacterController.deltaTime;
		}

		private void Fall_ExitState()
		{
			warriorController.SetAnimatorInt("Jumping", 0);
			warriorController.SetAnimatorTrigger(AnimatorTrigger.JumpTrigger);

			// Landed.
			if (warriorController.AcquiringGround()) { warriorController.Land(); }
		}

		private void Explode_Damage_EnterState()
		{
			
			isKnockBack = true;
			warriorController.SetAnimatorInt("Jumping", 1);
			warriorController.SetAnimatorTrigger(AnimatorTrigger.CriticalDamageTrigger);
			warriorController.superCharacterController.DisableClamping();
			warriorController.superCharacterController.DisableSlopeLimit();
			currentVelocity = Vector3.zero;
			currentVelocity += warriorController.superCharacterController.up  * CalculateJumpSpeed(jumpHeight, gravity) + warriorController.transform.forward * -1;
			warriorController.LockJump(true);
			warriorController.Jump();
		}

		private void Explode_Damage_SuperUpdate()
		{
			Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(warriorController.superCharacterController.up, currentVelocity);
			Vector3 verticalMoveDirection = currentVelocity - planarMoveDirection;

			// Falling.
			if (currentVelocity.y < 2)
			{
				currentVelocity = planarMoveDirection;
				currentState = WarriorState.ExplodeFall;
				return;
			}

			planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, warriorController.transform.forward * -1 * inAirSpeed, jumpAcceleration * warriorController.superCharacterController.deltaTime);
			verticalMoveDirection -= warriorController.superCharacterController.up * gravity * warriorController.superCharacterController.deltaTime;
			currentVelocity = planarMoveDirection + verticalMoveDirection;
		}

		private void ExplodeFall_EnterState()
		{
			warriorController.superCharacterController.DisableClamping();
			warriorController.superCharacterController.DisableSlopeLimit();
			warriorController.SetAnimatorInt("Jumping", 2);
			warriorController.SetAnimatorTrigger(AnimatorTrigger.CriticalDamageTrigger);
		}

		private void ExplodeFall_SuperUpdate()
		{
			// Landing.
			if (warriorController.AcquiringGround())
			{
                currentVelocity = Math3d.ProjectVectorOnPlane(warriorController.superCharacterController.up, currentVelocity);
                currentState = WarriorState.ExplodeLand;
                warriorController.SetAnimatorInt("Jumping", 0);
                warriorController.SetAnimatorTrigger(AnimatorTrigger.CriticalDamageTrigger);
                return;
			}

			// Normal gravity.
			currentVelocity -= warriorController.superCharacterController.up * gravity * warriorController.superCharacterController.deltaTime;
		}

		private void ExplodeFall_ExitState()
		{
			//currentVelocity = Vector3.zero;


			// Landed.
			//if (warriorController.AcquiringGround()) { warriorController.Land(); }
		}

		private void ExplodeLand_EnterState()
		{
            warriorController.superCharacterController.DisableClamping();
            warriorController.superCharacterController.DisableSlopeLimit();
			warriorController.LockAction(true);
        }

		private void ExplodeLand_SuperUpdate()
		{
            // Landing.
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction
                * warriorController.superCharacterController.deltaTime);

			_timer += Time.deltaTime;
            if (_timer >= 1f)
            {
                currentState = WarriorState.ExplodeWakeUp;
                return;
            }
        }

		private void ExplodeWakeUp_EnterState()
		{
			Debug.Log("途中までは処理したぞ");
			warriorController.superCharacterController.DisableClamping();
			warriorController.superCharacterController.DisableSlopeLimit();
			//warriorController.SetAnimatorInt("Jumping", 3);
			//warriorController.SetAnimatorTrigger(AnimatorTrigger.CriticalDamageTrigger);
		}

        private void ExplodeWakeUp_SuperUpdate()
        {
            currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction
                * warriorController.superCharacterController.deltaTime);
			if(!isKnockBack)
			{
                currentState = WarriorState.Idle;
                return;
            }
        }

        private void ExplodeWakeUp_ExitState()
		{
			Debug.Log("最後まで処理したやで");
			warriorController.SetAnimatorInt("Jumping", 0);
			isKnockBack = false;
            warriorController.LockJump(false);
            warriorController.takeExplodeDamage = false;
			warriorController.SetAnimatorInt("Action", 1);

            // Landed.
        }

		#endregion

		/// <summary>
		/// Rotate towards the direction the Warrior is moving.
		/// </summary>
		private void RotateTowardsMovementDir()
		{
			if (warriorController.moveInput != Vector3.zero && !warriorController.takeExplodeDamage) {
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(warriorController.moveInput), Time.deltaTime * rotationSpeed);
			}
		}

		public bool EndKnockBack()
		{
			return isKnockBack;
		}

		public void LandEnd()
		{
            warriorController.SetAnimatorInt("Jumping", 3);
            warriorController.SetAnimatorTrigger(AnimatorTrigger.CriticalDamageTrigger);
        }

        private void Update()
        {
            //Debug.Log(currentState + ", " + currentVelocity);
        }
    }
}