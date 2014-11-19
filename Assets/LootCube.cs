using System;
using UnityEngine;
using System.Collections;
using Random = System.Random;

public class LootCube : MonoBehaviour
{

    public int MaxHorizontalSpread = 1;
    public int MaxVerticalHeight = 10;
    public long ItemNumber { get; set; }
    public long Quantity { get; set; }

    // Use this for initialization
	void Start ()
	{
	    var rnd = new Random();
        transform.rigidbody.AddExplosionForce(3, transform.position, 1);
	    ItemNumber = rnd.Next(1, 10);
	    Quantity = rnd.Next(1, 3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Destroy()
    {
        Debug.Log("DEEEESTORY");
    }
}
