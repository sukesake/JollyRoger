using UnityEngine;
using System.Collections;

public class circleWalk : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	/* the object to orbit */
	public Transform target;
	
	/* speed of orbit (in degrees/second) */
	public float speed;
	
	// Update is called once per frame
	void Update () 
	{

		if (target != null) {
			transform.RotateAround(target.position, Vector3.up, -speed * Time.deltaTime);
		}
	}
}
