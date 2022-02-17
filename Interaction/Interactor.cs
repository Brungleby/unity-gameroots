using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public abstract class Interactor : MonoBehaviour
{
    [ SerializeField ]
    public InteractSensor Sensor;

    public string ActionType;

    [ SerializeField ]
    private UnityEvent< Interactible.Interaction > OnInteract;
    [ SerializeField ]
    private UnityEvent OnInteractIgnored;

    protected virtual void Awake()
    {
        if ( Sensor == null )
            Sensor = GetComponent<InteractSensor>();
    }

    public void InteractWith( Interactible other )
    {
        Interactible.Interaction interaction = other.ReceiveInteraction( this, ActionType );
        if ( interaction != null )
        {
            OnInteract.Invoke( interaction );
        }
    }

    public void TryInteract()
    {
        Interactible focus = Sensor.GetInteractible( ActionType );
        if ( focus != null )
            InteractWith( focus );
        else
            OnInteractIgnored.Invoke();
    }

    public void ReceiveInput( InputAction.CallbackContext context )
    {
        if ( context.started )
            TryInteract();
    }
}
