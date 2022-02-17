using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for InteractibleFinders that use Raycasting to look for Interactibles in the world, originating from the SourceTransform.
/// </summary>
public abstract class InteractibleSensor : InteractibleFinder
{
    public LayerMask InteractionLayers;
    public float MaxDistance = 2.0f;

    [ Tooltip( "Transform used to perform the Raycast from." ) ] [ SerializeField ]
    public Transform SourceTransform;

    protected override void Awake()
    {
        base.Awake();

        if ( SourceTransform == null )
            SourceTransform = transform;
    }
}
