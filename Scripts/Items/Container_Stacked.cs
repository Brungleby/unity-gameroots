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
            return _Entries.Count;
        }
    }

    public int CountAllItems {
        get {
            int quantity = 0;
            foreach ( ItemStack entry in _Entries )
            {
                quantity += entry.Quantity;
            }

            return quantity;
        }
    }

    public override bool IsEmpty {
        get {
            foreach ( ItemStack entry in _Entries )
            {
                if ( !entry.IsEmpty )
                    return false;
            }

            return true;
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

    public void ClearPlaceholderFor( Item item )
    {
        foreach ( ItemStack entry in _Entries )
        {
            if ( item == entry.Item )
            {
                _Entries.Remove( entry );
                break;
            }
        }
    }
    public void ClearAllPlaceholders()
    {
        List< ItemStack > toRemove = new List< ItemStack >();
        foreach ( ItemStack entry in _Entries )
        {
            if ( entry.IsEmpty )
                toRemove.Add( entry );
        }

        foreach ( ItemStack entry in toRemove )
        {
            _Entries.Remove( entry );
        }
    }

    public override bool CanAdd( Item item )
    {
        switch ( StackType )
        {
            case StackLimit.Single:
                if ( FindAvailableEntryFor( item ) != null )
                    return true;
                if ( QuantityOfEntries( item ) == 1 )
                    return false;
                break;
            case StackLimit.SingleUnlimited:
            case StackLimit.Multi:
                if ( FindAvailableEntryFor( item ) != null )
                    return true;
                break;
        }

        return base.CanAdd( item );
    }

    protected override void AddInternal( Item item )
    {
        ItemStack entry = FindAvailableEntryFor( item );

        if (
            !item.Stackable ||
            StackType == StackLimit.None ||
            entry == null
        ) {
            entry = CreateEmptyStack( item );
        }

        entry.Quantity++;
    }
    protected override bool RemoveInternal( Item item )
    {
        ItemStack entry = FindRemovableEntryFor( item );

        if ( entry != null )
        {
            entry.Quantity--;

            if ( entry.IsEmpty && ( !EnablePlaceholderStacks || QuantityOfEntries( item ) > 1 ) )
            {
                _Entries.Remove( entry );
            }

            return true;
        }

        return false;
    }

    public ItemStack CreateEmptyStack( Item item )
    {
        print( "Item: " + item.DisplayName );

        ItemStack entry = new ItemStack( item, 0 );
    
        // WTF is going on. Creating the ItemStack should set the item, but printing it immediately afterwards says it's null.

        print( "Stack: " + new ItemStack( item, 0 ).Item.DisplayName );
        _Entries.Add( entry );

        return entry;
    }

    private enum StackLimit {
        [ Tooltip( "Stacking is not allowed. Treat this as a Simple container." ) ]
        None,
        [ Tooltip( "Stack items into one entry. If the upper limit of the Item's StackLimit is reached, no more Items of that type can be added, even if there is more space in this container. E.g. Monster Hunter" ) ]
        Single,
        [ Tooltip( "Stack items into one entry, but there is no limit to how many items can be added to this one entry. E.g. The Legend of Zelda: Breath of the Wild" ) ]
        SingleUnlimited,
        [ Tooltip( "Stack items into multiple entries. Items can continue to be added until all STACKS are full if adding another of an existing Item, or all SLOTS are full if adding a new Item. E.g. Minecraft" ) ]
        Multi,
    }

    [ Tooltip( "If enabled, item stacks are not removed when they are emptied. This allows item stacks to persist as placeholders until more items are added." ) ]
    public bool EnablePlaceholderStacks = false;
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
