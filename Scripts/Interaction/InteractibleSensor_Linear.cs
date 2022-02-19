using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This Sensor performs a Sphere cast ( line cast with some width ) to search for Interactibles. Attach this to a 1st Person camera and it will return with Interactibles directly in the center of the view!
/// </summary>
public class InteractibleSensor_Linear : InteractibleSensor
{
    public override Interactible[] FindInteractibles()
    {
        RaycastHit hit = PerformCast(); 
        bool success = hit.collider != null;
        
        if ( success )
            return FindInteractiblesIn( hit.transform );

        return new Interactible[ 0 ];
    }

    [ Min( 0f ) ]
    public float SensorRadius = 0.1f;

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
