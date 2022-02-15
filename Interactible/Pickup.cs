using System.Collections.Generic;
using UnityEngine;

[ RequireComponent( typeof( ItemFilter ) ) ]
public class Pickup : Interactible
{
    public override string GetContextualTooltip( Interactor data = null )
    {
        return Item.Name;
    }

    protected override void Interact( Interactor data = null )
    {
        Destroy( gameObject );
    }

    // [ Tooltip( "Item added to inventory when this GameObject is picked up." ) ]
    public Item Item {
        get {
            return GetComponent<ItemFilter>().Item;
        }
        set {
            GetComponent<ItemFilter>().Item = value;
        }
    }

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