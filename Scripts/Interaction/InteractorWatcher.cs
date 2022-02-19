using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractorWatcher : MonoBehaviour
{
    protected abstract void OnInteractibleChanged( Interactible other );

    [ SerializeField ]
    private Interactor _Target;
    public Interactor Target {
        get {
            return _Target;
        }
    }

    private Interactible _LastSeenInteractible = null;

    // Update is called once per frame
    private void Update()
    {
        Interactible other = Target.CurrentInteractible;
        OnInteractibleChanged( other );

        // // For some reason, this doesn't do what it is supposed to do when destroying an item. Too annoyed to figure out the problem atm.
        // if ( _LastSeenInteractible != other )
        // {
        //     _LastSeenInteractible = other;
        //     OnInteractibleChanged( other );

        // }
    }
}
