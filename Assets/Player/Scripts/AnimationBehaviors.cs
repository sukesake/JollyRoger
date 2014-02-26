using UnityEngine;
using System.Collections;

public class AnimationBehaviors : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(CharacterIsJumping()){
			animation.Play("jump");
		}
		else if(CharacterIsMoving()){
			animation.Play("walk");
		}
	}

	private bool CharacterIsMoving()
	{
		return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
	}

	private bool CharacterIsJumping()
	{
		return  Input.GetButton("Jump");
	}
}
