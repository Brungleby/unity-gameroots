using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactors can interact with Interactibles.
/// </summary>
public class Interactible : MonoBehaviour
{
    public string[] AvailableActions = new string[1] { "Default" };

    public virtual bool CheckUser( Interactor user )
    {
        return true;
    }

    public virtual void Interact( Interactor user )
    {
        
    }

    public void ReceiveInteraction( Interactor user )
    {
        if ( CheckUser( user ) && IsActionAvailable( user ) )
            Interact( user );
    }

    public bool IsActionAvailable( string action )
    {
        return AvailableActions.Contains( action );
    }

    public bool IsActionAvailable( Interactor user )
    {
        return AvailableActions.Contains( user.Action );
    }
}