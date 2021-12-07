using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A complex item container model that uses ItemStacks to represent the items within. This is useful primarily for applications in which the user has a large number of the same item. E.g. Minecraft, Monster Hunter
/// </summary>
public class ItemStackContainer : ItemContainer<ItemStack>
{
    [ Tooltip( "If enabled, item stacks are not removed when they are emptied. This allows item stacks to persist as placeholders until more items are added." ) ]
    public bool KeepEmptyEntries = false;

    private ItemStack[] FindMatchingStacks( Item itemType )
    {
        List<ItemStack> result = new List<ItemStack>();

        foreach ( ItemStack stack in this )
        {
            if ( stack.Item == itemType )
                result.Add( stack );
        }

        return result.ToArray();
    }

    protected virtual ItemStack FindAvailableStackFor( Item itemType )
    {
        ItemStack[] matches = FindMatchingStacks( itemType );
        foreach ( ItemStack stack in matches )
        {
            if ( !stack.IsFull )
                return stack;
        }

        return null;
    }

    protected virtual ItemStack FindRemovableStackFor( Item itemType )
    {
        ItemStack[] matches = FindMatchingStacks( itemType );
        
        for (int i = matches.Length - 1; i >= 0 ; i--)
        {
            ItemStack stack = matches[ i ];

            if ( !stack.IsEmpty )
                return stack;
        }

        return null;
    }

    public sealed override List<ItemStack> GetOrderedList( string[] filters, bool exclusive )
    {
        List<ItemStack> result = new List<ItemStack>();

        if ( filters.Length != 0 )
        {
            foreach ( ItemStack stack in this )
            {
                if ( stack.Item.HasTags( filters, exclusive ) )
                    result.Add( stack );
            }
        }
        else
        {
            result.AddRange( this );
        }

        result.Sort();

        return result;
    }

    public sealed override int TotalItems {
        get {
            int result = 0;
            foreach ( ItemStack stack in this )
            {
                result += stack.Quantity;
            }
            return result;
        }
    }

    public sealed override bool ContainsAny( Item item )
    {
        foreach ( ItemStack stack in this )
        {
            if ( stack.Item == item )
                return true;
        }

        return false;
    }

    public override bool CanAddSingleItem( Item item )
    {
        return true;
    }

    public sealed override void AddSingleItem( Item item )
    {
        if ( item.Stackable )
        {
            ItemStack existingStack = FindAvailableStackFor( item );

            if ( existingStack != null )
            {
                existingStack.Quantity++;
                return;
            }
        }

        Add( new ItemStack( item ) );
    }

    public sealed override void RemoveSingleItem( Item item )
    {
        ItemStack existingStack = FindRemovableStackFor( item );

        if ( existingStack != null )
        {
            existingStack.Quantity--;

            if ( !KeepEmptyEntries && existingStack.IsEmpty )
            {
                Remove( existingStack );
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
}