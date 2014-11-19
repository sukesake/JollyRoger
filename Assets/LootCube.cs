using System;
using UnityEngine;
using System.Collections;
using Random = System.Random;

public class LootCube : MonoBehaviour
{

    public int MaxHorizontalSpread = 1;
    public int MaxVerticalHeight = 10;
    private bool _looted;
    private Transform _looter;
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

    void FixedUpdate()
    {
        if (_looted)
        {
            iTween.MoveUpdate(transform.gameObject, _looter.position, 1.5f);

            if (Vector3.Distance (transform.position, _looter.position) < 1)
            {
                Destroy(transform.gameObject);
            }
        }
    }

    void Destroy()
    {
        Debug.Log("DEEEESTORY");
    }

    public void DoLootAnimation(Transform looter)
    {
        _looted = true;
        _looter = looter;
    }
}
