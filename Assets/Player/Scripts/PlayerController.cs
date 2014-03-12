using UnityEngine;

[RequireComponent(typeof (CapsuleCollider))]
public partial class PlayerController : MonoBehaviour
{
    public bool CanJump = true;
    public float ForwardSpeed = 12.0f;
    public float Gravity = 10.0f;
    public float JumpHeight = 4.0f;
    public float MaxAirVelocityChange = 0.5f;
    public float MaxSpeed = 12.0f;
    public float MaxVelocityChange = 10.0f;
    public float SensitivityX = 15F;
    public float SidewaysSpeed = 8.0f;

    private bool _grounded;
    private CharacterMotor _motor;

    private Vector3 PlayerSpeed
    {
        get { return new Vector3(ForwardSpeed, 0, SidewaysSpeed); }
    }

    // Use this for initialization
    void Start()
    {
        _motor = GetComponent(typeof(CharacterMotor)) as CharacterMotor;
        if (_motor == null) Debug.Log("Motor is null!!");

        //originalRotation = transform.localRotation;
    }

    //private void Awake()
    //{
    //    rigidbody.freezeRotation = true;
    //    rigidbody.useGravity = false;
    //}

    void Update()
    {
        // Get input vector from kayboard or analog stick and make it length 1 at most
        var directionVector = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0);
        if (directionVector.magnitude > 1) directionVector = directionVector.normalized;

        // Rotate input vector into camera space so up is camera's up and right is camera's right
        directionVector = Camera.main.transform.rotation * directionVector;

        // Rotate input vector to be perpendicular to character's up vector
        var camToCharacterSpace = Quaternion.FromToRotation(Camera.main.transform.forward * -1, transform.up);
        directionVector = (camToCharacterSpace * directionVector);

        // Apply direction
        _motor.DesiredFacingDirection = directionVector;
    }

    private void FixedUpdateDepricated()
    {

       //  Calculate how fast we should be moving
        Vector3 currentVelocity = rigidbody.velocity;
      //  Vector3 targetVelocity = GetDirectionalInput().ScaleIt(PlayerSpeed).WorldSpaceIt(transform.rotation).ClampIt(MaxSpeed);

        if (Input.GetMouseButtonDown(0))
        {
            PunchShit();
        }

        if (_grounded)
        {
        //    Vector3 velocityChange = (targetVelocity - currentVelocity).ClampIt(MaxVelocityChange, MaxVelocityChange, 0);
       //     rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            if (CanJump && Input.GetButton("Jump"))
            {
                rigidbody.velocity = new Vector3(currentVelocity.x, CalculateJumpVerticalSpeed(), currentVelocity.z);
            }
        }
        else
        {
            // Apply a force that attempts to reach our target velocity
        //    Vector3 velocityChange = (targetVelocity - currentVelocity).ClampIt(MaxAirVelocityChange,
      //          MaxAirVelocityChange, 0);
      //      rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        // We apply gravity manually for more tuning control
        rigidbody.AddForce(new Vector3(0, -Gravity*rigidbody.mass, 0));

        _grounded = false;

        transform.Rotate(0, Input.GetAxis("Mouse X")*SensitivityX, 0);
    }


    private void PunchShit()
    {
        RaycastHit objectHit;


        // Shoot raycast
        if (Physics.Raycast(transform.position, transform.forward, out objectHit, 50))
        {
            //Debug.Log("Raycast hitted to: " + objectHit.collider);
            GameObject targetEnemy = objectHit.collider.gameObject;

            Debug.DrawLine(transform.position, objectHit.point, Color.green, 0.5f, false);

            if (targetEnemy.rigidbody != null)
            {
                targetEnemy.rigidbody.AddForce(7*(2*transform.forward + Vector3.up), ForceMode.VelocityChange);
 
            }
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * 50, Color.red, 0.5f, false);
        }
    }


    private void OnCollisionStay(Collision collisionInfo)
    {
        foreach (ContactPoint c in collisionInfo.contacts)
        {
            if (Mathf.Abs(c.normal.y) > Mathf.Abs(c.normal.x))
            {
                _grounded = true;
            }
        }
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(3*JumpHeight*Gravity);
    }
}