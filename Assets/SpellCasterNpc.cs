using UnityEngine;
using System.Collections;

public class SpellCasterNpc : MonoBehaviour {

    // This is the bullet prefab the will be instantiated when the player clicks
    // It must be set to an object in the editor 
    private FireBall Bullet;
    public Camera camera;
    public GameObject emitter;
    public float FireballCooldownInSeconds = 2f;
    private float _fireBallReadyTime;
    // Fire a bullet 
    public void Fire()
    {
        // Create a new bullet pointing in the same direction as the gun 
        //Bullet = Instantiate(Fire, transform.position, transform.rotation); 

        //mitter.Emit();
        if (_fireBallReadyTime <= Time.time)
        {
            Instantiate(emitter, transform.collider.bounds.center, transform.rotation); // transform.rotation); 
            _fireBallReadyTime = Time.time + FireballCooldownInSeconds;
            if (audio)
            {
                audio.Play();
            }
        }
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        
    }
}
