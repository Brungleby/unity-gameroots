using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorCamera : Interactor
{
    public float SensorLength = 2.0f;
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

    protected override Interactible GetInteractible()
    {
        RaycastHit hit;
        bool success = Physics.SphereCast(
            LineCamera.transform.position, SensorRadius,
            LineCamera.transform.forward, out hit, SensorLength,
            SensorLayerMask, QueryTriggerInteraction.Ignore
        );
        
        if ( success )
            return hit.transform.GetComponentInParent<Interactible>();
        return null;
    }
}
