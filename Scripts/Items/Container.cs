using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This ContainerBase contains ONLY items, although the method of storage is still yet to be decided. This class can't be added as a component due to its abstraction, but any other Component can expose a Container field due to its Items-only nature.
/// </summary>
public abstract class Container : ContainerBase< Item >
{
    public int Add( Item item, int quantity )
    {
        while ( quantity > 0 )
        {
            bool success = Add( item );
            if ( success )
                quantity--;
            else
                return quantity;
        }

        return 0;
    }

    public int Remove( Item item, int quantity )
    {
        while ( quantity > 0 )
        {
            bool success = Remove( item );
            if ( success )
                quantity--;
            else
                return quantity;
        }

        return 0;
    }
}
