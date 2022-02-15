using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class that stores an item reference.
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
