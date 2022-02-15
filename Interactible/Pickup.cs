using System.Collections.Generic;
using UnityEngine;

[ RequireComponent( typeof( ItemFilter ) ) ]
public class Pickup : Interactible
{
    public override string GetContextualTooltip( Interactor data )
    {
        return Item.Name;
    }

    protected override void Interact( Interactor data )
    {
        Destroy( gameObject );
    }

    // [ Tooltip( "Item added to inventory when this GameObject is picked up." ) ]
    public Item Item {
        get {
            return GetComponent<ItemFilter>().Item;
        }
        set {
            GetComponent<ItemFilter>().Item = value;
        }
    }

    
}