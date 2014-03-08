using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    protected Quaternion AlignCorrection;
    public bool CanJump = true;
    public Vector3 ForwardVector = Vector3.forward;
    public float Gravity = 10.0f;
    public float JumpHeight = 1.0f;
    private Vector3 _desiredFacingDirection;
    private Vector3 _desiredMovementDirection;
    public float MaxBackwardsSpeed = 1.5f;
    public float MaxForwardSpeed = 1.5f;
    public float MaxSidewaysSpeed = 1.5f;
    public float MaxVelocityChange = 0.2f;

    public CharacterMotor()
    {
        jumping = false;
        grounded = false;
    }

    public bool grounded { get; protected set; }

    public bool jumping { get; protected set; }

    public Vector3 DesiredMovementDirection
    {
        get { return _desiredMovementDirection; }
        set
        {
            _desiredMovementDirection = value;
            if (_desiredMovementDirection.magnitude > 1)
                _desiredMovementDirection = _desiredMovementDirection.normalized;
         
        }
    }

    public Vector3 DesiredFacingDirection
    {
        get { return _desiredFacingDirection; }
        set
        {
            _desiredFacingDirection = value;
            if (_desiredFacingDirection.magnitude > 1) _desiredFacingDirection = _desiredFacingDirection.normalized;
        }
    }

    public Vector3 DesiredVelocity
    {
        get
        {
            //return m_desiredVelocity;
            if (_desiredMovementDirection == Vector3.zero) return Vector3.zero;
            float zAxisEllipseMultiplier = (_desiredMovementDirection.z > 0 ? MaxForwardSpeed : MaxBackwardsSpeed)/
                                           MaxSidewaysSpeed;
            Vector3 temp =
                new Vector3(_desiredMovementDirection.x, 0, _desiredMovementDirection.z/zAxisEllipseMultiplier)
                    .normalized;
            float length = new Vector3(temp.x, 0, temp.z*zAxisEllipseMultiplier).magnitude*MaxSidewaysSpeed;
            Vector3 velocity = _desiredMovementDirection*length;
   
            return transform.rotation*velocity;
        }
    }

    private void Start()
    {
        AlignCorrection = new Quaternion();
        AlignCorrection.SetLookRotation(ForwardVector, Vector3.up);
        AlignCorrection = Quaternion.Inverse(AlignCorrection);
    }
}