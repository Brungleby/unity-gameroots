using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorSphere : Interactor
{
    [ Tooltip( "Only interactibles within this angle will be considered for sensing." ) ]
    public float SensorAngle = 180.0f; 

    [ Tooltip( "Custom Transform to set the origin of the sensor sphere." ) ] [ SerializeField ]
    private Transform _SensorOrigin;
    public Transform SensorOrigin {
        get {
            if ( _SensorOrigin )
                return _SensorOrigin;
            return transform;
        }
    }

    [ Tooltip( "Custom Transform to compare angles. Set it to a camera's transform to have available interactibles appear when looking at them with said camera." ) ] [ SerializeField ]
    private Transform _AngleOrigin;
    public Transform AngleOrigin {
        get {
            if ( _AngleOrigin )
                return _AngleOrigin;
            return transform;
        }
    }

    protected override Interactible GetInteractible()
    {
        // Find ALL things in the sphere.
        //
        RaycastHit[] hits = Physics.SphereCastAll(
            SensorOrigin.position, SensorSize, Vector3.zero, 0f,
            SensorLayerMask, QueryTriggerInteraction.Ignore
        );

        // Filter to only things within the sphere and angle that are Interactible.
        //
        List<Interactible> available = new List<Interactible>();

        foreach ( RaycastHit hit in hits )
        {
            Interactible item = hit.transform.GetComponentInParent<Interactible>();
            if ( item )
            {
                if ( IsAngleWithinSensor( GetAngleFromForward( item ) ) )
                {
                    available.Add( item );
                }
            }
        }
        
        // Of all things within range, select the one closest to our center of 'view'.
        //
        Interactible optimal = null;
        float nearestAngle = 180.0f;

        foreach ( Interactible item in available )
        {
            float angle = GetAngleFromForward( item );
            
            if ( optimal == null )
            {
                optimal = item;
                nearestAngle = angle;
            }

            if ( angle < nearestAngle )
            {
                optimal = item;
                nearestAngle = angle;
            }
        }

        return optimal;
    }

    private bool IsAngleWithinSensor( float angle )
    {
        return angle <= SensorAngle;
    }

    private float GetAngleFromForward( Interactible item )
    {
        Vector3 normal = ( item.transform.position - transform.position ).normalized;
        float dot = Vector3.Dot( AngleOrigin.forward, normal );
        float angle = Mathf.Acos( dot );

        return Mathf.Abs( angle );
    }
}
