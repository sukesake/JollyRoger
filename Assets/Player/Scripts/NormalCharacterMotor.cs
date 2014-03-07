using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class NormalCharacterMotor : CharacterMotor {
	
	public float MaxRotationSpeed = 270;
	
	private bool _firstframe = true;
	
	private void UpdateFacingDirection() {
		// Calculate which way character should be facing
		float facingWeight = DesiredFacingDirection.magnitude;
		Vector3 combinedFacingDirection = (
			transform.rotation * DesiredMovementDirection * (1-facingWeight)
			+ DesiredFacingDirection * facingWeight
		);
		combinedFacingDirection = Util.ProjectOntoPlane(combinedFacingDirection, transform.up);
		combinedFacingDirection = AlignCorrection * combinedFacingDirection;
		
		if (combinedFacingDirection.sqrMagnitude > 0.01f) {
			Vector3 newForward = Util.ConstantSlerp(
				transform.forward,
				combinedFacingDirection,
				MaxRotationSpeed*Time.deltaTime
			);
			newForward = Util.ProjectOntoPlane(newForward, transform.up);
			//Debug.DrawLine(transform.position, transform.position+newForward, Color.yellow);
			Quaternion q = new Quaternion();
			q.SetLookRotation(newForward, transform.up);
			transform.rotation = q;
		}
	}
	
	private void UpdateVelocity() {
		CharacterController controller = GetComponent(typeof(CharacterController)) as CharacterController;
		Vector3 velocity = controller.velocity;
		if (_firstframe) {
			velocity = Vector3.zero;
			_firstframe = false;
		}
		if (grounded) velocity = Util.ProjectOntoPlane(velocity, transform.up);
		
		// Calculate how fast we should be moving
		Vector3 movement = velocity;
		//bool hasJumped = false;
		jumping = false;
		if (grounded) {
			// Apply a force that attempts to reach our target velocity
			Vector3 velocityChange = (DesiredVelocity - velocity);
			if (velocityChange.magnitude > MaxVelocityChange) {
				velocityChange = velocityChange.normalized * MaxVelocityChange;
			}
			movement += velocityChange;
			
			// Jump
			if (CanJump && Input.GetButton("Jump")) {
				movement += transform.up * Mathf.Sqrt(2 * JumpHeight * Gravity);
				//hasJumped = true;
				jumping = true;
			}
		}
		
		float maxVerticalVelocity = 1.0f;
		AlignmentTracker at = GetComponent<AlignmentTracker>();
		if (Mathf.Abs(at.velocitySmoothed.y) > maxVerticalVelocity) {
			movement *= Mathf.Max(0.0f, Mathf.Abs(maxVerticalVelocity / at.velocitySmoothed.y));
		}
		
		// Apply downwards gravity
        movement += transform.up * -Gravity * Time.deltaTime;
		
		if (jumping) {
            movement -= transform.up * -Gravity * Time.deltaTime / 2;
			
		}
		
		// Apply movement
		CollisionFlags flags = controller.Move(movement * Time.deltaTime);
		grounded = (flags & CollisionFlags.CollidedBelow) != 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.deltaTime == 0 || Time.timeScale == 0)
			return;
		
		UpdateFacingDirection();
		
		UpdateVelocity();
	}
}
