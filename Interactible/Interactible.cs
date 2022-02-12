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
    public virtual string Tooltip {
        get {
            return TooltipBase;
        }
    }

    public bool SingleUse = false;

    public UnityEvent<Interactor> OnInteractSuccess;
    public UnityEvent<Interactor> OnInteractFailed;

    protected virtual void OnValidate() {}
    protected virtual void Awake() {}

    /// <summary>
    /// Overridable method for determining whether or not the given Interactor can in fact Interact with this.
    /// </summary>
    protected virtual bool CheckInstigator( Interactor instigator )
    {
        return true;
    }

    /// <summary>
    /// Overridable method for what this thing does when interacting with the given instigator.
    /// </summary>
    protected virtual void Interact( Interactor instigator ) {}
    
    public void TryInteract( Interactor instigator )
    {
        if ( CheckInstigator( instigator ) )
        {
            Interact( instigator );
            OnInteractSuccess.Invoke( instigator );

            if ( SingleUse )
                Destroy( this );
        }
        else
        {
            OnInteractFailed.Invoke( instigator );
            throw new InteractionException();
        }
    }
}
