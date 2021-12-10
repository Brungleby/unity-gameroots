using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterPhysicsMovementBase< Shape, BodyT, MoveVector, RotateSpace, InputAxis > : MonoBehaviour
{
    #region Static Definitions

        protected enum EGroundState
        {
            [ Tooltip( "In the air, not touching the ground at all." ) ]
            Airborne,
            [ Tooltip( "We're touching the ground, but it's too steep to walk on." ) ]
            Sloped,
            [ Tooltip( "We're touching the ground and can move around on it." ) ]
            Grounded,
        }

    #endregion

    #region Exposed Fields

        [ Header( "Required Components" ) ]
        
        [ Tooltip( "The main camera that is attached to this character." ) ] [ SerializeField ]
        protected Camera _CharacterCamera;

        [ Header( "Collision" ) ]
        
        [ Tooltip( "Determines which layers to block when moving. It is recommended that you set this gameObject's layer to something other than these layers, to prevent colliding with self." ) ] [ SerializeField ]
        protected LayerMask CollisionLayers;

        [ Tooltip( "If enabled, the character will try to stay stuck to the ground until they explicitly jump or walk off a ledge. Otherwise, use normal physics." ) ]
        public bool EnableStickyGround = true;

        [ Range( 0f, 90f ) ] [ Tooltip( "This is the angle of slope above which is too steep to walk up." ) ]
        public float SlopeAngleLimit = 50.0f;

        [ Header( "Walk Movement" ) ]

        [ Tooltip( "If enabled, the character will be allowed to attempt to move on steep slopes. Otherwise, they will continue to slide until they reach acceptable ground." ) ]
        public bool EnableSlopeWalk = true;
        [ Tooltip( "If enabled, the character will slide down slopes that are too steep to walk on." ) ]
        public bool EnableSlopeSlide = true;

        [ Space( 10 ) ]

        [ Tooltip( "Maximum speed we can walk, measured in m/s." ) ]
        public float MaxWalkSpeed = 10.0f;

        [ Header( "Jump Movement" ) ]

        public bool EnableJump = true;
        public int JumpCount = 1;


    #endregion

    #region Private Variables

        protected Shape Collider;
        protected BodyT Rigidbody;

        private bool _isSimulatingPhysics;

        private MoveVector _velocity;
        private MoveVector _velocityLastUpdate;

        private EGroundState _groundState;
        private MoveVector _groundPoint;
        private MoveVector _groundNormal;

        private MoveVector _walkVector;
        private MoveVector _walkVectorLastValid;

        private InputAxis _rawWalkInputAxis;

        private bool _isHoldingJump;
        private float _whenJumped;
        private int _jumpsMade;

    #endregion

    #region Properties

        #region

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

        #region Ground State

            public abstract float GroundAngle { get; }
            public abstract MoveVector SlopeSlideDirection { get; }
            
            protected EGroundState GroundState {
                get {
                    return _groundState;
                }
                set {
                    EGroundState previousState = _groundState;
                    _groundState = value;

                    if ( value != previousState ) {
                        if ( value == EGroundState.Airborne )
                            _OnLeftGround();
                        else if ( previousState == EGroundState.Airborne )
                            _OnLandedGround();
                    }
                }
            }

            public bool IsGrounded {
                get {
                    return GroundState != EGroundState.Airborne;
                }
            }

            public float GroundCheckOffset {
                get {
                    return 0.3f;
                }
            }

            public MoveVector GroundPoint {
                get {
                    return _groundPoint;
                }
                protected set {
                    _groundPoint = value;
                }
            }

            public MoveVector GroundNormal {
                get {
                    return _groundNormal;
                }
                protected set {
                    _groundNormal = value;
                }
            }

        #endregion

        #region Physics & Velocity

            public abstract MoveVector GravityUp { get; }
            protected abstract MoveVector GravityForce { get; }
            public abstract float VerticalVelocity { get; set; } 
            public abstract float Speed { get; set; }

            public abstract MoveVector Velocity { get; set; }
            // public MoveVector Velocity {
            //     get {
            //         return _velocity;
            //     }
            //     set {
            //         _velocity = value;
            //     }
            // }
            public MoveVector VelocityLastUpdate {
                get {
                    return _velocityLastUpdate;
                }
            }

        #endregion

        #region Input, Walk, Jump

            public abstract bool IsInputtingWalk { get; }
            public abstract MoveVector WalkVectorSafeNormal { get; }
            
            public MoveVector WalkVector {
                get {
                    return _walkVector;
                }
                protected set {
                    _walkVector = value;
                }
            }
            public MoveVector WalkVectorLastValid {
                get {
                    return _walkVectorLastValid;
                }
                protected set {
                    _walkVectorLastValid = value;
                }
            }

            protected InputAxis RawWalkInputAxis {
                get {
                    return _rawWalkInputAxis;
                }
                set {
                    _rawWalkInputAxis = value;
                }
            }

            public virtual bool CanJump {
                get {
                    return EnableJump && _jumpsMade < JumpCount;
                }
            }
            
        #endregion

    #endregion

    #region Events
    
        /// <summary>
        /// This function is called when the player touches any ground, even steep slopes.
        /// </summary>
        protected virtual void OnLandedGround() {}
        private void _OnLandedGround()
        {
            OnLandedGround();
        }
        
        /// <summary>
        /// This function is called when the character leaves the ground significantly, such as jumping into the air, or walking off a ledge.
        /// </summary>
        protected virtual void OnLeftGround() {}
        private void _OnLeftGround()
        {
            OnLeftGround();
        }

    #endregion

    #region Inherited from MonoBehaviour

        protected virtual void OnValidate()
        {
            Collider = GetComponent<Shape>();
            Rigidbody = GetComponent<BodyT>();
        }

        protected abstract void UpdateGround();
        
        protected abstract void UpdateForces();
        protected abstract void UpdateInputs();
        protected abstract void UpdatePosition();

        public abstract void AddImpulse( MoveVector velocity, bool ignoreMass = false );
        public abstract void AddForce( MoveVector velocity, bool ignoreMass = false );

        protected virtual void FixedUpdate()
        {
            UpdateGround();
            UpdateForces();
            UpdateInputs();

            UpdatePosition();
            _velocityLastUpdate = Velocity;
        }

    #endregion

    #region Jump

        public virtual void Jump()
        {
            _isHoldingJump = true;
            _whenJumped = Time.time;
            _jumpsMade++;

            VerticalVelocity = 0f;
        }

        public virtual void JumpEnd()
        {
            _isHoldingJump = false;
        }
    
        public bool TryJumpStart()
        {
            if ( CanJump )
            {
                Jump();
                return true;
            }

            return false;
        }

        public bool TryJumpEnd()
        {
            if ( _isHoldingJump )
            {
                JumpEnd();
                return true;
            }

            return false;
        }

    #endregion

    #region Input Methods
    
        public abstract void InputWalkAxis( InputAction.CallbackContext context );
        
        public void InputJumpAction( InputAction.CallbackContext context )
        {
            if ( context.started )
                TryJumpStart();
            else if ( context.canceled )
                TryJumpEnd();
        }

    #endregion

    #region Debug Methods
        
        [ SerializeField ] [ Tooltip( "Draws an arrow pointing in the direction the character is currently moving." ) ]
        private bool DrawMovementArrow = true;
        [ SerializeField ] [ Tooltip( "Draws a red line from the point of contact (if it exists) in the direction of the ground normal." ) ]
        private bool DrawGroundState = false;
        [ SerializeField ] [ Tooltip( "Draws a blue line from the center of the character in the direction and magnitude of their velocity." ) ]
        private bool DrawVelocity = false;
        [ SerializeField ] [ Tooltip( "Draws a white line from the center of the character in the direction and magnitude of their input velocity." ) ]
        private bool DrawInput = false;
    
        protected virtual void OnDrawGizmosSelected()
        {
            if ( DrawMovementArrow ) {
                DrawGizmoMovementArrow();
            }

            if ( DrawGroundState ) {
                DrawGizmoGroundState();
            }

            if ( DrawVelocity ) {
                DrawGizmoVelocity();
            }
            
            if ( DrawInput ) {
                DrawGizmoInput();
            }

        }

        protected abstract void DrawGizmoMovementArrow();
        protected abstract void DrawGizmoGroundState();
        protected abstract void DrawGizmoVelocity();
        protected abstract void DrawGizmoInput();

    #endregion
}
