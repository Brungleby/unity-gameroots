using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// This component's data will be sent to Interactibles that the Sensor interacts with. This is what allows items to interact with specific players/entities.
/// </summary>
[ RequireComponent( typeof( InteractSensor ) ) ]
public class Interactor : MonoBehaviour
{
    [ SerializeField ]
    private UnityEvent<Interactible> OnInteractSuccess;
    [ SerializeField ]
    private UnityEvent<Interactible> OnInteractFailure;
    [ SerializeField ]
    private UnityEvent OnInteractNone;

    public InteractSensor Sensor {
        get {
            return GetComponent<InteractSensor>();
        }
    }

    public void InteractWith( Interactible other )
    {
        bool success = other.ReceiveInteraction( this );
        if ( success )
            OnInteractSuccess.Invoke( other );
        else
            OnInteractFailure.Invoke( other );
    }

    public void TryInteract()
    {
        if ( Sensor.IsFocused )
            InteractWith( Sensor.FocusInteractible );
        else
            OnInteractNone.Invoke();
    }

    public void InputInteract( InputAction.CallbackContext context )
    {
        if ( context.started )
            TryInteract();
    }
}