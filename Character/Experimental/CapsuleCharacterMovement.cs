using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof( CapsuleCollider ) ) ]
public class CapsuleCharacterMovement : CharacterMovementBase3D
{
    #region Private Variables

        private CapsuleCollider _Capsule;

    #endregion

    #region Public Property Methods

        public override Collider Collider {
            get {
                return _Capsule;
            }
        }

        #region Capsule Geometry

            /// <summary>
            /// Half the height of the Controller capsule, from the center to the end of the capsule.
            /// </summary>
            public float CapsuleHalfHeight
            {
                get
                {
                    return _Capsule.height / 2f;
                }
            }
            /// <summary>
            /// Half the height of the Controller capsule, not including the hemispherical portion(s).
            /// </summary>
            public float CapsuleHalfHeightNoHemisphere
            {
                get
                {
                    return CapsuleHalfHeight - _Capsule.radius;
                }
            }
            /// <summary>
            /// The position of the very top of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleTop
            {
                get
                {
                    return transform.position + _Capsule.center + transform.up * CapsuleHalfHeight; 
                }
            }
            /// <summary>
            /// The position of the center of the top hemisphere of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleTopNoHemisphere
            {
                get
                {
                    return transform.position + _Capsule.center + transform.up * CapsuleHalfHeightNoHemisphere;
                }
            }
            /// <summary>
            /// The position of the very bottom of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleBottom
            {
                get
                {
                    return transform.position + _Capsule.center - transform.up * CapsuleHalfHeight; 
                }
            }
            /// <summary>
            /// The position of the center of the bottom hemisphere of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleBottomNoHemisphere
            {
                get
                {
                    return transform.position + _Capsule.center - transform.up * CapsuleHalfHeightNoHemisphere;
                }
            }

        #endregion

    #endregion

    protected override void Awake()
    {
        _Capsule = GetComponent<CapsuleCollider>();

        base.Awake();
    }

    protected sealed override RaycastHit GroundCheck()
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
            groundPoint1, groundPoint2, _Capsule.radius + SkinWidth,
            -GravityUp, out result, StepHeight,
            CollisionLayers, QueryTriggerInteraction.Ignore
        );

        return result;
    }

    protected override bool MoveSweep( Vector3 origin, Vector3 target, out RaycastHit hit )
    {
        Vector3 delta = target - origin;

        return Physics.CapsuleCast(
            CapsuleBottomNoHemisphere, CapsuleTopNoHemisphere, _Capsule.radius + SkinWidth,
            delta.normalized, out hit, delta.magnitude,
            CollisionLayers, QueryTriggerInteraction.Ignore
        );
    }

    protected override void DrawGroundState()
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

        Vector3 groundPoint1 = CapsuleBottomNoHemisphere - GravityUp * StepHeight;
        Vector3 groundPoint2 = CapsuleTopNoHemisphere    - GravityUp * StepHeight;

        if ( !completelyUpsidedown )
            Gizmos.DrawSphere( groundPoint1, _Capsule.radius );

        if ( !completelyUpright )
            Gizmos.DrawSphere( groundPoint2, _Capsule.radius );
    }
}
