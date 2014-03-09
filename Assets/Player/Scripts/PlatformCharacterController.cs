using System;
using UnityEngine;

public class PlatformCharacterController : MonoBehaviour
{
    public Transform AimTarget;
    public Transform CameraTransform;

    public bool DefaultIsWalk = false;
    public float WalkMultiplier = 0.5f;
    private Vector3 _airshipLastKnownPosition;
    private CharacterMotor _motor;
    private Quaternion _airshipLastKnownRotation;

    private void Start()
    {
        _motor = GetComponent(typeof (CharacterMotor)) as CharacterMotor;

        if (_motor == null)
        {
            throw new NullReferenceException("_motor is Null");
        }
        if (AimTarget == null)
        {
            throw new NullReferenceException("AimTarget is Null");
        }
        if (CameraTransform == null)
        {
            throw new NullReferenceException("CameraTransform is Null");
        }

        Screen.showCursor = false;
    }

    private void Update()
    {
        // Get input vector from keyboard or analog stick and make it length 1 at most
        var directionVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if (directionVector.magnitude > 1) directionVector = directionVector.normalized;
        directionVector = directionVector.normalized*Mathf.Pow(directionVector.magnitude, 2);

        // Rotate input vector into camera space so up is camera's up and right is camera's right
        directionVector = Camera.main.transform.rotation*directionVector;

        // Rotate input vector to be perpendicular to character's up vector
        Quaternion camToCharacterSpace = Quaternion.FromToRotation(Camera.main.transform.forward*-1, transform.up);
        directionVector = (camToCharacterSpace*directionVector);

        // Make input vector relative to Character's own orientation
        directionVector = Quaternion.Inverse(transform.rotation)*directionVector;

        if (WalkMultiplier != 1)
        {
            if ((Input.GetKey("left shift") || Input.GetKey("right shift")) != DefaultIsWalk)
            {
                directionVector *= WalkMultiplier;
            }
        }

        // Apply direction
        _motor.DesiredMovementDirection = directionVector;

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    /// <summary>
    ///     since we are in 3rd person view and want out aim to be accurate, we need to cast a ray from the
    ///     cameras location to the aimTarget. we use this ray for all collision detection. then using the
    ///     players location and the location of a collision we can do the necesarry animations
    /// </summary>
    private void Shoot()
    {
        RaycastHit objectHit;
        Vector3 aimDirection = (AimTarget.position - CameraTransform.position).normalized;

        //  Debug.DrawLine(raySourceLocation, targetLocation, Color.grey, 0.5f, false);
        Debug.DrawLine(transform.position, AimTarget.position, Color.green, 0.5f, false);
        // Shoot raycast
        if (Physics.Raycast(CameraTransform.position, aimDirection, out objectHit, 50))
        {
            GameObject targetEnemy = objectHit.collider.gameObject;

            //TODO(pruett): implement a maybe monad to make these assertions a lot more fluent
            // targetEnemy.rigidbody.IfNotNull().Then().AddForce(aimDirection.ScaleIt(20) + Vector3.up.ScaleIt(7));

            if (targetEnemy.rigidbody != null)
            {
                targetEnemy.rigidbody.AddForce(aimDirection.ScaleIt(20) + Vector3.up.ScaleIt(7),
                    ForceMode.VelocityChange);
                //normal take damage
            }
            objectHit.collider.SendMessage("TakeDamage", 52, SendMessageOptions.DontRequireReceiver);
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "platform")
        {
            _airshipLastKnownPosition = other.transform.position;
            _airshipLastKnownRotation = other.transform.rotation;
            Debug.Log("OMG I GOT ON A BOAT");
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "platform")
        {
            Vector3 airshipPositionChange = other.transform.position - _airshipLastKnownPosition;
            var airshipRotationChange = other.transform.rotation* Quaternion.Inverse(_airshipLastKnownRotation);
            Debug.Log("airshipPositionChange: " + airshipPositionChange);
            _airshipLastKnownPosition = other.transform.position;
            _airshipLastKnownRotation = other.transform.rotation;
            transform.position += airshipPositionChange;
            transform.rotation *= airshipRotationChange;

            //   Debug.Log("OMG IM ON A BOAT");
            //  transform.parent = other.transform;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "platform")
        {
            Debug.Log("im off the boat");
            // transform.parent = null;
        }
    }
}