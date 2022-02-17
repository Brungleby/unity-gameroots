using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class that stores an item reference. Can also be used as a single-slot Container in a pinch.
/// </summary>
public class ItemFilter : MonoBehaviour
{
    [ SerializeField ]
    private Item _Item;
    public virtual Item Item {
        get {
            return _Item;
        }
        set {
            _Item = value;
        }
    }
}
