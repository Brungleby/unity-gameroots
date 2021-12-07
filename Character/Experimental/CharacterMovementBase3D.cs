using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterMovementBase3D : CharacterMovementBase<Vector3, Quaternion, Vector2>
{
    private enum RotationMode
    {
        [ Tooltip( "The character will always point to the last direction walked in, even if they are not walking." ) ]
        Constant,
        [ Tooltip( "The character will try to rotate towards the last direction walked in, but will only be able to do so if we are inputting walk movement. Good for 3rd person." ) ]
        Implicit,
        [ Tooltip( "The character will not automatically rotate AND will strafe. Good for 1st person." ) ]
        Explicit,
    }


    #region Exposed Properties

        [ Header( "Rotation" ) ]

        [ SerializeField ] [ Tooltip( "Determines how this character's yaw rotation is controlled." ) ]
        private RotationMode RotationStyle = RotationMode.Implicit;
        
        [ Space( 10 ) ]
        
        [ Tooltip( "Amount of time it takes for us to reach our target rotation. Hint: the larger this value is, the more massive the character will feel." ) ]
        public float WalkRotationTime = 0.05f;
        [ Range( 0.0001f, 1f ) ] [ Tooltip( "Lower values quicken rotation when inputting walking slowly (not all the way)." ) ]
        public float WalkRotationExp = 0.5f;
    
    #endregion

    #region Private Variables

        private float _targetYaw;
        private float _targetYawVelocity;
    
    #endregion
    
    #region Public Property Methods

        public abstract Collider Collider
        {
            get;
        }

        public sealed override float GroundAngle {
            get {
                if ( GroundState == GroundMoveState.Airborne )
                    return 0.0f;
                return Mathf.Acos( Vector3.Dot( GroundNormal, GravityUp ) ) * Mathf.Rad2Deg;
            }
        }
        protected sealed override Vector3 SlopeNormal {
            get {
                return Vector3.Cross( Vector3.Cross( GravityUp, GroundNormal ), GroundNormal );
            }
        }

        #region Physics

            protected sealed override Vector3 GravityForce {
                get {
                    return Physics.gravity * GravityScale * GRAVITY_SCALE_MULTIPLIER;
                }
            }
            protected sealed override Vector3 GravityUp {
                get {
                    return -GravityForce.normalized;
                }
            }
            /// <summary>
            /// The planar vector opposing our gravity.
            /// </summary>
            public Vector3 GravityLateralPlane {
                get {
                    return GravityUp.Plane();
                }
            }
        
        #endregion


        #region Velocity

            /// <summary>
            /// The XZ speed of the character along their current gravity plane. Set value has NOT yet been tested.
            /// </summary>
            public Vector2 LateralSpeed {
                get {
                    return Vector3.ProjectOnPlane( Velocity, GravityLateralPlane ).XZtoXY();
                }
                set {
                    Velocity = Vector3.Scale( Velocity, GravityUp ) + Vector3.Scale( GravityLateralPlane, value.XYtoXZ() );
                }
            }

            public sealed override float Speed {
                get {
                    return Velocity.magnitude;
                }
                set {
                    Velocity = Velocity.normalized * value;
                }
            }

            public sealed override float VerticalSpeed {
                get {
                    return Vector3.Dot( Velocity, GravityUp );
                }
                set {
                    Velocity = Vector3.Scale( Velocity, GravityLateralPlane ) + GravityUp * value;
                }
            }
            public sealed override float VerticalSpeedPrevious {
                get {
                    return Vector3.Dot( VelocityLastUpdate, GravityUp );
                }
            }

        #endregion

        #region Walk Input
            public sealed override bool IsInputtingWalk {
                get {
                    return WalkInputAxis != Vector2.zero;
                }
            }
            public sealed override Vector3 WalkInputVector {
                get {
                    // Vector3 right = InputAnchor.right * _walkInput.x;
                    // Vector3 forward = Vector3.Scale( InputAnchor.forward, Vector3.up.Plane() ) * _walkInput.y;

                    // return right + forward;

                    return WalkInputAxis.XYtoXZ();
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
                        return WalkInputAxis.normalized;
                    else
                        return WalkInputAxisLastValid.normalized;
                }
            }
             
        #endregion

        public Transform InputAnchor
        {
            get
            {
                return transform;
            }
        }

    #endregion

    #region Overrides

        protected override void Awake()
        {
            base.Awake();
    
            _targetYaw = transform.rotation.y;
        }
         
        protected override void UpdateGround()
        {
            RaycastHit hit = GroundCheck();
            bool isTouchingGround = hit.collider != null;

            GroundNormal = hit.normal;
            GroundPoint = hit.point;
        
            if ( isTouchingGround && IsGroundImmunityExpired )
            {
                if ( GroundAngle <= SlopeAngleLimit )
                    GroundState = GroundMoveState.Grounded;
                else
                    GroundState = GroundMoveState.Sloped;
            }
            else
            {
                GroundState = GroundMoveState.Airborne;
            }
        }
    protected override void UpdateForces()
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
    protected override void UpdateInputs()
    {
        float walkPercentAccel = IsGrounded ? 1f : AirWalkPercent;
        float walkPercentDecel = IsGrounded ? 1f : 0f;

        Vector3 walkNormal;

        // Handle walking rotation here as well. 
        if ( RotationStyle == RotationMode.Explicit )
        {
            walkNormal = WalkInputVector;
        }
        else
        {
            walkNormal = transform.forward;

            if ( IsInputtingWalk || RotationStyle == RotationMode.Constant )
            {
                _targetYaw = Mathf.Atan2( WalkYawTarget.x, WalkYawTarget.y ) * Mathf.Rad2Deg; // * camera y
                
                float rotationDivisor = IsInputtingWalk ? Mathf.Pow( WalkInputAxis.magnitude, WalkRotationExp ) : 1f;

                float rotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y, _targetYaw, ref _targetYawVelocity,
                    WalkRotationTime / rotationDivisor
                );

                transform.rotation = Quaternion.Euler( 0f, rotation, 0f );
            }
        }

        Vector3 walkVector;

        if ( GroundState == GroundMoveState.Sloped && !EnableSlopeWalk )
            walkVector = Vector3.zero;
        else
            walkVector = Vector3.ProjectOnPlane( walkNormal, GravityUp );

        AddForce( walkVector * WalkInputAxis.magnitude * walkPercentAccel * WalkGroundAccel );
        AddForce( -Vector3.Scale( Velocity, GravityLateralPlane ) * walkPercentDecel * WalkGroundDecel );

        // Limit the lateral speed to the Max Walk Speed.
        Velocity = Vector3.ClampMagnitude( Vector3.Scale( Velocity, GravityLateralPlane ), MaxWalkSpeed * Time.deltaTime ) + GravityUp * VerticalSpeed;
    }

    protected override void UpdatePosition()
    {
        Move( Velocity );
    }

    #endregion

    protected abstract RaycastHit GroundCheck();

    protected abstract bool MoveSweep( Vector3 origin, Vector3 target, out RaycastHit hit );
    public void MoveTo( Vector3 position, bool teleport = true )
    {
        if ( teleport ) {
            transform.position = position;
        }
        else {
            Vector3 delta = position - transform.position;

            RaycastHit hit;
            bool isBlocked = MoveSweep( transform.position, position, out hit );

            if ( isBlocked )
                transform.position = transform.position + delta.normalized * hit.distance;
            else
                transform.position = position;
        }
    }
    public void Move( Vector3 translation, bool teleport = false )
    {
        MoveTo( transform.position + translation, teleport );
    }
    
    public sealed override void AddImpulse( Vector3 velocity )
    {
        Velocity += velocity;
    }
    public sealed override void AddForce( Vector3 velocity )
    {
        Velocity += velocity * Time.deltaTime;
    }

    public override void Jump()
    {
        base.Jump();

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
    }

    #region Input Methods

        public override void AddWalkInput( InputAction.CallbackContext context )
        {
            Vector2 axis = context.ReadValue<Vector2>();
            WalkInputAxis = axis;

            if ( axis.magnitude > 0.25f )
                WalkInputAxisLastValid = axis;
        }

    #endregion

    #region Debug Methods

        protected sealed override void DrawMovementArrow()
        {
            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position + transform.forward * ( Collider.bounds.extents.magnitude + 0.25f ),
                Quaternion.LookRotation( -transform.forward, transform.up ),
                Vector3.one
            );
            Gizmos.DrawFrustum( Vector3.zero, 22.0f, 0.6f, 0.0f, 1.0f );
            Gizmos.matrix = Matrix4x4.identity;
        }

        protected sealed override void DrawVelocity()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay( transform.position, Velocity * Time.deltaTime * 40.0f );
        }

        protected sealed override void DrawInput()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay( transform.position, WalkInputVector );
        }

    #endregion
}
