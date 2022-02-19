using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component searches for Interactibles within the referenced GameObject.
/// </summary>
public class InteractibleFinder_GameObject : InteractibleFinder
{
    public override Interactible[] FindInteractibles()
    {
        return FindInteractiblesIn( Ref );
    }

    public GameObject Ref;
}
