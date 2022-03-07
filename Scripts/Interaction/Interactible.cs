using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// When an Interactor with a matching Action Interacts with this Interactible, this Interactible's script will trigger.
/// </summary>
public class Interactible : MonoBehaviour
{
    public string[] AvailableActions = new string[1] { "Default" };

    public virtual bool CheckUser( Interactor user )
    {
        return IsActionAvailable( user );
    }

    public virtual void InteractWith( Interactor user )
    {
        
    }

    public void ReceiveInteraction( Interactor user )
    {
        if ( CheckUser( user ) && IsActionAvailable( user ) )
            InteractWith( user );
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