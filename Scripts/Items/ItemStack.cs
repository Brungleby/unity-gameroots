/// <summary>
/// A basic listing for representing items in stacked containers.
/// </summary>
public class ItemStack : System.IComparable< ItemStack >
{
    public int CompareTo( ItemStack other )
    {
        return Item.CompareTo( other.Item );
    }

    public override string ToString()
    {
        return base.ToString() + "( " + Item.ID + " )";
    }

    public ItemStack( Item item, int quantity, int capacity, bool inheritCapacity )
    {
        _Item = item;
        _InheritCapacity = inheritCapacity;
        _Capacity = capacity;
        Quantity = quantity;
    }
    public ItemStack( Item item, int quantity, int capacity )
    {
        _Item = item;
        _Capacity = capacity;
        Quantity = quantity;

        _InheritCapacity = false;
    }
    public ItemStack( Item item, int quantity )
    {
        _Item = item;
        Quantity = quantity;

        _Capacity = 0;
        _InheritCapacity = true;
    }
    public ItemStack( Item item )
    {
        _Item = item;
        
        Quantity = 0;
        _Capacity = 0;
        _InheritCapacity = true;
    }

    /// <summary>
    /// Returns the Name or PluralName of the item, depending on the quantity of this listing (only uses the Name if there is 1.)
    /// </summary>
    public string QuantizedName {
        get {
            return _Item.QuantizedName( Quantity );
        }
    }

    public bool IsEmpty {
        get {
            return Quantity == 0;
        }
    }
    public bool IsFull {
        get {
            return Capacity > 0 && Quantity >= Capacity;
        }
    }

    private Item _Item;
    public Item Item {
        get {
            return _Item;
        }
    }

    private bool _InheritCapacity;

    private int _Quantity;
    /// <summary>
    /// The current number of this item that is stored.
    /// </summary>
    public int Quantity {
        get {
            return _Quantity;
        }
        set {
            _Quantity = value;
        }
    }

    private int _Capacity;
    /// <summary>
    /// The maximum number of this item that can be stored.
    /// </summary>
    public int Capacity {
        get {
            if ( _InheritCapacity )
                return _Item.StackCapacity;
            else
                return _Capacity;
        }
    }
}
