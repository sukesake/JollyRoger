using UnityEngine;
using System.Collections;

public class animator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		animation.Play("Idle01");
	}
	
	// Update is called once per frame
	void Update () {
		var horizontal = Input.GetAxis("Horizontal"); 
		var vertical = Input.GetAxis("Vertical");
		if(horizontal > 0)
		{
			animation.Play("Move01_R");
		}
		else if(horizontal < 0)
		{
			animation.Play("Move01_L");
		}
		else if(vertical > 0)
		{
			animation.Play("Move01_F");
		}
		else if(vertical < 0)
		{
			animation.Play("Move01_B");
		}
		else
		{
			animation.Play("Idle01");
		}


	}
}
