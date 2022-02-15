using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The base class for items which can be stored inside containers and from there, filtered and sorted. Supports a few basic properties most games' items will have.
/// </summary>
[ CreateAssetMenu( fileName = "New Item", menuName = "Inventory/Basic Item", order = 100 ) ]
public class Item : ScriptableObject, System.IComparable<Item>
{
    #region Overrides : ScriptableObject

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        public override string ToString()
        {
            return ID;
        }

    #endregion

    #region Overrides : IComparable<Item>

        public int CompareTo( Item other )
        {
            return Mathf.FloorToInt( Mathf.Sign( ListOrder - other.ListOrder ) );
        }

    #endregion

    [ Header( "Info" ) ]

    [ SerializeField ] [ Tooltip( "The internal name of this item, used for comparing item equality (which is used when stacking item listings)." ) ]
    private string _ID = "";
    public string ID {
        get {
            if ( _ID == string.Empty )
                return Name;
            else
                return _ID;
        }
    }
    [ Tooltip( "The display name of this item." ) ]
    public string Name = "";
    [ SerializeField ] [ Tooltip( "Plural form of this item that can be used when describing multiple instances of this item. If left unspecified, defaults to Name." ) ]
    private string _PluralName;
    public string PluralName {
        get {
            if ( _PluralName == string.Empty )
                return Name;
            else
                return _PluralName;
        }
        set {
            _PluralName = value;
        }
    }
    [ TextArea( 4, 10 ) ] [ Tooltip( "The display description of this item." ) ]
    public string Description = "";

    [ Header( "General" ) ]

    [ Tooltip( "The GameObject to Instantiate for this Item." ) ] [ SerializeField ]
    public GameObject Prefab;

    [ Space( 10 ) ]

    [ Tooltip( "Whether or not players can use this item directly." ) ]
    public bool Usable = true;
    [ Tooltip( "Whether or not this item is consumed when it is used." ) ]
    public bool Consumable = false;
    [ Tooltip( "Whether or not this item can be sold at shops." ) ]
    public bool Sellable = false;
    [ Tooltip( "How much this item can be bought for at shops." ) ]
    public int BaseValue = 0;

    [ Header( "Item Listing" ) ]

    [ Tooltip( "Icon that can represent this Item." ) ]
    public Sprite Icon;
    [ Tooltip( "Whether or not this Item can be consolidated into a stack of similar types. In other words, is it NOT unique?" ) ]
    public bool Stackable = true;
    [ Min( 0 ) ] [ Tooltip( "How many of this item can be stored into a single item listing. If left unspecified at 0, there will be no limit." ) ]
    public static int _Capacity = 0;
    public int Capacity {
        get {
            return _Capacity > 0 ? _Capacity : int.MaxValue;
        }
    }
    [ Tooltip( "If a container is auto-sorted, this is the order in which this item will be sorted. Smaller numbers are sorted first." ) ]
    public int ListOrder = 0;
    [ SerializeField ] [ Tooltip( "Set of categorizational tags that apply to this item. Accessed as a HashSet via code." ) ]
    private string[] _FilterTags;
    public HashSet<string> FilterTags {
        get {
            HashSet<string> result = new HashSet<string>();

            foreach ( string tag in _FilterTags )
            {
                result.Add( tag );
            }

            return result;
        }
        set {
            _FilterTags = new string[ value.Count ];
            value.CopyTo( _FilterTags );
        }
    }

    public bool HasTag( string filter )
    {
        return FilterTags.Contains( filter );
    }
    public bool HasTags( string[] filters, bool exclusive )
    {
        foreach ( string filter in filters )
        {
            if ( HasTag( filter ) != exclusive )
                return !exclusive;
        }

        return exclusive;
    }
    public bool HasAllTags( string[] filters )
    {
        return HasTags( filters, true );
    }
    public bool HasAnyTags( string[] filters )
    {
        return HasTags( filters, false );
    }
}
