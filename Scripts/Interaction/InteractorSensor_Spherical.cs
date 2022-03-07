// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

/// <summary>
/// This InteractSensor is primarily for 3rd Person games. It performs a complicated series of Raycasts to determine the optimal source of Interactibles to find. First it searches within a radius around the SourceTransform, and only within a specific cone angle. Then it may check to determine if there is anything obstructing those items from view. It will try to find the closest Interactible GameObject from the conical center and work its way outwards if obstructed.
/// </summary>
// public class InteractSensor_Sphere : InteractSensor_Raycast
// {
//     protected override Interactible GetFocusedInteractible()
//     {
//         // Find ALL things in the sphere.
//         //
//         RaycastHit[] hits = Physics.SphereCastAll(
//             SensorOrigin.position, MaxDistance, Vector3.zero, 0f,
//             InteractionLayers, QueryTriggerInteraction.Ignore
//         );

//         // Filter to only things within the sphere and angle that are Interactible.
//         //
//         List<Interactible> available = new List<Interactible>();

//         foreach ( RaycastHit hit in hits )
//         {
//             Interactible item = hit.transform.GetComponentInParent<Interactible>();
//             if ( item )
//             {
//                 if ( IsAngleWithinSensor( GetAngleFromForward( item ) ) )
//                 {
//                     if ( LineCheck( item ) || !EnableLineCheck )
//                     {
//                         available.Add( item );
//                     }
//                 }
//             }
//         }

//         // Of all things within range, select the one closest to our center of 'view'.
//         //
//         Interactible optimal = null;
//         float nearestAngle = 180.0f;

//         foreach ( Interactible item in available )
//         {
//             float angle = GetAngleFromForward( item );
            
//             if ( optimal == null )
//             {
//                 optimal = item;
//                 nearestAngle = angle;
//             }

//             if ( angle < nearestAngle )
//             {
//                 optimal = item;
//                 nearestAngle = angle;
//             }
//         }

//         return optimal;
//     }

//     [ Tooltip( "Maximum angle from the AngleOrigin that Interactibles can be sensed within." ) ]
//     public float MaxAngle = 180.0f;
    
//     [ Tooltip( "Whether or not to check if an Interactible is blocked." ) ]
//     public bool EnableLineCheck = true;

//     [ Tooltip( "This is the radius of the raycast used to confirm there is nothing between the confirmation source and a prospective Interactible." ) ]
//     public float LineTestRadius = 0.1f;

//     [ Tooltip( "Custom Transform to set the origin of the sensor sphere." ) ] [ SerializeField ]
//     private Transform _SensorOrigin;
//     public Transform SensorOrigin {
//         get {
//             if ( _SensorOrigin )
//                 return _SensorOrigin;
//             return transform;
//         }
//     }

//     [ Tooltip( "Custom Transform to compare angles. Set it to a camera's transform to have available interactibles appear when looking at them with said camera." ) ] [ SerializeField ]
//     private Transform _AngleOrigin;
//     public Transform AngleOrigin {
//         get {
//             if ( _AngleOrigin )
//                 return _AngleOrigin;
//             return transform;
//         }
//     }

//     private bool IsAngleWithinSensor( float angle )
//     {
//         return angle <= MaxAngle;
//     }

//     private float GetAngleFromForward( Interactible item )
//     {
//         Vector3 normal = ( item.transform.position - transform.position ).normalized;
//         float dot = Vector3.Dot( AngleOrigin.forward, normal );
//         float angle = Mathf.Acos( dot );

//         return Mathf.Abs( angle );
//     }

//     private bool LineCheck( Interactible item )
//     {
//         Vector3 delta = item.transform.position - SensorOrigin.position;
        
//         RaycastHit hit; Physics.SphereCast(
//             SensorOrigin.position, LineTestRadius, delta.normalized,
//             out hit, delta.magnitude, InteractionLayers, QueryTriggerInteraction.Ignore
//         );

//         return hit.collider.GetComponent<Interactible>() == item;
//     }
// }
