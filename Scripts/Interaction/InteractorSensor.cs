using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a component that senses interactibles in the world for an Interactor to use.
/// </summary>
public abstract class InteractorSensor : MonoBehaviour
{
    public abstract Interactible GetInteractible();



    public static Interactible[] FindInteractiblesIn( GameObject o )
    {
        return o.GetComponentsInParent< Interactible >();
    }
    public static Interactible[] FindInteractiblesIn( Component c )
    {
        return c.GetComponentsInParent< Interactible >();
    }



    public LayerMask InteractionLayers;
    public float MaxDistance = 2.0f;

    [ Tooltip( "Transform used to perform the Raycast from." ) ] [ SerializeField ]
    public Transform SourceTransform;



    private Interactible _FocusedInteractible;
    public Interactible FocusedInteractible {
        get {
            return _FocusedInteractible;
        }
        private set {
            _FocusedInteractible = value;
        }
    }

    public bool IsFocused {
        get {
            return _FocusedInteractible != null;
        }
    }



    protected virtual void Awake()
    {
        if ( SourceTransform == null )
            SourceTransform = transform;
    }

    protected virtual void Update()
    {
        FocusedInteractible = GetInteractible();
    }
}
