using System;
using UnityEngine;
using System.Collections;
using Random = System.Random;

public class DamageDealer : MonoBehaviour {

	public int Damage = 15;
    private Random _rng;

    // Use this for initialization
	void Start ()
	{
	    _rng = new Random();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnParticleCollision(GameObject c)
	{
		var component = c.GetComponent<DamageTaker>();

      
		if(component != null)
		{
			component.TakeDamage(_rng.Next(9,200));
		}
	}
}
