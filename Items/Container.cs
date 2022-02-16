using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
