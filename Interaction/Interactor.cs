using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// This component can receive input to trigger interactions with Interactibles (found using an attached InteractibleFinder). You will need to use one Interactor per type of Interaction you would like to implement, denoted by the ActionType.
/// </summary>
public class Interactor : MonoBehaviour
{
    [ SerializeField ]
    public InteractibleFinder Sensor;

    public string ActionType = "Default";

    [ SerializeField ]
    private UnityEvent< Interactible.Interaction > OnInteract;
    [ SerializeField ]
    private UnityEvent OnInteractIgnored;

    protected virtual void Awake()
    {
        if ( Sensor == null )
            Sensor = GetComponent< InteractibleFinder >();
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
