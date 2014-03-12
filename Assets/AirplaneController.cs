using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class AirplaneController : MonoBehaviour
{

    private float _rollSpeed = 200;
    private float _pitchSpeed = 200;
    private float _yawSpeed = 200;
    private float _acceleration = 2000;
    private float _oldVelocity;

    private float _thrust = 0;
    private float _maxThrust = 600000;

    private float _liftFactor = 400;

    private float _speed = 0;
    private float _gravity = 100;


	// Use this for initialization
	void Start () 
    {
        _oldVelocity = rigidbody.velocity.sqrMagnitude;
        rigidbody.inertiaTensor = new Vector3(16, 16, 16);
        
	}
	
	// Update is called once per frame
	void Update () 
    {
	    //var addRot = Quaternion.identity;
       

       var roll = Input.GetAxis("Roll") * (Time.deltaTime * _rollSpeed);

       var pitch = Input.GetAxis("Pitch") * (Time.deltaTime * _pitchSpeed);

       var yaw = Input.GetAxis("Yaw") * (Time.deltaTime * _yawSpeed);

        _thrust = Input.GetAxis("Throttle") * (Time.deltaTime * _acceleration);

        var lift = Input.GetAxis("Throttle") * (Time.deltaTime *  _liftFactor); //TODO(pruett): there should be some math here to make the lift factor contigent upon pitch

        var torqueVector = new Vector3(-roll, yaw, -pitch).WorldSpaceIt(transform.rotation);

       // transform.Rotate(torqueVector);
        
        rigidbody.AddTorque(torqueVector);

	 //   if (_thrust >= 0)
	  //  {
	        var maxVelocity = 1000;

            var thrustVector = ((new Vector3(_thrust*5, _gravity, 0).WorldSpaceIt(transform.rotation))- rigidbody.velocity);
	        Debug.Log("thrust: " + thrustVector);
            //rigidbody.AddRelativeForce(thrustVector);
            rigidbody.AddForce(thrustVector, ForceMode.Acceleration);
	 //   }

        rigidbody.AddForce(new Vector3(0,-_gravity, 0), ForceMode.Acceleration);
    }
}
