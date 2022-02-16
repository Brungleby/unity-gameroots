using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for ALL containers, and sort of acts like a Collection. ContainerBases can store ANYTHING, not just Items. This can be used to track "live" items ( i.e. GameObjects or custom item components ).
/// </summary>
public abstract class ContainerBase< Type > : MonoBehaviour
{
    // public abstract ICollection< T > OrderContents( IComparer< T > method, IEqualityComparer< T > filter );

    public abstract int Count { get; }
    public abstract bool Contains( Type item );
    public abstract int QuantityOf( Type item );
    
    public abstract void Clear(); 

    protected abstract bool AddInternal( Type item );
    protected abstract bool RemoveInternal( Type item );

    public virtual bool CanAdd( Type item )
    {
        return !IsFull;
    }
    public virtual bool CanRemove( Type item )
    {
        return true;
    }

    [ SerializeField ]
    private int _Capacity = 1;
    public virtual int Capacity {
        get {
            return _Capacity;
        }
        set {
            _Capacity = value;
        }
    }

    public bool IsEmpty {
        get {
            return Count <= 0;
        }
    }

    public bool IsFull {
        get {
            return Capacity >= 0 && Count >= Capacity;
        }
    }

    public bool Add( Type item )
    {
        if ( CanAdd( item ) )
        {
            AddInternal( item );
            return true;
        }

        return false;
    }    
    public Type[] Add( Type[] list )
    {
        List< Type > failed = new List< Type >();
        foreach ( Type t in list )
        {
            bool success = Add( t );
            if ( !success )
                failed.Add( t );
        }

        return failed.ToArray();
    }

    public bool Remove( Type item )
    {
        if ( CanRemove( item ) )
        {
            RemoveInternal( item );
            return true;
        }

        return false;
    }
    public Type[] Remove( Type[] list )
    {
        List< Type > failed = new List<Type>();
        foreach ( Type t in list )
        {
            bool success = Remove( t );
            if ( !success )
                failed.Add( t );
        }

        return failed.ToArray();
    }
}
