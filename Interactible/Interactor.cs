using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Interactor : MonoBehaviour
{
    protected abstract Interactible GetInteractible();
    
    public LayerMask SensorLayerMask;
    public float SensorSize = 2.0f;

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

    private void Update()
    {
        _focus = GetInteractible();
    }

    public virtual void Interact( Interactible interact )
    {
        interact.Use();
    }

    public bool TryInteract()
    {
        if ( IsFocused )
        {
            Interact( FocusInteractible );

            return true;
        }

        return false;
    }

    public void InputInteract( InputAction.CallbackContext context )
    {
        if ( context.started )
            TryInteract();
    }
}
