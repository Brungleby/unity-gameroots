using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractorFeedback : MonoBehaviour
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
        if ( _LastSeenInteractible != other )
        {
            _LastSeenInteractible = other;

            OnInteractibleChanged( other );
        }
    }
}
