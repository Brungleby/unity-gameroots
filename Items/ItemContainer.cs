using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for item storage. By default there are no restrictions for what items can or cannot be added. These can be added by creating a child class and overriding CanAdd() and/or CanAddSingleItem().
/// </summary>
public abstract class ItemContainer<T> : MonoBehaviour, ICollection<T>
{
    private List<T> _Contents;

    public bool IsEmpty {
        get {
            return Count == 0;
        }
    }
    public virtual bool IsFull {
        get {
            return false;
        }
    }

    /// <param name="exclusive">
    /// If set to true, return only items that contain ALL filters. If false, return all items that contain ANY filters.
    /// </param>
    public abstract List<T> GetOrderedList( string[] filters, bool exclusive = false );
    public List<T> GetOrderedList( string filter, bool exclusive = false )
    {
        return GetOrderedList( new string[ 1 ] { filter }, exclusive );
    }
    public List<T> GetOrderedList( bool exclusive = false )
    {
        return GetOrderedList( new string[ 0 ], exclusive );
    }

    public bool IsReadOnly => false;
    public int Count => _Contents.Count;
    public abstract int TotalItems {
        get;
    }

    public abstract bool CanAddSingleItem( Item item );
    public virtual bool CanAdd( T entry )
    {
        return true;
    }

    public abstract bool ContainsAny( Item item );
    public bool Contains( T entry )
    {
        return _Contents.Contains( entry );
    }

    public abstract void AddSingleItem( Item item );
    public void Add( T entry )
    {
        if ( CanAdd( entry ) )
            _Contents.Add( entry );
        else
            throw new UnityException();
    }
    public T[] Add( T[] entries )
    {
        List<T> failed = new List<T>();

        foreach ( T item in entries )
        {
            try {
                Add( item );
            } catch {
                failed.Add( item );
            }
        }

        return failed.ToArray();
    }
    public int Add( Item item, int count )
    {
        int target = count;

        while ( count > 0 )
        {
            try {
                AddSingleItem( item );
                count--;
            }
            catch {
                return target - count;
            }
        }
        
        return 0;
    }
    public Item[] Add( Item[] items )
    {
        List<Item> failed = new List<Item>();
        
        foreach ( Item item in items )
        {
            try {
                AddSingleItem( item );
            } catch {
                failed.Add( item );
            }
        }
        
        return failed.ToArray();
    }
    
    public abstract void RemoveSingleItem( Item item );
    public bool Remove( T entry )
    {
        return _Contents.Remove( entry );
    }
    public void Remove( T[] entries )
    {
        foreach ( T entry in entries )
        {
            Remove( entry );
        }
    }
    public void Remove( Item item, int count )
    {
        while ( count > 0 )
        {
            RemoveSingleItem( item );
        }
    }
    public void Remove( Item[] items )
    {
        foreach ( Item item in items )
        {
            RemoveSingleItem( item );
        }
    }

    public void Clear()
    {
        _Contents.Clear();
    }

    public void CopyTo( T[] array, int arrayIndex )
    {
        _Contents.CopyTo( array, arrayIndex ); 
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _Contents.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _Contents.GetEnumerator();
    }

    protected virtual void Awake() {}
}