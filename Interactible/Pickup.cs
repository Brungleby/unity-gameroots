using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactible
{
    public override string Tooltip => base.Tooltip + " " + Item.Name;

    protected override bool CheckInstigator( Interactor instigator )
    {
        return instigator.PickupDeposit.Item == null;
    }

    protected override void Interact( Interactor instigator )
    {
        instigator.PickupDeposit.Item = Item;
        Destroy( gameObject );
    }

    [ Tooltip( "Item added to inventory when this GameObject is picked up." ) ]
    public Item Item;
    [ Tooltip( "Transform at which to attach the Item's prefab." ) ]
    public Transform Socket;
    public GameObject PreviewObject;

    protected override void OnValidate()
    {
        base.OnValidate();

        if ( Item.Prefab.GetComponent<MeshRenderer>() )
        {
            PreviewObject.GetComponent<MeshFilter>().mesh = Item.Prefab.GetComponent<MeshFilter>().sharedMesh;
            PreviewObject.GetComponent<MeshRenderer>().materials = Item.Prefab.GetComponent<MeshRenderer>().sharedMaterials;
        }
    }

}