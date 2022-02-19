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

    // private enum RotationMode
    // {
    //     [ InspectorName( "Custom" ) ] [ Tooltip( "Rotation input is reserved for custom rotation controls." ) ]
    //     Explicit,
    //     [ InspectorName( "First Person" ) ] [ Tooltip( "Only rotation input X is used to rotate the player's ya. Ideal for first-person controls." ) ]
    //     ExplicitTransfer,
    //     [ InspectorName( "Third Person, Standard" ) ] [ Tooltip( "Rotation input is discarded. The character will try to rotate towards the last direction walked in, but will only be able to do so if we are inputting walk movement. Good for 3rd person." ) ]
    //     Implicit,
    //     [ InspectorName( "Third Person, Constant" ) ] [ Tooltip( "The character will always point to the last direction walked in, even if they are not walking." ) ]
    //     ImplicitConstant,
    //     [ InspectorName( "Third Person, Copy Camera" ) ] [ Tooltip( "The character will always point in the exact direction the camera is facing. Good for 1st person or tank controls." ) ]
    //     ImplicitCamera,
    // }

    private enum RotationMode
    {
        [ Tooltip( "Rotation yaw is derived from the CharacterCamera Component. Make sure to disable the X axis in the CharacterCamera!" ) ]
        FirstPerson,
        [ Tooltip( "Rotation input is transferred to the camera. Character rotation is determined by gravity and movement direction." ) ]
        ThirdPerson,
    }

    private static float GRAVITY_SCALE_MULTIPLIER = 0.01f;

    #endregion

    #region Exposed Properties

        [ Header( "Required Components" ) ]
        #region

            [ Tooltip( "The main camera that is attached to this character." ) ] [ SerializeField ]
            protected Camera _CharacterCamera;
            public Camera CharacterCamera {
                get {
                    return _CharacterCamera;
                }
            }
            public virtual Camera Camera {
                get {
                    if ( CharacterCamera )
                        return CharacterCamera;
                    else
                        return Camera.main;
                }
            }
            public Transform CameraTransform {
                get {
                    return Camera.transform;
                }
            }
        
        #endregion

        [ Header( "Collision" ) ]
        #region

            [ SerializeField ] [ Tooltip( "Determines which layers to check for ground. It is recommended that you set this gameObject's layer to something other than these layers, to prevent colliding with self." ) ]
            protected LayerMask GroundCheckLayers;
            [ Tooltip( "If enabled, the character's OnLandedGround function will trigger even when landing on steep slopes, allowing them to jump from such slopes." ) ]
            public bool EnableLandingOnSlope = false;

        #endregion

        [ Header( "Physics" ) ]

        [ Tooltip( "Scale of gravity forces applied." ) ]
        public float GravityScale = 1.0f;

        [ Min( 0 ) ] [ Tooltip( "This is the amount of gravity applied to us when we are on the ground." ) ] [ SerializeField ]
        private float _GroundedGravityStrength = 2.5f;
        public float GroundedGravityStrength {
            get {
                return _GroundedGravityStrength;
            }
        }



        [ Header( "Rotation" ) ]

        [ InspectorName( "Enable Rotation" ) ] [ Tooltip( "If enabled, the character will rotate using the HandleRotation() method. Otherwise, rotation cannot be controlled using input." ) ] [ SerializeField ]
        private bool EnableRotation = true;

        [ SerializeField ] [ Tooltip( "Determines how this character's yaw rotation is controlled. When using a custom HandleRotation() method, this field is ignored." ) ]
        private RotationMode RotationStyle = RotationMode.FirstPerson;
        
        
        [ Space( 10 ) ]
        
        [ Tooltip( "Amount of time it takes for us to reach our target rotation. Hint: the larger this value is, the more massive the character will feel." ) ]
        public float WalkRotationTime = 0.05f;
        [ Range( 0.0001f, 1f ) ] [ Tooltip( "Lower values quicken rotation when inputting walking slowly (not all the way)." ) ]
        public float WalkRotationExp = 0.5f;



        [ Header( "Walk Movement" ) ]


        [ Space( 10 ) ]

        [ Tooltip( "If enabled, the character will be allowed to attempt to move on steep slopes. Otherwise, they will continue to slide until they reach acceptable ground." ) ]
        public bool EnableSlopeWalk = true;
        [ Tooltip( "If enabled, the character will slide down slopes that are too steep to walk on." ) ]
        public bool EnableSlopeSlide = true;
        
        [ Space( 10 ) ]

        [ Tooltip( "Maximum speed we can walk, measured in m/s." ) ] [ SerializeField ]
        public float GroundWalkingSpeed = 10.0f;

        [ Tooltip( "How quickly we speed up to reach MaxWalkSpeed." ) ] [ SerializeField ]
        private float _GroundWalkingAcceleration = 10.0f;
        public float GroundWalkingAcceleration {
            get {
                return _GroundWalkingAcceleration * 0.01f;
            }
        }
        [ Tooltip( "How quickly we slow down to stop when not inputting movement." ) ] [ SerializeField ]
        private float _GroundBrakingDeceleration = 10.0f;
        public float GroundBrakingDeceleration {
            get {
                return _GroundBrakingDeceleration;
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

        protected CharacterController _Controller;

        private float _groundCheckOffset;
        public float GroundCheckOffset {
            get {
                return _groundCheckOffset;
            }
        }

        private Vector3 _velocity;
        private Vector3 _previousVelocity;

        private GroundMoveState _groundState;
        private Vector3 _groundPoint;
        private Vector3 _groundNormal;

        private Vector2 _rawWalkInputVector;
        private Vector3 _walkVector;
        private Vector3 _lastValidWalkVector;

        private Vector2 _rawRotateInputVector;
        private Vector3 _rotateVector;

        private float _targetYaw;
        private float _targetYawVelocity;

        private bool _isMovingUpInAir;

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
                                _OnLandedGround();
                                break;
                            case GroundMoveState.Sloped:
                                if ( EnableLandingOnSlope )
                                    _OnLandedGround();
                                else
                                    _OnDetachedGround();
                                break;
                            case GroundMoveState.Airborne:
                                _OnLeftGround();
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
            public Vector2 LateralVelocity
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
            public float LateralSpeed
            {
                get {
                    return Vector3.ProjectOnPlane( Velocity, GravityLateralPlane.normalized ).XZtoXY().magnitude;
                }
                set {
                    Velocity = Vector3.Scale( Velocity, GravityUp ) + Vector3.Scale( GravityLateralPlane.normalized, LateralVelocity.XYtoXZ().normalized * value );
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
            protected virtual float ContextualGroundWalkMultiplier
            {
                get
                {
                    return 1f;
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
            public Vector3 WalkVector {
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
                    float x = Vector3.Dot( transform.right, WalkVector );
                    float y = Vector3.Dot( transform.forward, WalkVector );

                    Vector2 result = new Vector2( x, y );
                    return result.normalized;
                }
            }
            private Vector2 WalkYawTarget
            {
                get
                {
                    if ( IsInputtingWalk )
                        return WalkVector.XZtoXY().normalized;
                    else
                        return LastValidWalkInputVector.XZtoXY().normalized;
                }
            }

        #endregion
        #region Rotate Input
 
            public bool IsInputtingRotate {
                get {
                    return _rawRotateInputVector.magnitude > 0f;
                }
            }
            public Vector3 RotateVector {
                get {
                    return _rotateVector;
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
                return false;
            }
        }

    #endregion


        /// <summary>
        /// This function is called when the character hit any object. If they are on the ground, this is constantly happening due to gravity.
        /// </summary>
        protected virtual void OnHit( ControllerColliderHit hit ) {}
        void OnControllerColliderHit( ControllerColliderHit hit )
        {
            float dot = Vector3.Dot( hit.moveDirection, -GravityUp );
            float angle = Mathf.Acos( dot ) * Mathf.Rad2Deg;

            bool isGroundAngle = angle < _Controller.slopeLimit;
            bool isOnSolidGround = IsGrounded && isGroundAngle;
            
            Velocity = Vector3.ProjectOnPlane( Velocity, isOnSolidGround ? GravityUp : hit.normal );
            
            if ( IsGrounded )
                Velocity += GravityUp * GravityScale * -GroundedGravityStrength * Time.deltaTime;

            OnHit( hit );
        }

    #region Events

        /// <summary>
        /// This function is called when the character touches the ground. This can be enabled or disabled for steep slopes.
        /// </summary>
        protected virtual void OnLandedGround() {}
        void _OnLandedGround()
        {
            _jumpsMade = 0;
            _Controller.stepOffset = GroundCheckOffset;

            OnLandedGround();
        }

        /// <summary>
        /// This function is called when the character leaves the ground significantly, such as jumping into the air, or walking off a ledge. Includes a call for OnDetachedGround.
        /// </summary>
        protected virtual void OnLeftGround() {}
        void _OnLeftGround()
        {
            _OnDetachedGround();
        }

        /// <summary>
        /// This function is called when the character leaves the ground for any reason, no matter how minute.
        /// </summary>
        protected virtual void OnDetachedGround() {}
        void _OnDetachedGround()
        {
            _Controller.stepOffset = 0f;
            _jumpsMade++;
        }

        protected virtual void OnReachedAirbornePeak() {}
        void _OnReachedAirbornePeak()
        {
            _isMovingUpInAir = false;

            OnReachedAirbornePeak();
        }
        
        protected virtual void OnReachedAirborneTrough() {}
        void _OnReachedAirborneTrough()
        {
            _isMovingUpInAir = true;

            OnReachedAirborneTrough();
        }
    
    #endregion
    
    protected virtual void OnValidate()
    {
        Awake();
    }

    protected virtual void Awake()
    {
        _Controller = GetComponent<CharacterController>();
        _groundCheckOffset = _Controller.stepOffset;

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

        #region Airborne Update

            if ( !IsGrounded )
            {
                if (  _isMovingUpInAir && VerticalSpeed <= 0f )
                    _OnReachedAirbornePeak();
                if ( !_isMovingUpInAir && VerticalSpeed >= 0f )
                    _OnReachedAirborneTrough();
            }
            
        #endregion

        #region Forces Update

            UpdateForces();

        #endregion

        #region Walk Input Update
             
            _walkVector = GetCameraAdjustedInputVector( _rawWalkInputVector );

            if ( IsInputtingWalk )
                _lastValidWalkVector = _walkVector;
        
        #endregion

        #region Walk Update

            float walkPercentAccel = ( IsGrounded ? 1f : AirWalkPercent );
            float walkPercentDecel = Mathf.Clamp01( ( IsGrounded ? 1f : 0f ) - WalkVector.magnitude );

            Vector3 walkVector;


            if ( GroundState == GroundMoveState.Sloped && !EnableSlopeWalk )
                walkVector = Vector3.zero;
            else
                walkVector = Vector3.ProjectOnPlane( WalkVector, GravityUp );

            // if ( IsTouchingAnyGround )
            //     walkVector = Vector3.ProjectOnPlane( walkVector, GroundNormal );

            AddForce( walkVector * walkPercentAccel * GroundWalkingAcceleration );
            AddForce( -Vector3.Scale( Velocity, GravityLateralPlane ) * walkPercentDecel * GroundBrakingDeceleration );

            // Limit the lateral speed to the Max Walk Speed.
            Velocity = Vector3.ClampMagnitude( Vector3.Scale( Velocity, GravityLateralPlane ), GroundWalkingSpeed * ContextualGroundWalkMultiplier * Time.deltaTime ) + GravityUp * VerticalSpeed;
            
        #endregion
        #region Rotation Update

            if ( EnableRotation )
            {
                HandleRotation( _rawRotateInputVector );
            }

        #endregion
        #region Jump Update
            
        #endregion

        _Controller.Move( Velocity );
        _previousVelocity = Velocity;
    }

    protected virtual void UpdateForces()
    {
        AddForce( GravityForce );
    }

    protected virtual void HandleRotation( Vector2 input )
    {
        _rotateVector = GetCameraAdjustedInputVector( _rawRotateInputVector );

        
        switch ( RotationStyle )
        {
            case RotationMode.FirstPerson:

                CharacterCamera camera = CharacterCamera.GetComponent<CharacterCamera>();

                Quaternion target = Quaternion.Euler( transform.eulerAngles.x, camera.Rotation.x, transform.eulerAngles.z );

                transform.rotation = Quaternion.LerpUnclamped( transform.rotation, target, camera.DampingSpeed * Time.deltaTime );

            break;
            case RotationMode.ThirdPerson:

                _targetYaw = Mathf.Atan2( WalkYawTarget.x, WalkYawTarget.y ) * Mathf.Rad2Deg;

                float yawSpeed = IsInputtingWalk ? Mathf.Pow( _rawWalkInputVector.magnitude, WalkRotationExp ) : 1f;
                float yaw = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y, _targetYaw, ref _targetYawVelocity,
                    ( 1f / yawSpeed ) * ( WalkRotationTime )
                );

                transform.rotation = Quaternion.Euler( transform.eulerAngles.x, yaw, transform.eulerAngles.z );
            
            break;
        }
    }
    
    protected virtual RaycastHit GroundCheck()
    {
        RaycastHit result;

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
            -GravityUp, out result, GroundCheckOffset,
            GroundCheckLayers, QueryTriggerInteraction.Ignore
        );

        return result;
    }

    #region Basic Physics
    
        public float SpeedInSpecifiedDirection( Vector3 normal )
        {
            return Vector3.Dot( Velocity.normalized, normal.normalized ) * Speed;
        }

        public void AddImpulse( Vector3 velocity )
        {
            Velocity += velocity;
        }
        public void AddForce( Vector3 velocity )
        {
            Velocity += velocity * Time.deltaTime;
        }

    #endregion

    #region Jump Input

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
        
    #endregion

    #region Input Methods

        public void AddWalkInput( InputAction.CallbackContext context )
        {
            _rawWalkInputVector = context.ReadValue<Vector2>();
        }

        public void AddRotateInput( InputAction.CallbackContext context )
        {
            _rawRotateInputVector = context.ReadValue<Vector2>();
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

        protected Vector3 GetCameraAdjustedInputVector( Vector2 input )
        {
            return ConvertInputWithTransform( CameraTransform, input );
        }
        protected static Vector3 ConvertInputWithTransform( Transform transform, Vector2 input )
        {
            Vector3 right = transform.right;
            Vector3 forward = Vector3.Scale( transform.forward, Vector3.up.Plane() ).normalized;

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
                    CapsuleBottom + ( transform.up * GroundCheckOffset ) + transform.forward * ( _Controller.radius + 0.8f ),
                    Quaternion.LookRotation( -transform.forward, transform.up ),
                    Vector3.one
                );
                Gizmos.DrawFrustum( Vector3.zero, 22.0f, 0.6f, 0.0f, 1.0f );
                Gizmos.matrix = Matrix4x4.identity;
            }

            if ( DrawVelocity )
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay( transform.position, Velocity * Time.deltaTime * 50000.0f );
            }
            
            if ( DrawInput )
            {
                Gizmos.color = Color.white;
                Gizmos.DrawRay( transform.position, WalkVector );
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

                Vector3 groundPoint1 = CapsuleBottomNoHemisphere - GravityUp * GroundCheckOffset;
                Vector3 groundPoint2 = CapsuleTopNoHemisphere    - GravityUp * GroundCheckOffset;

                if ( !completelyUpsidedown )
                    Gizmos.DrawSphere( groundPoint1, _Controller.radius );

                if ( !completelyUpright )
                    Gizmos.DrawSphere( groundPoint2, _Controller.radius );
            }
        }

    #endregion
}
