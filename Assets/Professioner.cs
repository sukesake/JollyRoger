using UnityEngine;
using System.Collections;

public class Professioner : MonoBehaviour
{

    public string Profession = "Unemployed";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public string GetProfession()
    {
        return Profession;
    }
}
