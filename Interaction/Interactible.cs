using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactible : MonoBehaviour
{
    public class Interaction
    {
        public Interactor Instigator;
        public Interactible Effector;
        public string ActionType;
        public bool Result;

        public Interaction( Interactor _instigator, Interactible _effector, string _actionType, bool _result )
        {
            Instigator = _instigator;
            Effector = _effector;
            ActionType = _actionType;
            Result = _result;
        }
    }

    public string[] AvailableActions = new string[ 1 ] { "Default" };

    [ SerializeField ]
    private UnityEvent< Interaction > OnInteract;

    protected virtual void Interact( Interaction query ) {}

    protected virtual bool CheckInteraction( Interaction query )
    {
        return true;
    }

    public Interaction ReceiveInteraction( Interactor other, string actionType )
    {
        if ( IsActionAvailable( actionType ) )
        {
            Interaction inst = new Interaction( other, this, actionType, false );
            bool success = CheckInteraction( inst );

            if ( success )
            {
                inst.Result = true;
                Interact( inst );
            }

            OnInteract.Invoke( inst );

            return inst;
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
}
