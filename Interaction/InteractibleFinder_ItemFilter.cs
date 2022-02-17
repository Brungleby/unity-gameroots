using UnityEngine;

/// <summary>
/// This component searches for Interactibles within the referenced ItemFilter.Item.Prefab GameObject.
/// </summary>
public class InteractibleFinder_ItemFilter : InteractibleFinder
{
    public override Interactible[] FindInteractibles()
    {
        return FindInteractiblesIn( Ref.Item.Prefab );
    }

    public ItemFilter Ref;
}