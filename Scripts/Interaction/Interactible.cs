using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactors can interact with Interactibles.
/// </summary>
public class Interactible : MonoBehaviour
{
    protected virtual bool CheckUser( Interactor user )
    {
        return true;
    }

    public virtual void Interact( Interactor user )
    {

    }

    public void ReceiveInteraction( Interactor user )
    {
        if ( CheckUser ( user ) )
            Interact( user );
    }
}