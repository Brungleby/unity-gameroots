using UnityEngine;

public class Interactible : MonoBehaviour
{
    [ SerializeField ]
    private string _Tooltip = "Interact";

    protected virtual void OnValidate() {}
    protected virtual void Awake() {}

    public virtual string GetContextualTooltip( Interactor data )
    {
        return _Tooltip;
    }

    /// <summary>
    /// Overridable method for determining whether or not the given Interactor can in fact Interact with this.
    /// </summary>
    protected virtual bool CheckData( Interactor data )
    {
        return true;
    }

    /// <summary>
    /// Overridable method for what this thing does when interacting with the given user.
    /// </summary>
    protected virtual void Interact( Interactor data )
    {
        print( _Tooltip );
    }
    
    public bool ReceiveInteraction( Interactor data )
    {
        if ( CheckData( data ) )
        {
            Interact( data );
            return true;
        }

        return false;
    }
}
