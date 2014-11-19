using System.Globalization;
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class LootDropper : MonoBehaviour
{

    public GameObject LootCube = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void DropLoot()
    {
        var rnd = new Random();

        var lootCount = rnd.Next(1, 5);

        for (int i = 0; i < lootCount; i++)
        {
            Instantiate(LootCube, transform.position, Quaternion.identity);
        }
    }
}
