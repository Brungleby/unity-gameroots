using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactible
{
    protected override bool CheckInteraction( Interactor instigator )
    {
        return
            base.CheckInteraction( instigator ) &&
            instigator.GetType() == typeof( Interactor_Pickup );
    }

    protected override Interaction Interact( Interactor instigator )
    {
        Interactor_Pickup instigatorPickup = (Interactor_Pickup) instigator;
        
        bool success = instigatorPickup.Deposit.Add( Item );

        if ( !success )
            return FailureInteraction( instigator, "Deposit container is full." );

        Destroy( gameObject );

        return SuccessInteraction( instigator );
    }

    public Transform SocketTransform;
    [ SerializeField ]
    private GameObject _PreviewObject;

    [ SerializeField ]
    private Item _Item;
    public Item Item {
        get {
            return _Item;
        }
        set {
            _Item = value;
            RefreshPrefab();
        }
    }

    protected virtual void OnValidate()
    {
        if ( Item && _PreviewObject )
        {
            _PreviewObject.GetComponent<MeshFilter>().mesh = Item.Prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
            _PreviewObject.GetComponent<MeshRenderer>().materials = Item.Prefab.GetComponentInChildren<MeshRenderer>().sharedMaterials;
            _PreviewObject.transform.localScale = Item.Prefab.GetComponentInChildren<MeshFilter>().transform.localScale;
        }
    }

    protected virtual void Awake()
    {
        RefreshPrefab();
    }

    void RefreshPrefab()
    {
        if ( _PreviewObject )
            Destroy( _PreviewObject );

        if ( Item )
            _PreviewObject = Instantiate( Item.Prefab, SocketTransform );
    }
}