using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Special Container class where the order and placement of entries is recorded. For instance, a game where Items take up length and width inside of a 2D chest grid.
/// </summary>
public abstract class Container_Complex< Type, Space > : ContainerBase< Type >
{
    public abstract Item GetItem( Space at );

    protected abstract bool AddInternalAt( Item item, Space at );
    protected abstract bool RemoveInternalAt( Space at );

}
