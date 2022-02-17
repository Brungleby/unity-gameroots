using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container class used to store items in a stacked manner. Good for use in games where the player will be collecting lots of items, or any game where items are not unique and can be "stacked" together.
/// </summary>
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
            if ( entry == null || ( entry.IsFull && StackType == StackLimit.Multi ) )
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

    private enum StackLimit {
        [ Tooltip( "Stacking is not allowed. Treat this as a Simple container." ) ]
        None,
        [ Tooltip( "Stack items into one entry. If the upper limit of the Item's StackLimit is reached, no more Items of that type can be added. E.g. Monster Hunter" ) ]
        Single,
        [ Tooltip( "Stack items into one entry, but there is no limit to how many items can be added to this one entry. E.g. The Legend of Zelda: Breath of the Wild" ) ]
        SingleUnlimited,
        [ Tooltip( "Stack items into multiple entries. Items can continue to be added until all STACKS are full if adding another of an existing Item, or all SLOTS are full if adding a new Item. E.g. Minecraft" ) ]
        Multi,
    }

    [ Tooltip( "If enabled, item stacks are not removed when they are emptied. This allows item stacks to persist as placeholders until more items are added." ) ]
    public bool KeepEmptyEntries = false;
    [ Tooltip( "Defines how stacks are stored." ) ] [ SerializeField ]
    private StackLimit StackType = StackLimit.Multi;

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
            else if ( StackType != StackLimit.Multi )
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
