using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component finds Interactibles for Interactors to Interact with.
/// </summary>
public abstract class InteractibleFinder : MonoBehaviour
{
    public abstract Interactible[] FindInteractibles();

    public static Interactible[] FindInteractiblesIn( GameObject o )
    {
        return o.GetComponentsInParent< Interactible >();
    }
    public static Interactible[] FindInteractiblesIn( Component c )
    {
        return c.GetComponentsInParent< Interactible >();
    }

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
