using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the component that senses and triggers interaction on an object in the world. It should only sense Interactibles and should for the most part be stateless. Any attached InteractionData is passed to the Interactible. This way, any Interactor can use any InteractData independently.
/// </summary>
public abstract class InteractSensor : MonoBehaviour
{
    protected abstract Interactible GetInteractible();

    public LayerMask SensorLayerMask;

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
}
