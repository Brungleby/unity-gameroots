using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Basic Item", order = 101)]
public class Item_Interactor : Item, IInteractor
{
    public Interactible GetInteractible => throw new System.NotImplementedException();

    public string GetTooltip(Interactible interactible)
    {
        throw new System.NotImplementedException();
    }

    public void InteractWith(Interactible other)
    {
        throw new System.NotImplementedException();
    }
}