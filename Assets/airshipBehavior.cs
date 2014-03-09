using UnityEngine;
using System.Collections;

public class airshipBehavior : MonoBehaviour
{

    public Transform RotationTarget;
    public float Speed = 1f;
    private bool _timeToGo = false;

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var startPosition = transform.position;
	    if (_timeToGo)
	    {
            
	        transform.RotateAround(RotationTarget.position, Vector3.up, Speed*Time.deltaTime);
	    }
	    var endPosition = transform.position;


	}

    public void TakeDamage()
    {
        _timeToGo = !_timeToGo;
    }
}
