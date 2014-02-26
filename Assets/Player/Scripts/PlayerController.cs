using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class PlayerController : MonoBehaviour {
	
	public float ForwardSpeed = 12.0f;
	public float SidewaysSpeed = 8.0f;
	public float MaxSpeed = 12.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public float maxAirVelocityChange = 0.5f;
	public bool canJump = true;
	public float jumpHeight = 4.0f;
	private bool grounded = false;
	public float sensitivityX = 15F;
	
	
	
	void Awake () 
	{
		rigidbody.freezeRotation = true;
		rigidbody.useGravity = false;
	}
	
	void FixedUpdate () 
	{
		// Calculate how fast we should be moving
		Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		targetVelocity.x *= SidewaysSpeed;
		targetVelocity.z *= ForwardSpeed;
		targetVelocity = transform.TransformDirection(targetVelocity);
		if(targetVelocity.magnitude > MaxSpeed)
		{
			targetVelocity.Normalize();
			targetVelocity *= MaxSpeed;
		}
		
		if (grounded) 
		{
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rigidbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
			velocityChange.y = 0;
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
			
			// Jump
			if (canJump && Input.GetButton("Jump")) 
			{
				rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
			}
		}
		else
		{
			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rigidbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxAirVelocityChange, maxAirVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxAirVelocityChange, maxAirVelocityChange);
			velocityChange.y = 0;
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
		}
		
		// We apply gravity manually for more tuning control
		rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));
		
		grounded = false;

		transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
	}
	
	void OnCollisionStay (Collision collisionInfo) 
	{
		foreach(ContactPoint c in collisionInfo.contacts)
		{
			if(Mathf.Abs(c.normal.y) > Mathf.Abs(c.normal.x))
			{
				grounded = true;
			}
		}		    
	}
	
	float CalculateJumpVerticalSpeed () 
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
}