using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour
{
    [System.Serializable]
    public class InteractionException : UnityException
    {
        public InteractionException() { }
        public InteractionException(string message) : base(message) { }
        public InteractionException(string message, System.Exception inner) : base(message, inner) { }
        protected InteractionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [ SerializeField ]
    private string TooltipBase = "Interact";

    protected virtual void OnValidate() {}
    protected virtual void Awake() {}

    public virtual string Tooltip( InteractionData data = null )
    {
        return TooltipBase;
    }

    /// <summary>
    /// Overridable method for determining whether or not the given Interactor can in fact Interact with this.
    /// </summary>
    protected virtual bool CheckData( InteractionData data = null )
    {
        return true;
    }

    /// <summary>
    /// Overridable method for what this thing does when interacting with the given user.
    /// </summary>
    protected virtual void Interact( InteractionData data = null ) {}
    
    public bool TryReceiveInteraction( InteractionData data = null )
    {
        if ( data == null )
        {
            Interact();
            return true;
        }

        if ( CheckData( data ) )
        {
            Interact( data );
            return true;
        }

        return false;
    }
}
