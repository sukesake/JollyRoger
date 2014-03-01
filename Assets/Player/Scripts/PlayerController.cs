using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
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

    private Vector3 PlayerSpeed
    {
        get { return new Vector3(ForwardSpeed, 0, SidewaysSpeed); }
    }


    private void Awake()
    {
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
    }

    private void FixedUpdate()
    {
        // Calculate how fast we should be moving
        Vector3 currentVelocity = rigidbody.velocity;
        Vector3 targetVelocity =
            GetDirectionalInput().ScaleIt(PlayerSpeed).WorldSpaceIt(transform.rotation).ClampIt(MaxSpeed);

        if (Input.GetMouseButtonDown(0))
        {
            PunchShit();
        }

        if (_grounded)
        {
            Vector3 velocityChange = (targetVelocity - currentVelocity).ClampIt(MaxVelocityChange, MaxVelocityChange, 0);
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            if (CanJump && Input.GetButton("Jump"))
            {
                rigidbody.velocity = new Vector3(currentVelocity.x, CalculateJumpVerticalSpeed(), currentVelocity.z);
            }
        }
        else
        {
            // Apply a force that attempts to reach our target velocity
            Vector3 velocityChange = (targetVelocity - currentVelocity).ClampIt(MaxAirVelocityChange,
                MaxAirVelocityChange, 0);
            rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
        }

        // We apply gravity manually for more tuning control
        rigidbody.AddForce(new Vector3(0, -Gravity*rigidbody.mass, 0));

        _grounded = false;

        transform.Rotate(0, Input.GetAxis("Mouse X")*SensitivityX, 0);
    }


    private void PunchShit()
    {
        RaycastHit objectHit;
        Debug.Log("localEulerAngles: " + transform.localEulerAngles);

        // Shoot raycast
        if (Physics.Raycast(transform.position, transform.forward, out objectHit, 50))
        {
            //Debug.Log("Raycast hitted to: " + objectHit.collider);
            GameObject targetEnemy = objectHit.collider.gameObject;

            Debug.DrawLine(transform.position, objectHit.point, Color.green, 0.5f, false);

            if (targetEnemy.rigidbody != null)
            {
                targetEnemy.rigidbody.AddForce(7*(2*transform.forward + Vector3.up), ForceMode.VelocityChange);
                Debug.Log("TargetEnemy: " + targetEnemy.name);
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