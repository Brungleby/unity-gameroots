using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple container class for storing Items. Items in this container cannot stack; duplicates are listed separately. Good for games that have only a few key items or adventure games.
/// </summary>
public class Container_Simple : Container
{
    public override int Count => _Contents.Count;

    public override bool Contains( Item item )
    {
        foreach ( Item i in _Contents )
        {
            if ( item == i )
                return true;
        }

        return false;
    }
    public override int QuantityOf( Item item )
    {
        int result = 0;
        foreach ( Item i in _Contents )
        {
            if ( item == i )
                result++;
        }

        return result;
    }

    public override void Clear()
    {
        _Contents.Clear();
    }

    protected override void AddInternal( Item item )
    {
        _Contents.Add( item );
    }
    protected override bool RemoveInternal( Item item )
    {
        return _Contents.Remove( item );
    }

    private List< Item > _Contents;

    protected virtual void Awake()
    {
        _Contents = new List< Item >();
    }
}
