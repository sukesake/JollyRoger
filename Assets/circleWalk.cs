using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class circleWalk : MonoBehaviour
{

    private Vector3 _spawnLocation;
    private Vector3 _desiredLocation;
    private DateTimeOffset _lastLocationChosenTime;
	// Use this for initialization
	void Start ()
	{
	    _spawnLocation = transform.position;
	    _lastLocationChosenTime = DateTimeOffset.Now;
	}
	
	/* speed of orbit (in degrees/second) */
	public float Speed = 40f;
    public int  Health = 100;
    private bool _grounded = false;

    // Update is called once per frame
	void Update () 
	{
	    if (Health <= 0)
	    {
            Debug.Log("DEATH");
	        Destroy(transform.gameObject);
	    }
	    if (TimeToPickNewLocation())
	    {
	        PickNewDesiredLocation();
	    }
	  
        if (_grounded)
	        {
	            MoveTowardsDesiredLocation();
	        }
	        //   transform.RotateAround(_spawnLocation, Vector3.up, -Speed * Time.deltaTime);
        
	    _grounded = false;
	}

    public void TakeDamage(int damage)
    {
        Debug.Log("OUCH");
        Health -= damage;
    }

    

    private void MoveTowardsDesiredLocation()
    {

        var relativePos = _desiredLocation - transform.position;

        var lookAt = Quaternion.LookRotation(relativePos);

        transform.RotateAround(_desiredLocation, Vector3.up, -Speed * Time.deltaTime);

      //  transform.rotation = Quaternion.Lerp(transform.rotation, lookAt, Time.deltaTime * 10);

       // rigidbody.AddForce(transform.forward * Speed, ForceMode.VelocityChange);
     //   rigidbody.velocity = transform.forward * Speed;

    }

    private void PickNewDesiredLocation()
    {
        var desiredLocation = Random.insideUnitCircle;
        _desiredLocation = new Vector3
        {
            x = transform.position.x + (desiredLocation.x*100),
            y = transform.position.y + (desiredLocation.y * 100),
            z = transform.position.z + (desiredLocation.y*100)
        };
        _desiredLocation.z = transform.position.z;
  
        _lastLocationChosenTime = DateTimeOffset.Now;
    }

    private bool TimeToPickNewLocation()
    {
        var timeSinceLastPick = DateTimeOffset.Now - _lastLocationChosenTime;
      
        return timeSinceLastPick > TimeSpan.FromSeconds(5);
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        foreach (ContactPoint c in collisionInfo.contacts)
        {
            if (Mathf.Abs(c.normal.y) > Mathf.Abs(c.normal.x))
            {
                _grounded = true;
            }
        }
    }
}
