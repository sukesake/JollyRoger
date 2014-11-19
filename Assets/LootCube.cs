using System;
using UnityEngine;
using System.Collections;
using Random = System.Random;

public class LootCube : MonoBehaviour
{

    public int MaxHorizontalSpread = 1;
    public int MaxVerticalHeight = 10;
	// Use this for initialization
	void Start ()
	{
	    var rnd = new Random();
        transform.rigidbody.AddExplosionForce(3, transform.position, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
