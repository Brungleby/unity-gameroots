using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractSensor_Raycast : InteractSensor
{
    public LayerMask InteractionLayers;
    public float MaxDistance = 2.0f;
}
