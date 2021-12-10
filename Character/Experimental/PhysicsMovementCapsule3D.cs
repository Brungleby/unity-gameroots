using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ RequireComponent( typeof( CapsuleCollider ) ) ]
public class PhysicsMovementCapsule3D : CharacterPhysicsMovementBase3D< CapsuleCollider >
{
    #region Public Property Methods

        #region Capsule Geometry

            /// <summary>
            /// Half the height of the Controller capsule, from the center to the end of the capsule.
            /// </summary>
            public float CapsuleHalfHeight
            {
                get
                {
                    return Collider.height / 2f;
                }
            }
            /// <summary>
            /// Half the height of the Controller capsule, not including the hemispherical portion(s).
            /// </summary>
            public float CapsuleHalfHeightNoHemisphere
            {
                get
                {
                    return CapsuleHalfHeight - Collider.radius;
                }
            }
            /// <summary>
            /// The position of the very top of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleTop
            {
                get
                {
                    return transform.position + Collider.center + transform.up * CapsuleHalfHeight; 
                }
            }
            /// <summary>
            /// The position of the center of the top hemisphere of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleTopNoHemisphere
            {
                get
                {
                    return transform.position + Collider.center + transform.up * CapsuleHalfHeightNoHemisphere;
                }
            }
            /// <summary>
            /// The position of the very bottom of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleBottom
            {
                get
                {
                    return transform.position + Collider.center - transform.up * CapsuleHalfHeight; 
                }
            }
            /// <summary>
            /// The position of the center of the bottom hemisphere of the Controller capsule in world space.
            /// </summary>
            public Vector3 CapsuleBottomNoHemisphere
            {
                get
                {
                    return transform.position + Collider.center - transform.up * CapsuleHalfHeightNoHemisphere;
                }
            }

        #endregion

    #endregion

    #region Abstract Method Overrides

        public sealed override RaycastHit GroundCheck()
        {
            RaycastHit result;

            // Physics.SphereCast(
            //     CapsuleBottomNoHemisphere, _Controller.radius,
            //     -transform.up, out result, _stepOffset, GroundCheckLayers,
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
                groundPoint1, groundPoint2, Collider.radius,
                -GravityUp, out result, GroundCheckOffset,
                CollisionLayers, QueryTriggerInteraction.Ignore
            );

            return result;
        }

    #endregion
}
