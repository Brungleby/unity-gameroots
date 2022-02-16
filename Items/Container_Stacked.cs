using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container_Stacked : Container
{
    public override int Count {
        get {
            return 0;
        }
    }

    public int CountEntries {
        get {
            return _Entries.Count;
        }
    }

    public override bool Contains( Item item )
    {
        foreach ( ItemStack stack in _Entries )
        {
            if ( item == stack.Item )
                if ( !stack.IsEmpty )
                    return true;
        }

        return false;
    }
    public override int QuantityOf( Item item )
    {
        int result = 0;
        foreach ( ItemStack stack in _Entries )
        {
            if ( item == stack.Item )
                result += stack.Quantity;
        }

        return result;
    }

    public bool ContainsEntry( Item item )
    {
        foreach ( ItemStack stack in _Entries )
        {
            if ( item == stack.Item )
                return true;
        }

        return false;
    }
    public int QuantityOfEntries( Item item )
    {
        int result = 0;
        foreach ( ItemStack stack in _Entries )
        {
            if ( item == stack.Item )
                result++;
        }

        return result;
    }

    public override void Clear()
    {
        _Entries.Clear();
    }

    protected override bool AddInternal( Item item )
    {
        if ( item.Stackable )
        {
            ItemStack entry = FindAvailableEntryFor( item );
            if ( entry == null || ( entry.IsFull && EnableDuplicateStacks ) )
            {
                entry = new ItemStack( item, 0 );
                _Entries.Add( entry );
            }
            else if ( entry.IsFull )
            {
                return false;
            }

            entry.Quantity++;

            return true;
        }
        else
        {
            _Entries.Add( new ItemStack( item ) );

            return true;
        }
    }
    protected override bool RemoveInternal( Item item )
    {
        ItemStack entry = FindRemovableEntryFor( item );

        if ( entry != null )
        {
            entry.Quantity--;

            if ( !KeepEmptyEntries && entry.IsEmpty )
            {
                _Entries.Remove( entry );
            }

            return true;
        }

        return false;
    }

    [ Tooltip( "If enabled, item stacks are not removed when they are emptied. This allows item stacks to persist as placeholders until more items are added." ) ]
    public bool KeepEmptyEntries = false;
    public bool EnableDuplicateStacks = true;

    private List< ItemStack > _Entries;

    protected virtual void Awake()
    {
        _Entries = new List< ItemStack >();
    }

    protected virtual ItemStack FindAvailableEntryFor( Item item )
    {
        ItemStack[] matches = FindMatchingStacks( item );
        foreach ( ItemStack stack in matches )
        {
            if ( !stack.IsFull )
                return stack;
            else if ( !EnableDuplicateStacks )
                break;
        }

        return null;
    }

    protected virtual ItemStack FindRemovableEntryFor( Item item )
    {
        ItemStack[] matches = FindMatchingStacks( item );
        foreach ( ItemStack stack in matches )
        {
            if ( !stack.IsEmpty )
                return stack;
        }

        return null;
    }

    protected ItemStack[] FindMatchingStacks( Item item )
    {
        List< ItemStack > result = new List< ItemStack >();

        foreach ( ItemStack stack in _Entries )
        {
            if ( stack.Item == item )
                result.Add( stack );
        }

        return result.ToArray();
    }
}
