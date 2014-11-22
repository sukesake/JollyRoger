using UnityEngine;
using System.Collections;

public class Mooder : MonoBehaviour
{
    public string Mood = "Neutral";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetMood()
    {
        return Mood;
    }
}
