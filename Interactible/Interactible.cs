using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour
{
    [ SerializeField ]
    private string TooltipBase = "Interact";
    public virtual string Tooltip {
        get {
            return TooltipBase;
        }
    }

    public bool SingleUse = false;

    public UnityEvent OnUseEvent;

    public virtual void Use()
    {
        OnUseEvent.Invoke();

        if ( SingleUse )
            Destroy( this );
    }
}
