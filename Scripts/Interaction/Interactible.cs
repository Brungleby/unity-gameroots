using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interactors can interact with Interactibles.
/// </summary>
public class Interactible : MonoBehaviour
{
    public string[] AvailableActions = new string[ 1 ] { "Default" };

    [ SerializeField ]
    private UnityEvent< Interaction > OnInteractSuccess;
    [ SerializeField ]
    private UnityEvent< Interaction > OnInteractFailure;

    public virtual string Tooltip {
        get {
            return "";
        }
    }

    /// <summary>
    /// This function checks whether or not the interaction can be performed at all. If it can't, don't even bother creating an Interaction.
    /// </summary>
    /// <returns>
    /// True if the Interaction can be attempted, not necessarily if it will succeed. False if the Interaction should not be attempted.
    /// </returns>
    protected virtual bool CheckInteraction( Interactor instigator )
    {
        return IsActionAvailable( instigator.Action );
    }

    /// <summary>
    /// This is the interaction that actually happens. If the interaction succeeds, return interaction.Complete(); otherwise action.Abort();
    /// </summary>
    protected virtual Interaction Interact( Interactor instigator )
    {
        return new Interaction( instigator, this, false, "Interaction not implemented." );
    }

    public Interaction ReceiveInteraction( Interactor instigator )
    {
        // When an interactor INSTIGATES this function, first check to make sure that this Interactible can perform the input actionType. If it can't, treat it as if nothing happened.
        if ( CheckInteraction( instigator ) )
        {
            // Execute the Interaction. The Interact function should validate
            Interaction interaction = Interact( instigator );

            if ( interaction.Result )
                OnInteractSuccess.Invoke( interaction );
            else
                OnInteractFailure.Invoke( interaction );

            return interaction;
        }

        return null;
    }

    public bool IsActionAvailable( string action )
    {
        foreach ( string available in AvailableActions )
        {
            if ( action == available )
                return true;
        }

        return false;
    }

    protected Interaction SuccessInteraction( Interactor i, string message = "" )
    {
        return new Interaction( i, this, true, message );
    }
    protected Interaction FailureInteraction( Interactor i, string message = "" )
    {
        return new Interaction( i, this, false, message );
    }
}
