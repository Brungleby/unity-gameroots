using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractorCamera : Interactor
{
    protected override Interactible GetInteractible()
    {
        RaycastHit hit = LineTest( SensorLength );
        if ( hit.collider )
            return hit.transform.GetComponentInParent<Interactible>();
        return null;
    }

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

    public RaycastHit LineTest( float length )
    {
        RaycastHit hit; Physics.SphereCast(
            LineCamera.transform.position, SensorRadius,
            LineCamera.transform.forward, out hit, length,
            SensorLayerMask, QueryTriggerInteraction.Ignore
        );
        return hit;
    }
}
