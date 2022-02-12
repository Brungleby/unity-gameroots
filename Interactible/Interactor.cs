using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public abstract class Interactor : MonoBehaviour
{
    protected abstract Interactible GetInteractible();

    public LayerMask SensorLayerMask;

    public UnityEvent<Interactible> OnInteractSuccess;
    public UnityEvent<Interactible> OnInteractFailure;
    public UnityEvent OnInteractNone;

    private Interactible _focus;
    public Interactible FocusInteractible {
        get {
            return _focus;
        }
    }

    public bool IsFocused {
        get {
            return _focus != null;
        }
    }

    public InteractionData InteractData {
        get {
            return GetComponent<InteractionData>();
        }
    }

    private void Update()
    {
        _focus = GetInteractible();
    }

    public void InteractWith( Interactible other )
    {
        bool success = other.TryReceiveInteraction( InteractData );
        if ( success )
            OnInteractSuccess.Invoke( other );
        else
            OnInteractFailure.Invoke( other );
    }

    public void TryInteract()
    {
        if ( IsFocused )
            InteractWith( FocusInteractible );
        else
            OnInteractNone.Invoke();
    }

    public void InputInteract( InputAction.CallbackContext context )
    {
        if ( context.started )
            TryInteract();
    }
}
