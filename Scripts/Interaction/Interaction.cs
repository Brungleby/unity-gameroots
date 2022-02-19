using UnityEngine;

public class Interaction
{
    public Interactor Instigator;
    public Interactible Effector;
    public string ActionType;

    private bool _Result;
    public bool Result {
        get {
            return _Result;
        }
        private set {
            _Result = value;
        }
    }

    private string _Message;
    public string Message {
        get {
            return _Message;
        }
        private set {
            _Message = value;
        }
    }

    public Interaction( Interactor _instigator, Interactible _effector, string _actionType )
    {
        Instigator = _instigator;
        Effector = _effector;
        ActionType = _actionType;
        Result = false;
        Message = "Interaction failed (unknown).";
    }

    public Interaction Complete( string message )
    {
        Result = true;
        Message = message;
        return this;
    }
    public Interaction Complete()
    {
        return Complete( "Success" );
    }

    public Interaction Abort( string message )
    {
        Result = false;
        Message = message;
        return this;
    }
    public Interaction Abort()
    {
        return Abort( "Failure" );
    }
}