using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This Sensor performs a Sphere cast ( line cast with some width ) to search for Interactibles. Attach this to a 1st Person camera and it will return with Interactibles directly in the center of the view!
/// </summary>
public class InteractorSensor_Linear : InteractorSensor
{
    public override Interactible GetInteractible()
    {
        RaycastHit hit = PerformCast();

        if ( hit.collider != null )
        {
            return hit.collider.GetComponentInParent< Interactible >();
        }

        return null;
    }

    [ Min( 0f ) ]
    public float SensorRadius = 0f;

    public RaycastHit PerformCast()
    {
        RaycastHit hit; Physics.SphereCast(
            SourceTransform.position, SensorRadius,
            SourceTransform.forward, out hit, MaxDistance,
            InteractionLayers, QueryTriggerInteraction.Ignore
        );

        return hit;
    }
}
