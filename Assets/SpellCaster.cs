using UnityEngine;
using System.Collections;

public class SpellCaster : MonoBehaviour {

	// This is the bullet prefab the will be instantiated when the player clicks
	// It must be set to an object in the editor 
	FireBall Bullet; 
	public GameObject emitter;
	public Camera camera;
	// Fire a bullet 
	public void Fire () {
		// Create a new bullet pointing in the same direction as the gun 
		//Bullet = Instantiate(Fire, transform.position, transform.rotation); 

		//mitter.Emit();
		Instantiate(emitter, transform.position, camera.transform.rotation);// transform.rotation); 
		if (audio) {
			audio.Play();
		}
	} 
	
	public void Update () {
		// Fire if the left mouse button is clicked 
		if (Input.GetButtonDown("Fire1")) 
		{
			Fire();
		} 
	}
}
