using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorCamera : Interactor
{
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
        bool success = Physics.Raycast(
            LineCamera.transform.position, LineCamera.transform.forward, out hit,
            SensorSize, SensorLayerMask, QueryTriggerInteraction.Ignore
        );

        if ( success )
            return hit.collider.GetComponent<Interactible>();
        else
            return null;
    }
}
