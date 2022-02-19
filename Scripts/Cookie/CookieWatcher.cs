using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CookieWatcher : MonoBehaviour
{
    public Cookie Reference;

    public float Alpha {
        get {
            return Reference.Value / Reference.Maximum;
        }
    }

    protected virtual void Update() {}
}