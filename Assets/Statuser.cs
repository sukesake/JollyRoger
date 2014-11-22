using UnityEngine;
using System.Collections;

public class Statuser : MonoBehaviour
{
    public string Status = "Friendly";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetStatus()
    {
        return Status;
    }
}
