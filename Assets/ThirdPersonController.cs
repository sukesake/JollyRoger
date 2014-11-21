using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void JumpDelegate();

public class ThirdPersonController : MonoBehaviour
{
    // Assign to this delegate to respond to the controller jumping
    public int currentJumpCount = 0;
    public int maxJumpCount = 2;


    private const float inputThreshold = 0.01f,
        groundDrag = 5.0f,
        directionalJumpFactor = 0.7f;

    // Tweak these to adjust behaviour relative to speed
    private const float groundedDistance = 0.5f;
    private readonly Inventory _inventory = new Inventory();

    public bool
        // Turn this off if the camera should be controllable even without cursor lock
        controlLock = false;

    public LayerMask groundLayers = -1;

    // Tweak if character lands too soon or gets stuck "in air" often


    private bool grounded;
    public float groundedCheckOffset = 0.7f;
    public float jumpSpeed = 1.0f;
    public float mouseTurnSpeed = 4f;
    public JumpDelegate onJump = null;

    public bool
        requireLock = true;

    public bool
        showGizmos = true;

    public float speed = 1.0f;
    public Rigidbody target;
    public float turnSpeed = 2.0f;
    public float walkSpeedDownscale = 2.0f;

    private bool walking;
	private bool releasedJump = true;


    public bool Grounded
        // Make our grounded status available for other components
    {
        get { return grounded; }
    }

    private float SidestepAxisInput
        // If the right mouse button is held, the horizontal axis also turns into sidestep handling
    {
        get
        {
            if (true)
                //if (Input.GetMouseButton (1))
            {
                float sidestep = Input.GetAxis("Sidestep"), horizontal = Input.GetAxis("Horizontal");

                return Mathf.Abs(sidestep) > Mathf.Abs(horizontal) ? sidestep : horizontal;
            }
            return Input.GetAxis("Sidestep");
        }
    }


    private void Reset()
        // Run setup on component attach, so it is visually more clear which references are used
    {
        Setup();
    }


    private void Setup()
        // If target is not set, try using fallbacks
    {
        if (target == null)
        {
            target = GetComponent<Rigidbody>();
        }
    }


    private void Start()
        // Verify setup, configure rigidbody
    {
        Setup();
        // Retry setup if references were cleared post-add

        if (target == null)
        {
            Debug.LogError("No target assigned. Please correct and restart.");
            enabled = false;
            return;
        }

        target.freezeRotation = true;
        // We will be controlling the rotation of the target, so we tell the physics system to leave it be
        walking = false;
    }


    private void Update()
        // Handle rotation here to ensure smooth application.
    {
        float rotationAmount;

        if (true)
            //if (Input.GetMouseButton (1) && (!requireLock || controlLock || Screen.lockCursor))
            // If the right mouse button is held, rotation is locked to the mouse
        {
            if (controlLock)
            {
                Screen.lockCursor = true;
            }

            rotationAmount = Input.GetAxis("Mouse X")*mouseTurnSpeed*Time.deltaTime;
        }
        else
        {
            if (controlLock)
            {
                Screen.lockCursor = false;
            }

            rotationAmount = Input.GetAxis("Horizontal")*turnSpeed*Time.deltaTime;
        }

        target.transform.RotateAround(target.transform.up, rotationAmount);

        if (Input.GetButtonDown("ToggleWalk"))
        {
            walking = !walking;
        }
        if (Input.GetButtonDown("Loot"))
        {
            Loot();
        }
    }

    private void Loot()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, 3);

        Debug.Log(string.Format("loot-splosion found {0} things", hitColliders.Length));

        for (var i = 0; i < hitColliders.Length; i++)
        {
            var lootCube = hitColliders[i].GetComponent<LootCube>();
            if (lootCube != null)
            {
                Debug.Log(string.Format("found a cube #{0}x{1}", lootCube.ItemNumber, lootCube.Quantity));
                _inventory.Loot(lootCube.ItemNumber, lootCube.Quantity);

                lootCube.DoLootAnimation(transform);

               // iTween.MoveTo(lootCube.transform.gameObject, transform.position, 3f);
                //iTween.MoveTo(lootCube.transform.gameObject, new Hashtable());
             //   Destroy(lootCube.transform.gameObject);
            }
        }
    }


    private void FixedUpdate()
        // Handle movement here since physics will only be calculated in fixed frames anyway
    {
        grounded = Physics.Raycast(
            target.transform.position + target.transform.up*-groundedCheckOffset,
            target.transform.up*-1,
            groundedDistance,
            groundLayers
            );
        // Shoot a ray downward to see if we're touching the ground
		
        if (grounded)
        {
            currentJumpCount = 0;
        }

        if (Input.GetButtonUp("Jump"))
        {
            releasedJump = true;
        }

		if (currentJumpCount < maxJumpCount && releasedJump)
        {
            target.drag = groundDrag;
            // Apply drag when we're grounded

            if (Input.GetButton("Jump"))
                // Handle jumping
            {
                if (currentJumpCount > 0)
                {
                    target.rigidbody.velocity = new Vector3(target.rigidbody.velocity.x, 0, target.rigidbody.velocity.z);
                }
                target.AddForce(
                    jumpSpeed*target.transform.up +
                    target.velocity.normalized*directionalJumpFactor,
                    ForceMode.VelocityChange
                    );
                // When jumping, we set the velocity upward with our jump speed
                // plus some application of directional movement

                if (onJump != null)
                {
                    onJump();
                }
                currentJumpCount++;
                releasedJump = false;
            }
            else
                // Only allow movement controls if we did not just jump
            {
                var movement = Input.GetAxis("Vertical")*target.transform.forward +
                               SidestepAxisInput*target.transform.right;

                var appliedSpeed = walking ? speed/walkSpeedDownscale : speed;
                // Scale down applied speed if in walk mode

                if (Input.GetAxis("Vertical") < 0.0f)
                    // Scale down applied speed if walking backwards
                {
                    appliedSpeed /= walkSpeedDownscale;
                }

                if (movement.magnitude > inputThreshold)
                    // Only apply movement if we have sufficient input
                {
                    target.AddForce(movement.normalized*appliedSpeed, ForceMode.VelocityChange);
                }
                else
                    // If we are grounded and don't have significant input, just stop horizontal movement
                {
                    target.velocity = new Vector3(0.0f, target.velocity.y, 0.0f);
                }
            }
        }
        else
        {
            target.drag = 0.0f;
            // If we're airborne, we should have no drag
        }
    }


    private void OnDrawGizmos()
        // Use gizmos to gain information about the state of your setup
    {
        if (!showGizmos || target == null)
        {
            return;
        }

        Gizmos.color = grounded ? Color.blue : Color.red;
        Gizmos.DrawLine(target.transform.position + target.transform.up*-groundedCheckOffset,
            target.transform.position + target.transform.up*-(groundedCheckOffset + groundedDistance));
    }
}

internal class Inventory
{
    private readonly Dictionary<long, long> playerInventory = new Dictionary<long, long>();

    /// <summary>
    ///     Here is where inventory validation will occur such as, is there room, are more than one of this item allowed to be
    ///     possed.
    /// </summary>
    /// <param name="itemNumber"></param>
    /// <param name="quantity"></param>
    public void Loot(long itemNumber, long quantity)
    {
        if (playerInventory.ContainsKey(itemNumber))
        {
            playerInventory[itemNumber] += quantity;
        }
        else
        {
            playerInventory.Add(itemNumber, quantity);
        }
    }
}