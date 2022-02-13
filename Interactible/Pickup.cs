using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactible
{
    public override string Tooltip( InteractionData data = null )
    {
        return Item.Name;
    }

    protected override bool CheckData( InteractionData data = null )
    {
        return true;
    }

    protected override void Interact( InteractionData data = null )
    {
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

        if ( Item )
        {
            gameObject.name = Item.name + " (Pickup)";

            if ( Item.Prefab.GetComponentInChildren<MeshRenderer>() )
            {
                PreviewObject.GetComponent<MeshFilter>().mesh = Item.Prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
                PreviewObject.GetComponent<MeshRenderer>().materials = Item.Prefab.GetComponentInChildren<MeshRenderer>().sharedMaterials;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        Swap( Item );
    }

    public void Swap( Item item )
    {
        Item = item;
        
        if ( item )
        {
            gameObject.name = Item.name + " (Pickup)";
            Destroy( PreviewObject );
            PreviewObject = Instantiate( Item.Prefab, Socket );
        }
        else
        {
            Destroy( gameObject );
        }
    }

}