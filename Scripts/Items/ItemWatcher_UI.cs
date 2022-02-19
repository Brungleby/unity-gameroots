using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemWatcher_UI : ItemWatcher
{
    [ SerializeField ]
    private Image IconImage;
    [ SerializeField ]
    private TextMeshProUGUI NameText;
    [ SerializeField ]
    private TextMeshProUGUI QuantityText;

    void OnValidate()
    {
        if ( Item )
            NameText.text = Item.DisplayName;
    }

    // Update is called once per frame
    void Update()
    {
        QuantityText.text = ( Container.QuantityOf( Item ) ).ToString();
    }
}
