using UnityEngine;

/// <summary>
/// A basic listing for representing items in complex containers.
/// </summary>
public class ItemStack : System.IComparable<ItemStack>
{
    public int CompareTo( ItemStack other )
    {
        return Item.CompareTo( other.Item );
    }

    public ItemStack( Item itemType, int quantity, int capacity, bool inheritCapacity )
    {
        _ItemType = itemType;
        _InheritCapacity = inheritCapacity;
        _Capacity = capacity;
        Quantity = quantity;
    }
    public ItemStack( Item itemType, int quantity, int capacity )
    {
        new ItemStack( itemType, quantity, capacity, false );
    }
    public ItemStack( Item itemType, int quantity )
    {
        new ItemStack( itemType, quantity, 0, true );
    }
    public ItemStack( Item itemType )
    {
        new ItemStack( itemType, 1 );
    }

    /// <summary>
    /// Returns the Name or PluralName of the item, depending on the quantity of this listing (only uses the Name if there is 1.)
    /// </summary>
    public string QuantizedName {
        get {
            return Mathf.Abs( Quantity ) == 1 ? _ItemType.Name : _ItemType.PluralName;
        }
    }

    public bool IsEmpty {
        get {
            return Quantity == 0;
        }
    }
    public bool IsFull {
        get {
            return Quantity >= Capacity;
        }
    }

    [ SerializeField ]
    private Item _ItemType;
    public Item Item {
        get {
            return _ItemType;
        }
    }

    [ SerializeField ]
    private bool _InheritCapacity;

    [ SerializeField ]
    private int _Quantity;
    /// <summary>
    /// The current number of this item that is stored.
    /// </summary>
    public int Quantity {
        get {
            return _Quantity;
        }
        set {
            _Quantity = Mathf.Clamp( value, 0, Capacity );
        }
    }

    [ SerializeField ]
    private int _Capacity;
    /// <summary>
    /// The maximum number of this item that can be stored.
    /// </summary>
    public int Capacity {
        get {
            if ( _InheritCapacity )
                return _ItemType.Capacity;
            else
                return _Capacity > 0 ? _Capacity : int.MaxValue;
        }
    }
}
