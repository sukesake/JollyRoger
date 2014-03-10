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

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var thrustVector = new Vector3(-Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), -Input.GetAxis("Vertical"));
	    Debug.Log("thrustVactor: " + thrustVector);
        transform.rigidbody.AddForce(thrustVector.WorldSpaceIt(transform.rotation), ForceMode.VelocityChange);

        _angleH += Mathf.Clamp(Input.GetAxis("Mouse X") + Input.GetAxis("Horizontal2"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;

        _angleV += Mathf.Clamp(-Input.GetAxis("Mouse Y") + -Input.GetAxis("Vertical2"), -1, 1) * verticalAimingSpeed * Time.deltaTime;
        // limit vertical angle
        _angleV = Mathf.Clamp(_angleV, minVerticalAngle, maxVerticalAngle);

        // Set aim rotation
        Quaternion aimRotation = Quaternion.Euler(-_angleV, _angleH, 0);
        Quaternion camYRotation = Quaternion.Euler(0, _angleH, 0);
        transform.localRotation = aimRotation;
	}
}
