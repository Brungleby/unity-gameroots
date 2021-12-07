using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple item container for storing individual items separately. Useful for games with lots of key items or fewer types and quantities of items. E.g. Subnautica, Undertale, Diablo II.
/// </summary>
public class ItemSimpleContainer : ItemContainer<Item>
{
    [ SerializeField ]
    private Item[] _InitialContents;

    public sealed override List<Item> GetOrderedList( string[] filters, bool exclusive )
    {
        List<Item> result = new List<Item>();
        
        bool doFilter = filters.Length != 0;

        // Filter the list.
        //
        if ( filters.Length != 0 )
        {
            foreach ( Item item in this )
            {
                if ( item.HasTags( filters, exclusive ) )
                    result.Add( item );
            }
        }
        else
        {
            result.AddRange( this );
        }

        result.Sort();

        return result;
    }

    public sealed override int TotalItems => this.Count;

    public sealed override bool ContainsAny( Item item )
    {
        return Contains( item );
    }

    public sealed override bool CanAdd( Item entry )
    {
        return CanAddSingleItem( entry );
    }

    public override bool CanAddSingleItem( Item item )
    {
        return CanAdd( item );
    }

    public sealed override void AddSingleItem( Item item )
    {
        Add( item );
    }

    public sealed override void RemoveSingleItem( Item item )
    {
        Remove( item );
    }

    protected override void Awake()
    {
        base.Awake();

        foreach ( Item item in _InitialContents )
        {
            Add( item );
        }
    }
}