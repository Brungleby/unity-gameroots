using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Container_Complex< Type, Space > : ContainerBase< Type >
{
    public abstract Item GetItem( Space at );

    protected abstract bool AddInternalAt( Item item, Space at );
    protected abstract bool RemoveInternalAt( Space at );

}
