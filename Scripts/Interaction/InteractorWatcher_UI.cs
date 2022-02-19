using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractorWatcher_UI : InteractorFeedback
{
    protected override void OnInteractibleChanged( Interactible other )
    {
        GetComponent<TextMeshProUGUI>().text = Target.GetTooltip();
    }
}
