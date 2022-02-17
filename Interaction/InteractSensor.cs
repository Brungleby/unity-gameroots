using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the component that senses Interactibles in the world.
/// </summary>
public abstract class InteractSensor : MonoBehaviour
{
    public abstract List< Interactible > FindInteractibles();

    private List< Interactible > _foci;
    public List< Interactible > GetFocusedInteractibles {
        get {
            return _foci;
        }
    }

    public bool IsFocused {
        get {
            return _foci.Count > 0;
        }
    }

    public Interactible GetInteractible( string actionType )
    {
        foreach ( Interactible i in _foci )
        {
            if ( i.IsActionAvailable( actionType ) )
                return i;
        }

        return null;
    }

    protected virtual void Awake()
    {
        _foci = new List< Interactible >();
    }

    protected virtual void Update()
    {
        _foci.Clear();
        _foci.AddRange( FindInteractibles() );
    }
}
