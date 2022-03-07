using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// This is a component that can interact with Interactibles found in the world. 
/// </summary>
[ RequireComponent( typeof( InteractorSensor ) ) ]
public class Interactor : MonoBehaviour
{
    [ SerializeField ]
    private InteractorSensor _Sensor;
    public InteractorSensor Sensor {
        get {
            return _Sensor;
        }
        private set {
            _Sensor = value;
        }
    }

    public virtual void InteractWith( Interactible other )
    {
        other.ReceiveInteraction( this );
    }

    public void TryInteract()
    {
        if ( Sensor.IsFocused )
            InteractWith( Sensor.FocusedInteractible );
    }

    public virtual void ReceiveInput( InputAction.CallbackContext context )
    {
        if ( context.started )
            TryInteract();
    }

    protected virtual void Awake()
    {
        if ( Sensor == null )
            Sensor = GetComponent< InteractorSensor >();
    }
}