using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactible
{
    public override string Tooltip => base.Tooltip + " " + Item.Name;

    public override void Use()
    {
        base.Use();

        Destroy( gameObject );
    }

    [ Tooltip( "Item added to inventory when this GameObject is picked up." ) ]
    public Item Item;
}