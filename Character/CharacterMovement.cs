using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    #region Static Definitions

    protected enum GroundMoveState
    {
        [ Tooltip( "In the air, not touching the ground at all." ) ]
        Airborne,
        [ Tooltip( "We're touching the ground, but it's too steep to walk on." ) ]
        Sloped,
        [ Tooltip( "We're touching the ground and can move around on it." ) ]
        Grounded,
    }

    private enum RotationMode
    {
        [ InspectorName( "Manual" ) ] [ Tooltip( "The character will not automatically rotate AND will strafe. Good for 1st person." ) ]
        Explicit,
        [ InspectorName( "Automatic, Only When Walking" ) ] [ Tooltip( "The character will try to rotate towards the last direction walked in, but will only be able to do so if we are inputting walk movement. Good for 3rd person." ) ]
        Implicit,
        [ InspectorName( "Automatic, Constant" ) ] [ Tooltip( "The character will always point to the last direction walked in, even if they are not walking." ) ]
        ImplicitConstant,
    }

    private static float GRAVITY_SCALE_MULTIPLIER = 0.01f;

    #endregion

    #region Exposed Properties

        [ Header( "Collision" ) ]

        [ SerializeField ] [ Tooltip( "Determines which layers to check for ground. It is recommended that you set this gameObject's layer to something other than these layers, to prevent colliding with self." ) ]
        private LayerMask GroundCheckLayers;
        [ Tooltip( "If enabled, the character's OnLandedGround function will trigger even when landing on steep slopes, allowing them to jump from such slopes." ) ]
        public bool EnableLandingOnSlope = false;


        [ Header( "Physics" ) ]

        [ Tooltip( "Scale of gravity forces applied." ) ]
        public float GravityScale = 1.0f;

        // [ Tooltip( "This is the amount of gravity applied to us just once when we hit the ground. A low value will make falling off ledges and slopes easier, and a high value will cause us to fall down." ) ]
        // public float GroundedGravityStrength = 0.04f;



        [ Header( "Rotation" ) ]

        [ SerializeField ] [ Tooltip( "Determines how this character's yaw rotation is controlled." ) ]
        private RotationMode RotationStyle = RotationMode.Implicit;
        
        [ Space( 10 ) ]
        
        [ Tooltip( "Amount of time it takes for us to reach our target rotation. Hint: the larger this value is, the more massive the character will feel." ) ]
        public float WalkRotationTime = 0.05f;
        [ Range( 0.0001f, 1f ) ] [ Tooltip( "Lower values quicken rotation when inputting walking slowly (not all the way)." ) ]
        public float WalkRotationExp = 0.5f;



        [ Header( "Walk Movement" ) ]

        public Transform CameraTransform;

        [ Space( 10 ) ]

        [ Tooltip( "If enabled, the character will be allowed to attempt to move on steep slopes. Otherwise, they will continue to slide until they reach acceptable ground." ) ]
        public bool EnableSlopeWalk = true;
        [ Tooltip( "If enabled, the character will slide down slopes that are too steep to walk on." ) ]
        public bool EnableSlopeSlide = true;
        
        [ Space( 10 ) ]

        [ Tooltip( "Maximum speed we can walk, measured in m/s." ) ] [ SerializeField ]
        private float _MaxWalkSpeed = 10.0f;
        public float MaxWalkSpeed {
            get {
                return _MaxWalkSpeed;
            }
        }
        [ Tooltip( "How quickly we speed up to reach MaxWalkSpeed." ) ] [ SerializeField ]
        private float _WalkGroundAccel = 10.0f;
        public float WalkGroundAccel {
            get {
                return _WalkGroundAccel * 0.01f;
            }
        }
        [ Tooltip( "How quickly we slow down to stop when not inputting movement." ) ] [ SerializeField ]
        private float _WalkGroundDecel = 0.1f;
        public float WalkGroundDecel {
            get {
                return _WalkGroundDecel;
            }
        }
        [ Range( 0.0f, 1.0f ) ] [ Tooltip( "Determines how strongly the character slide down slopes." ) ]
        public float SlopeSlideStrength = 1.0f;
        [ Range( 0.0f, 1.0f ) ] [ Tooltip( "Percentage of walk acceleration and MaxWalkSpeed we can use while airborne." ) ]
        public float AirWalkPercent = 1.0f;



        [ Header( "Jump Movement" ) ]

        [ Tooltip( "If enabled, this character will use the built-in method of jumping." ) ]
        public bool EnableJump = true;
        
        [ Space( 10 ) ]

        [ Min( 0 ) ] [ Tooltip( "Number of jumps allowed before needing to touch the ground again." ) ]
        public int JumpCount = 1;
        [ Min( 0f ) ] [ Tooltip( "How strong the jump is." ) ]
        public float JumpStrength = 10.0f;
        [ Range( 0.0f, 1.0f ) ] [ Tooltip( "When Jumping from Ground, this is the percentage of GroundNormal to follow." ) ]
        public float GroundedSlopeJumpBias = 0.0f;
        [ Range( 0.0f, 1.0f ) ] [ Tooltip( "When Jumping from a Slope, this is the percentage of GroundNormal to follow." ) ]
        public float SteepSlopeJumpBias = 1.0f;

        [ Space( 10 ) ]

        [ SerializeField ] [ Min( 0f ) ] [ Tooltip( "For this long after jumping, we will be guaranteed to not be in the Grounded state." ) ]
        private float GroundCheckDelayAfterJump = 0.1f;



        [ Header( "Debug" ) ]
        
        [ SerializeField ] [ Tooltip( "Draws an arrow pointing in the direction the character is currently moving." ) ]
        private bool DrawMovementArrow = true;
        [ SerializeField ] [ Tooltip( "Draws a red line from the point of contact (if it exists) in the direction of the ground normal." ) ]
        private bool DrawGroundState = false;
        [ SerializeField ] [ Tooltip( "Draws a blue line from the center of the character in the direction and magnitude of their velocity." ) ]
        private bool DrawVelocity = false;
        [ SerializeField ] [ Tooltip( "Draws a white line from the center of the character in the direction and magnitude of their input velocity." ) ]
        private bool DrawInput = false;

    #endregion
    
    #region Private Variables

        private CharacterController _Controller;

        private Vector3 _velocity;
        private Vector3 _previousVelocity;

        private GroundMoveState _groundState;
        private Vector3 _groundPoint;
        private Vector3 _groundNormal;

        private Vector2 _rawWalkInputVector;
        private Vector3 _walkVector;
        private Vector3 _lastValidWalkVector;

        private float _targetYaw;
        private float _targetYawVelocity;

        private bool _isHoldingJump;
        private float _whenJumped;
        private int _jumpsMade;

    #endregion

    #region Public Property Methods

        #region Capsule Geometry

            /// <summary>
            /// Half the height of the Controller capsule, from the center to the end of the capsule.
            /// </summary>
            public float CapsuleHalfHeight
            {
                get
                {
                    return _Controller.height / 2f;
                }
            }
            /// <summary>
            /// Half the height of the Controller capsule, not including the hemispherical portion(s).
            /// </summary>
            public float CapsuleHalfHeightNoHemisphere
            {
                get
                {
                    return CapsuleHalfHeight - _Controller.radius;
                }
            }
            /// <summary>
            /// The position of the very top of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleTop
            {
                get
                {
                    return transform.position + _Controller.center + transform.up * CapsuleHalfHeight; 
                }
            }
            /// <summary>
            /// The position of the center of the top hemisphere of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleTopNoHemisphere
            {
                get
                {
                    return transform.position + _Controller.center + transform.up * CapsuleHalfHeightNoHemisphere;
                }
            }
            /// <summary>
            /// The position of the very bottom of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleBottom
            {
                get
                {
                    return transform.position + _Controller.center - transform.up * CapsuleHalfHeight; 
                }
            }
            /// <summary>
            /// The position of the center of the bottom hemisphere of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleBottomNoHemisphere
            {
                get
                {
                    return transform.position + _Controller.center - transform.up * CapsuleHalfHeightNoHemisphere;
                }
            }
            /// <summary>
            /// The position of the center of the ground check sphere.
            /// </summary>
            protected Vector3 GroundCheckSphereCenter
            {
                get
                {
                    return CapsuleBottomNoHemisphere - transform.up * _Controller.stepOffset;
                }
            }

        #endregion    
        #region Ground Movement

            /// <summary>
            /// The state of being on the ground which this character is currently experiencing.
            /// </summary>
            protected GroundMoveState GroundState
            {
                get
                {
                    return _groundState;
                }
                private set
                {
                    GroundMoveState previous = _groundState;
                    _groundState = value;
                
                    if ( _groundState != previous )
                    {
                        switch ( _groundState )
                        {
                            case GroundMoveState.Grounded:
                                OnLandedGround();
                                break;
                            case GroundMoveState.Sloped:
                                if ( EnableLandingOnSlope )
                                    OnLandedGround();
                                else
                                    OnDetachedGround();
                                break;
                            case GroundMoveState.Airborne:
                                OnLeftGround();
                                break;
                        }
                    }
                }
            }
            /// <summary>
            /// Whether or not we are firmly planted on the ground.
            /// </summary>
            public bool IsGrounded
            {
                get
                {
                    return GroundState == GroundMoveState.Grounded;
                }
            }
            /// <summary>
            /// Whether or not we are touching any ground at all, even a steep slope.
            /// </summary>
            public bool IsTouchingAnyGround
            {
                get
                {
                    return GroundState != GroundMoveState.Airborne;
                }
            }
            /// <summary>
            /// The normal vector of the ground on which we are walking. Will be ( 0, 0, 0 ) if airborne.
            /// </summary>
            public Vector3 GroundNormal
            {
                get
                {
                    return _groundNormal;
                }
            }
            /// <summary>
            /// The position at which we are touching the ground. Will be ( 0, 0, 0 ) if airborne.
            /// </summary>
            public Vector3 GroundPoint
            {
                get
                {
                    return _groundPoint;
                }
            }
            /// <summary>
            /// The float angle (degrees) of the ground on which we are walking. Will be 0.0f if airborne.
            /// </summary>
            public float GroundAngle
            {
                get
                {
                    if ( GroundState == GroundMoveState.Airborne )
                        return 0.0f;
                    return Mathf.Acos( Vector3.Dot( GroundNormal, GravityUp ) ) * Mathf.Rad2Deg;
                }
            }
            /// <summary>
            /// The direction that this player should slide down a steep slope.
            /// </summary>
            private Vector3 SlopeNormal
            {
                get
                {
                    return Vector3.Cross( Vector3.Cross( GravityUp, GroundNormal ), GroundNormal );
                }
            }


        #endregion
        #region Gravity

            /// <summary>
            /// The gravitational force exerted on this character.
            /// </summary>
            protected Vector3 GravityForce
            {
                get
                {
                    return Physics.gravity * GravityScale * GRAVITY_SCALE_MULTIPLIER;
                }
            }
            /// <summary>
            /// The normalized direction opposite the gravity on this character.
            /// </summary>
            protected Vector3 GravityUp
            {
                get
                {
                    return -GravityForce.normalized;
                }
            }
            /// <summary>
            /// The planar vector opposing our gravity.
            /// </summary>
            public Vector3 GravityLateralPlane
            {
                get
                {
                    return GravityUp.Plane();
                }
            }
        
        #endregion
        #region Velocity
        
            /// <summary>
            /// The velocity as read from the CharacterController.
            /// </summary>
            public Vector3 ControllerVelocity
            {
                get
                {
                    return _Controller.velocity;
                }
            }
            /// <summary>
            /// The velocity/momentum of the character. Be careful when setting this directly!
            /// </summary>
            public Vector3 Velocity
            {
                get
                {
                    return _velocity;
                }
                set
                {
                    _velocity = value;
                }
            }
            /// <summary>
            /// The XZ speed of the character along their current gravity plane. Set value has NOT yet been tested.
            /// </summary>
            public Vector2 LateralSpeed
            {
                get
                {
                    return Vector3.ProjectOnPlane( Velocity, GravityLateralPlane ).XZtoXY();
                }
                set
                {
                    Velocity = Vector3.Scale( Velocity, GravityUp ) + Vector3.Scale( GravityLateralPlane, value.XYtoXZ() );
                }
            }
            /// <summary>
            /// The Y speed of the character along their current gravity axis.
            /// </summary>
            public float VerticalSpeed
            {
                get
                {
                    return Vector3.Dot( Velocity, GravityUp );
                }
                set
                {
                    Velocity = Vector3.Scale( Velocity, GravityLateralPlane ) + GravityUp * value;
                }
            }
            private float VerticalSpeedPrevious
            {
                get
                {
                    return Vector3.Dot( _previousVelocity, GravityUp );
                }
            }
            /// <summary>
            /// The speed of the character as a single float value, i.e. speedometer reading.
            /// </summary>
            public float Speed
            {
                get
                {
                    return Velocity.magnitude;
                }
                set
                {
                    Velocity = Velocity.normalized * value;
                }
            }

        #endregion
        
        #region Walk Input

            /// <summary>
            /// Describes whether or not the character is inputting any walk movement or not.
            /// </summary>
            public bool IsInputtingWalk
            {
                get
                {
                    return _rawWalkInputVector.magnitude > 0f;
                }
            }
            /// <summary>
            /// The Vector3 describing the direction in which we are inputting walking on the ground. This gets complicated when considering camera movement and gravity changing.
            /// </summary>
            public Vector3 WalkInputVector {
                get {
                    return _walkVector;
                }
            }
            public Vector3 LastValidWalkInputVector {
                get {
                    return _lastValidWalkVector;
                }
            }
            /// <summary>
            /// The Vector2 describing how our walk input differs from our forward vector; suitable for animation blendspaces.
            /// </summary>
            public Vector2 StrafeVector
            {
                get
                {
                    float x = Vector3.Dot( transform.right, WalkInputVector );
                    float y = Vector3.Dot( transform.forward, WalkInputVector );

                    Vector2 result = new Vector2( x, y );
                    return result.normalized;
                }
            }
            private Vector2 WalkYawTarget
            {
                get
                {
                    if ( IsInputtingWalk )
                        return WalkInputVector.XZtoXY().normalized;
                    else
                        return LastValidWalkInputVector.XZtoXY().normalized;
                }
            }

        #endregion
        #region Jump Input

            public virtual bool CanJump
            {
                get
                {
                    return _jumpsMade < JumpCount;
                }
            }

        #endregion

        public bool IsStrafing {
            get {
                return RotationStyle == RotationMode.Explicit;
            }
        }

    #endregion

    #region Events

        /// <summary>
        /// This function is called when the character touches the ground. This can be enabled or disabled for steep slopes.
        /// </summary>
        protected virtual void OnLandedGround()
        {
            // Set our vertical velocity to a constant rate.
            // VerticalSpeed = -GroundedGravityStrength;

            _jumpsMade = 0;
        }

        /// <summary>
        /// This function is called when the character leaves the ground significantly, such as jumping into the air, or walking off a ledge. Includes a call for OnDetachedGround.
        /// </summary>
        protected virtual void OnLeftGround()
        {
            // Set the vertical velocity to zero.
            VerticalSpeed = Mathf.Max( VerticalSpeed, 0f );

            OnDetachedGround();
        }

        /// <summary>
        /// This function is called when the character leaves the ground for any reason, no matter how minute.
        /// </summary>
        protected virtual void OnDetachedGround()
        {
            _jumpsMade++;
        }

        protected virtual void OnReachedAirborneApex() {}
    
    #endregion
    
    protected virtual void OnValidate()
    {
        Awake();
    }

    protected virtual void Awake()
    {
        _Controller = GetComponent<CharacterController>();
        _targetYaw = transform.rotation.y;
    }

    protected virtual void Start() {}

    protected virtual void Update()
    {
        #region Ground Update

            RaycastHit hit = GroundCheck();
            bool touchingGround = hit.collider != null;

            _groundNormal = hit.normal;
            _groundPoint = hit.point;

            if ( touchingGround && Time.time > _whenJumped + GroundCheckDelayAfterJump )
            {
                if ( GroundAngle <= _Controller.slopeLimit )
                    GroundState = GroundMoveState.Grounded;
                else
                    GroundState = GroundMoveState.Sloped;
            }
            else
            {
                GroundState = GroundMoveState.Airborne;
            }

        #endregion
        #region Forces Update

            UpdateForces();

        #endregion
        #region Walk Update

            float walkPercentAccel = ( IsGrounded ? 1f : AirWalkPercent );
            float walkPercentDecel = Mathf.Clamp01( ( IsGrounded ? 1f : 0f ) - WalkInputVector.magnitude );

            Vector3 walkVector;

            if ( GroundState == GroundMoveState.Sloped && !EnableSlopeWalk )
                walkVector = Vector3.zero;
            else
                walkVector = Vector3.ProjectOnPlane( WalkInputVector, GravityUp );

            AddForce( walkVector * walkPercentAccel * WalkGroundAccel );
            AddForce( -Vector3.Scale( Velocity, GravityLateralPlane ) * walkPercentDecel * WalkGroundDecel );

            // Limit the lateral speed to the Max Walk Speed.
            Velocity = Vector3.ClampMagnitude( Vector3.Scale( Velocity, GravityLateralPlane ), MaxWalkSpeed * Time.deltaTime ) + GravityUp * VerticalSpeed;
            
        #endregion
        #region Jump Update
            
        #endregion
        #region Rotation Update

            if ( RotationStyle != RotationMode.Explicit )
            {
                if ( IsInputtingWalk || RotationStyle == RotationMode.ImplicitConstant )
                {
                    _targetYaw = Mathf.Atan2( WalkYawTarget.x, WalkYawTarget.y ) * Mathf.Rad2Deg;
                    
                    float rotationDivisor = IsInputtingWalk ? Mathf.Pow( _rawWalkInputVector.magnitude, WalkRotationExp ) : 1f;

                    float rotation = Mathf.SmoothDampAngle(
                        transform.eulerAngles.y, _targetYaw, ref _targetYawVelocity,
                        WalkRotationTime / rotationDivisor
                    );

                    transform.rotation = Quaternion.Euler( transform.eulerAngles.x, rotation, transform.eulerAngles.z );
                }
            }

        #endregion

        _Controller.Move( Velocity );
        _previousVelocity = Velocity;

        print( _walkVector );
    }

    protected virtual RaycastHit GroundCheck()
    {
        RaycastHit result;

        // Physics.SphereCast(
        //     CapsuleBottomNoHemisphere, _Controller.radius,
        //     -transform.up, out result, _Controller.stepOffset, GroundCheckLayers,
        //     QueryTriggerInteraction.Ignore
        // );

        float upFactor = Vector3.Dot( GravityUp, transform.up );
        bool completelyUpright = upFactor >= 0.99f;
        bool completelyUpsidedown = upFactor <= -0.99f;

        Vector3 groundPoint1 = CapsuleBottomNoHemisphere;
        Vector3 groundPoint2 = CapsuleTopNoHemisphere;

        if ( completelyUpsidedown )
            groundPoint1 = groundPoint2;
        else if ( completelyUpright )
            groundPoint2 = groundPoint1;

        Physics.CapsuleCast(
            groundPoint1, groundPoint2, _Controller.radius,
            -GravityUp, out result, _Controller.stepOffset,
            GroundCheckLayers, QueryTriggerInteraction.Ignore
        );

        return result;
    }
    protected virtual void UpdateForces()
    {
        if ( GroundState == GroundMoveState.Airborne )
        {
            AddForce( GravityForce );
            
            if ( VerticalSpeedPrevious > 0f && VerticalSpeed <= 0f )
                OnReachedAirborneApex();
        }
        else if ( GroundState == GroundMoveState.Sloped )
        {
            AddForce( SlopeNormal * GravityForce.magnitude * SlopeSlideStrength );
        }
    }

    protected bool TryStartJump()
    {
        if ( CanJump )
        {
            Jump();
            return true;
        }

        return false;
    }

    protected bool TryJumpEnd()
    {
        if ( _isHoldingJump )
        {
            JumpEnd();
            return true;
        }

        return false;
    }

    public virtual void Jump()
    {
        VerticalSpeed = 0f;

        Vector3 jumpNormal;
        
        switch ( GroundState )
        {
            case GroundMoveState.Grounded:
                jumpNormal = Vector3.LerpUnclamped( GravityUp, GroundNormal, GroundedSlopeJumpBias ).normalized;
                break;
            case GroundMoveState.Sloped:
                jumpNormal = Vector3.LerpUnclamped( GravityUp, GroundNormal, SteepSlopeJumpBias ).normalized;
                break;
            default:
                jumpNormal = GravityUp;
                break;
        }

        AddImpulse( jumpNormal * JumpStrength );

        _isHoldingJump = true;
        
        _whenJumped = Time.time;
        _jumpsMade++;
    }
    
    public virtual void JumpEnd()
    {
        _isHoldingJump = false;
    }

    public void AddImpulse( Vector3 velocity )
    {
        Velocity += velocity;
    }
    public void AddForce( Vector3 velocity )
    {
        Velocity += velocity * Time.deltaTime;
    }

    #region Input Methods

        public void AddWalkInput( InputAction.CallbackContext context )
        {
            _rawWalkInputVector = context.ReadValue<Vector2>();

            _walkVector = GetCameraAdjustedWalkVector( _rawWalkInputVector );

            if ( IsInputtingWalk )
                _lastValidWalkVector = _walkVector;
        }

        public void JumpInput( InputAction.CallbackContext context )
        {
            if ( EnableJump )
            {
                if ( context.started )
                {
                    TryStartJump();
                }
                else if ( context.canceled )
                {
                    TryJumpEnd();
                }
            }
        }

        protected Vector3 GetCameraAdjustedWalkVector( Vector2 input )
        {
            Vector3 right = CameraTransform.right;
            Vector3 forward = Vector3.Scale( CameraTransform.forward, Vector3.up.Plane() ).normalized;

            Vector3 composite = right * input.x + forward * input.y;

            return Vector3.ClampMagnitude( composite, 1f );
        }

    #endregion

    #region Debug Methods

        public void OnDrawGizmosSelected()
        {
            if ( DrawMovementArrow )
            {
                Gizmos.color = Color.white;
                Gizmos.matrix = Matrix4x4.TRS(
                    transform.position + transform.forward * ( _Controller.radius + 0.8f ),
                    Quaternion.LookRotation( -transform.forward, transform.up ),
                    Vector3.one
                );
                Gizmos.DrawFrustum( Vector3.zero, 22.0f, 0.6f, 0.0f, 1.0f );
                Gizmos.matrix = Matrix4x4.identity;
            }

            if ( DrawVelocity )
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay( transform.position, _Controller.velocity * Time.deltaTime * 40.0f );
            }
            
            if ( DrawInput )
            {
                Gizmos.color = Color.white;
                Gizmos.DrawRay( transform.position, WalkInputVector );
            }

            if ( DrawGroundState )
            {
                switch ( GroundState )
                {
                    case GroundMoveState.Grounded:
                        Gizmos.color = Color.red;
                        Gizmos.DrawRay( GroundPoint, GroundNormal );
                        
                        Gizmos.color = new Color( 1.0f, 0.0f, 0.0f, 0.35f );
                        break;
                    case GroundMoveState.Sloped:
                        Gizmos.color = Color.red;
                        Gizmos.DrawRay( GroundPoint, GroundNormal );

                        Gizmos.color = new Color( 1.0f, 0.0f, 1.0f, 0.35f );
                        break;
                    default:
                        Gizmos.color = new Color( 0.0f, 0.0f, 1.0f, 0.35f );
                        break;
                }

                float upFactor = Vector3.Dot( GravityUp, transform.up );
                bool completelyUpright = upFactor >= 0.99f;
                bool completelyUpsidedown = upFactor <= -0.99f;

                Vector3 groundPoint1 = CapsuleBottomNoHemisphere - GravityUp * _Controller.stepOffset;
                Vector3 groundPoint2 = CapsuleTopNoHemisphere    - GravityUp * _Controller.stepOffset;

                if ( !completelyUpsidedown )
                    Gizmos.DrawSphere( groundPoint1, _Controller.radius );

                if ( !completelyUpright )
                    Gizmos.DrawSphere( groundPoint2, _Controller.radius );
            }
        }

    #endregion
}
