using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractor
{
    Interactible GetInteractible { get; }

    string GetTooltip( Interactible interactible );
    void InteractWith( Interactible other );
}
