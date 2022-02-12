using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component's data will be sent to Interactibles that the Interactor interacts with.
/// </summary>
[ RequireComponent( typeof( Interactor ) ) ]
public abstract class InteractionData : MonoBehaviour
{

}