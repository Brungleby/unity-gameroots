using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof( Rigidbody ) ) ]
public abstract class CharacterPhysicsMovementBase3D< Shape > : CharacterPhysicsMovementBase< Shape, Rigidbody, Vector3, Quaternion, Vector2 >
{
    public Vector2 GravityLateralPlane {
        get {
            return GravityUp.Plane();
        }
    }

    protected override void FixedUpdate()
    {
        // Velocity = Rigidbody.velocity;
        Velocity = Vector3.zero;

        base.FixedUpdate();
    }

    void OnCollisionEnter( Collision collision )
    {
        print( "Hit " + collision.ToString() );

        Vector3 hitNormal = collision.GetContact( 0 ).normal;

        // Velocity = Vector3.ProjectOnPlane( Velocity, hitNormal );
    }
    
    public abstract RaycastHit GroundCheck();

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

    #region Abstract Property Overrides

        public sealed override float GroundAngle {
            get {
                if ( !IsGrounded )
                    return 0.0f;
                return Mathf.Acos( Vector3.Dot( GroundNormal, GravityUp ) ) * Mathf.Rad2Deg;
            }
        }
        public sealed override Vector3 SlopeSlideDirection {
            get {
                return Vector3.Cross( Vector3.Cross( GravityUp, GroundNormal ), GroundNormal );
            }
        }

        public sealed override Vector3 GravityUp {
            get {
                return -GravityForce.normalized;
            }
        }
        protected override Vector3 GravityForce {
            get {
                return Physics.gravity;
            }
        }

        public sealed override float VerticalVelocity {
            get {
                return Vector3.Dot( Velocity, GravityUp );
            }
            set {
                Velocity = Vector3.Scale( Velocity, GravityLateralPlane ) + GravityUp * value;
            }
        }

        public override Vector3 Velocity {
            get {
                return Rigidbody.velocity;
            }
            set {
                Rigidbody.velocity = value;
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

        public sealed override bool IsInputtingWalk {
            get {
                return RawWalkInputAxis != Vector2.zero;
            }
        }

        public sealed override Vector3 WalkVectorSafeNormal {
            get {
                return WalkVectorLastValid.normalized;
            }
        }

    #endregion

    #region Abstract Method Overrides

        public override void AddImpulse( Vector3 velocity, bool ignoreMass = false )
        {
            // Rigidbody.Add( velocity * ( ignoreMass ? Rigidbody.mass : 1f ) );
        }
        public override void AddForce( Vector3 velocity, bool ignoreMass = false )
        {
            Velocity += velocity * ( ignoreMass ? Rigidbody.mass : 1f );
        }
    
        protected override void UpdateGround()
        {
            RaycastHit hit = GroundCheck();
            bool touchingGround = hit.collider != null;

            GroundNormal = hit.normal;
            GroundPoint  = hit.point;

            if ( touchingGround /* && Time.time > _whenJumped + GroundCheckDelayAfterJump */ )
            {
                if ( GroundAngle <= SlopeAngleLimit )
                    GroundState = EGroundState.Grounded;
                else
                    GroundState = EGroundState.Sloped;
            }
            else
            {
                GroundState = EGroundState.Airborne;
            }
        }
        protected override void UpdateForces()
        {
            // AddForce( GravityForce * Time.fixedDeltaTime );
            if ( !IsGrounded )
                Velocity += GravityForce;
        }
        protected override void UpdateInputs()
        {
            WalkVector = GetCameraAdjustedInputVector( RawWalkInputAxis );

            print( Speed );

            if ( IsInputtingWalk )
                WalkVectorLastValid = WalkVector;

            // AddForce( WalkVector * Time.fixedDeltaTime, true );
            Velocity += WalkVector * MaxWalkSpeed;
        }
        protected override void UpdatePosition()
        {
            // Rigidbody.velocity = Velocity;

            // Doesn't work because it doesn't collide with anything.
            // Rigidbody.MovePosition( Rigidbody.position + Velocity );
        }

        public override void InputWalkAxis( InputAction.CallbackContext context )
        {
            RawWalkInputAxis = context.ReadValue<Vector2>();
        }

        protected override void DrawGizmoMovementArrow()
        {
            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(
                transform.position + ( transform.forward * 1.3f ),
                Quaternion.LookRotation( -transform.forward, transform.up ),
                Vector3.one
            );
            Gizmos.DrawFrustum( Vector3.zero, 22.0f, 0.6f, 0.0f, 1.0f );
            Gizmos.matrix = Matrix4x4.identity;
        }
        protected override void DrawGizmoGroundState()
        {

        }
        protected override void DrawGizmoVelocity()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay( transform.position, Velocity * Time.deltaTime * 50f );
        }
        protected override void DrawGizmoInput()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay( transform.position, WalkVector );
        }

    #endregion
}
