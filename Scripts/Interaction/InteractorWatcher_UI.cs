using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractorWatcher_UI : InteractorWatcher
{
    protected override void OnInteractibleChanged( Interactible other )
    {
        GetComponent<TextMeshProUGUI>().text = Target.GetTooltip();
    }
}
