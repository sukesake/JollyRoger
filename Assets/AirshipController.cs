using System;
using UnityEngine;
using System.Collections;

public class AirshipController : MonoBehaviour {
    private float _angleH;
    private float _angleV;

    public float horizontalAimingSpeed = 270f;
    public float verticalAimingSpeed = 270f;
    public float maxVerticalAngle = 80f;
    public float minVerticalAngle = -80f;
    private const float _maxVelocity = 100;
    private const float _maxVelocityChange = 3;

    

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currentVelocity = rigidbody.velocity;
        var thrustVector = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("VerticalThrust"), -Input.GetAxis("Horizontal")).WorldSpaceIt(transform.rotation).ScaleIt(_maxVelocity);
	   // Debug.Log("thrustVactor: " + thrustVector);

        Vector3 velocityChange = (thrustVector - currentVelocity);
       // Debug.Log("input: " + KeyCode.LeftControl.ToString());
       //  Debug.Log("velocityChange: " + velocityChange);
         velocityChange = velocityChange.ClampIt(_maxVelocityChange, _maxVelocityChange, _maxVelocityChange);
       
        //    rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        transform.rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        _angleH += Mathf.Clamp(Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal2"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;

        _angleV += Mathf.Clamp(-Input.GetAxis("Mouse Y") + -Input.GetAxis("Vertical2"), -1, 1) * verticalAimingSpeed * Time.deltaTime;
        // limit vertical angle
        _angleV = Mathf.Clamp(_angleV, minVerticalAngle, maxVerticalAngle);

        // Set aim rotation
        var sway = (Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal2")) * -200 * Time.deltaTime;

        Quaternion aimRotation = Quaternion.Euler(0, _angleH, -_angleV);
        Quaternion camYRotation = Quaternion.Euler(0, _angleH, 0);
        transform.localRotation = aimRotation;
	}
}
