using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactible
{
    public Item Item {
        get {
            return GetComponent<ItemFilter>().Item;
        }
    }

    protected override bool CheckInteraction( Interactor instigator, string actionType )
    {
        return
            base.CheckInteraction( instigator, actionType ) &&
            instigator.GetType() == typeof( Interactor_Pickup );
    }

    protected override Interaction Interact( Interaction interaction )
    {
        Interactor_Pickup instigator = (Interactor_Pickup) interaction.Instigator;

        bool success = instigator.Deposit.Add( Item );

        if ( !success )
            return interaction.Abort( "Deposit container is full." );

        Destroy( gameObject );

        return interaction.Complete();
    }
}