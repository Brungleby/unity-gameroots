using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractSensor_Camera : InteractSensor_Raycast
{
    public override List< Interactible > FindInteractibles()
    {
        List< Interactible > result = new List< Interactible >();
        
        RaycastHit hit;
        bool success = Physics.SphereCast(
            LineCamera.transform.position, SensorRadius,
            LineCamera.transform.forward, out hit, MaxDistance,
            InteractionLayers, QueryTriggerInteraction.Ignore
        );
        
        if ( success )
        {
            result.AddRange( hit.transform.GetComponentsInParent< Interactible >() );
        }

        return result;
    }

    public float SensorRadius = 0.1f;

    [ Tooltip( "Camera from which to cast the sensor ray. Defaults to sibling camera component." ) ] [ SerializeField ]
    private Camera _LineCamera;
    public Camera LineCamera {
        get {
            if ( _LineCamera )
                return _LineCamera;
            return GetComponent<Camera>();
        }
    }
}
