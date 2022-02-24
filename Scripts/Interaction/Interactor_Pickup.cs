using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor_Pickup : Interactor
{
    [ SerializeField ]
    public Container Deposit;

    // [ SerializeField ]
    // public Animator Animator;

    // public override void ReceiveInput( InputAction.CallbackContext context )
    // {
    //     if ( context.started )
    //         Animator.SetBool( "IsInteracting", true );
    //     if ( context.canceled )
    //         Animator.SetBool( "IsInteracting", false );
    // }
}