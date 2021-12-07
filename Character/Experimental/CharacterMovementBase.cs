using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterMovementBase<MoveSpace, RotateSpace, InputSpace> : MonoBehaviour
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

        protected static float GRAVITY_SCALE_MULTIPLIER = 0.01f;

    #endregion

    #region Exposed Properties
        
        [ SerializeField ] [ Tooltip( "Draws an arrow pointing in the direction the character is currently moving." ) ]
        private bool GizmoMovementArrow = true;
        [ SerializeField ] [ Tooltip( "Draws a red line from the point of contact (if it exists) in the direction of the ground normal." ) ]
        private bool GizmoGroundState = false;
        [ SerializeField ] [ Tooltip( "Draws a blue line from the center of the character in the direction and magnitude of their velocity." ) ]
        private bool GizmoVelocity = false;
        [ SerializeField ] [ Tooltip( "Draws a white line from the center of the character in the direction and magnitude of their input velocity." ) ]
        private bool GizmoInput = false;

        [ Header( "Collision" ) ]

        [ SerializeField ] [ Tooltip( "Determines which layers to block when moving. It is recommended that you set this gameObject's layer to something other than these layers, to prevent colliding with self." ) ]
        protected LayerMask CollisionLayers;
        [ Tooltip( "If enabled, the character's OnLandedGround function will trigger even when landing on steep slopes, allowing them to jump from such slopes." ) ]
        public bool EnableLandingOnSlope = false;

        public float SkinWidth = 0.08f;

        public float SlopeAngleLimit = 50.0f;

        public float StepHeight = 0.3f;



        [ Header( "Physics" ) ]

        [ Tooltip( "Scale of gravity forces applied." ) ]
        public float GravityScale = 1.0f;



        [ Header( "Walk Movement" ) ]

        [ Tooltip( "If enabled, the character will be allowed to attempt to move on steep slopes. Otherwise, they will continue to slide until they reach acceptable ground." ) ]
        public bool EnableSlopeWalk = true;
        [ Tooltip( "If enabled, the character will slide down slopes that are too steep to walk on." ) ]
        public bool EnableSlopeSlide = true;

        [ Space( 10 ) ]

        [ Tooltip( "Maximum speed we can walk, measured in m/s." ) ]
        public float MaxWalkSpeed = 10.0f;
        [ Tooltip( "How quickly we speed up to reach MaxWalkSpeed." ) ]
        public float WalkGroundAccel = 10.0f;
        [ Tooltip( "How quickly we slow down to stop when not inputting movement." ) ]
        public float WalkGroundDecel = 0.1f;
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

    #endregion


    #region Private Variables

        private MoveSpace _velocity;
        private MoveSpace _velocityLastUpdate;

        private GroundMoveState _groundState;

        private float _targetWalkVelocity;

        private MoveSpace _groundPoint;
        private MoveSpace _groundNormal;

        private InputSpace _walkInputAxis;
        private InputSpace _walkInputAxisLastValid;

        private bool _isHoldingJump;
        private float _whenJumped;
        private int _jumpsMade;

    #endregion

    #region Property Methods

        #region Velocity

            /// <summary>
            /// Current 3-dimensional velocity/momentum of the character. Be careful when setting this directly!
            /// </summary>
            public MoveSpace Velocity {
                get {
                    return _velocity;
                }
                set {
                    _velocity = value;
                }
            }
            /// <summary>
            /// Returns the velocity from the previous update.
            /// </summary>
            public MoveSpace VelocityLastUpdate {
                get {
                    return _velocityLastUpdate;
                }
            }
            /// <summary>
            /// The speed of the character as a single float value, i.e. speedometer reading.
            /// </summary>
            public abstract float Speed {
                get; set;
            }            
            /// <summary>
            /// The Y speed of the character along their current gravity axis.
            /// </summary>
            public abstract float VerticalSpeed {
                get; set;
            }
            public abstract float VerticalSpeedPrevious {
                get;
            }


        #endregion
        

        /// <summary>
        /// The state of being on the ground which this character is currently experiencing.
        /// </summary>
        protected GroundMoveState GroundState {
            get {
                return _groundState;
            }
            set {
                GroundMoveState previous = _groundState;
                _groundState = value;
            
                if ( _groundState != previous ) {
                    switch ( _groundState ) {
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
        public bool IsGrounded {
            get {
                return GroundState == GroundMoveState.Grounded;
            }
        }
        /// <summary>
        /// Whether or not we are touching any ground at all, even a steep slope.
        /// </summary>
        public bool IsTouchingAnyGround {
            get {
                return GroundState != GroundMoveState.Airborne;
            }
        }
        /// <summary>
        /// Returns true if we can start checking for ground under us after jumping.
        /// </summary>
        public bool IsGroundImmunityExpired {
            get {
                return Time.time > _whenJumped + GroundCheckDelayAfterJump;
            }
        }

        #region Physics

            /// <summary>
            /// The gravitational force exerted on this character.
            /// </summary>
            protected abstract MoveSpace GravityForce {
                get;
            }
            /// <summary>
            /// The normalized direction opposite the gravity on this character.
            /// </summary>
            protected abstract MoveSpace GravityUp {
                get;
            }
        
        #endregion
        
        #region Ground Geometry

            /// <summary>
            /// The position at which we are touching the ground. Will be ( 0, 0, 0 ) if airborne.
            /// </summary>
            public MoveSpace GroundPoint {
                get {
                    return _groundPoint;
                }
                protected set {
                    _groundPoint = value;
                }
            }
            /// <summary>
            /// The normal vector of the ground on which we are walking. Will be ( 0, 0, 0 ) if airborne.
            /// </summary>
            public MoveSpace GroundNormal {
                get {
                    return _groundNormal;
                }
                protected set {
                    _groundNormal = value;
                }
            }
            /// <summary>
            /// The direction that this player should slide down a steep slope.
            /// </summary>
            protected abstract MoveSpace SlopeNormal {
                get;
            }
            /// <summary>
            /// Returns the angle of the ground beneath us in degrees. The result is absolute value'd. I.E. Steepness.
            /// </summary>
            public abstract float GroundAngle {
                get;
            }

        #endregion

        #region Walk Input

            /// <summary>
            /// Returns our current WalkInputAxis.
            /// </summary>
            public InputSpace WalkInputAxis {
                get {
                    return _walkInputAxis;
                }
                protected set {
                    _walkInputAxis = value;
                }
            }
            /// <summary>
            /// Returns the last WalkInputAxis that was not zero.
            /// </summary>
            public InputSpace WalkInputAxisLastValid {
                get {
                    return _walkInputAxisLastValid;
                }
                protected set {
                    _walkInputAxisLastValid = value;
                }
            }
            /// <summary>
            /// Describes whether or not the character is inputting any walk movement or not.
            /// </summary>
            public abstract bool IsInputtingWalk {
                get;
            }
            /// <summary>
            /// The Vector3 describing the direction in which we are inputting walking on the ground. This gets complicated when considering camera movement and gravity changing.
            /// </summary>
            public abstract MoveSpace WalkInputVector {
                get;
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

    #endregion
    
    protected virtual void OnValidate()
    {
        Awake();
    }

    protected virtual void Awake() {
        
    }

    protected virtual void Update() {

        UpdateGround();
        UpdateForces();
        UpdateInputs();

        UpdatePosition();
        _velocityLastUpdate = Velocity;
    }

    protected abstract void UpdateGround();
    protected abstract void UpdateForces();
    protected abstract void UpdateInputs();
    protected abstract void UpdatePosition();
    
    public abstract void AddImpulse( MoveSpace velocity );
    public abstract void AddForce( MoveSpace velocity );
    

    #region Input Methods

        public abstract void AddWalkInput( InputAction.CallbackContext context );

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

    #endregion
    
    #region Events

        /// <summary>
        /// This function is called when the character touches the ground. This can be enabled or disabled for steep slopes.
        /// </summary>
        protected virtual void OnLandedGround() {

            _jumpsMade = 0;
        }

        /// <summary>
        /// This function is called when the character leaves the ground significantly, such as jumping into the air, or walking off a ledge. Includes a call for OnDetachedGround.
        /// </summary>
        protected virtual void OnLeftGround() {

            // Set the vertical velocity to zero.
            VerticalSpeed = Mathf.Max( VerticalSpeed, 0f );

            OnDetachedGround();
        }

        /// <summary>
        /// This function is called when the character leaves the ground for any reason, no matter how minute.
        /// </summary>
        protected virtual void OnDetachedGround() {

            _jumpsMade++;
        }

        protected virtual void OnReachedAirborneApex() {}
    
    #endregion

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
        _isHoldingJump = true;
        _whenJumped = Time.time;
        _jumpsMade++;

        VerticalSpeed = 0f;
    }
    
    public virtual void JumpEnd()
    {
        _isHoldingJump = false;
    }

    #region Debug Methods

        public virtual void OnDrawGizmosSelected()
        {
            if ( GizmoMovementArrow ) {
                DrawMovementArrow();
            }

            if ( GizmoVelocity ) {
                DrawVelocity();
            }
            
            if ( GizmoInput ) {
                DrawInput();
            }

            if ( GizmoGroundState ) {
                DrawGroundState();
            }
        }

        protected abstract void DrawMovementArrow();
        protected abstract void DrawVelocity();
        protected abstract void DrawInput();
        protected abstract void DrawGroundState();

    #endregion
}
