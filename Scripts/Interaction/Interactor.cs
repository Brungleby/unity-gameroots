using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This is a component that can interact with Interactibles found in the world. 
/// </summary>
public class Interactor : MonoBehaviour
{
    public string Action = "Default";

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

    public virtual void TryInteract()
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
            Sensor = GetComponentInParent< InteractorSensor >();
    }
}