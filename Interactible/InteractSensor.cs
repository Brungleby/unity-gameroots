using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the component that senses interactibles in the world. An attached Interactor can be used to read data from this sensor.
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
