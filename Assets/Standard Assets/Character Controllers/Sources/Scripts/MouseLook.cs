using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour 
{
	public GameObject target = null;
	public GameObject targetReticle = null;

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float distance = 10.0f;
	public float offset = 2.0f;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -30F;
	public float maximumY = 30F;

	float rotationY = 0F;

	void Update ()
	{
		if(target == null || targetReticle == null)
		{
			return;
		}

		transform.position = target.transform.position + Vector3.up * offset + target.transform.forward * -distance;

		if (axes == RotationAxes.MouseXAndY)
		{
			float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else if (axes == RotationAxes.MouseX)
		{
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
		}
		else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);

			if(rotationY > 0)
			{
				distance = Mathf.Lerp(10, 5, rotationY / maximumY);
				offset = Mathf.Lerp(2.0f, -4.0f, rotationY / maximumY);
			}
			else
			{
				distance = Mathf.Lerp(10, 5, -rotationY / maximumY);
				offset = Mathf.Lerp(2.0f, 6.0f, -rotationY / maximumY);
			}
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);

			targetReticle.transform.position = transform.position + transform.forward * 30;
		}
	}
	
	void Start ()
	{
		Screen.showCursor = false;
		// Make the rigid body not change rotation
		//if (rigidbody)
			//rigidbody.freezeRotation = true;
	}
}