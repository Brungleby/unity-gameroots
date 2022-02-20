using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : Interactible
{
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