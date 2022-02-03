using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningMovement : MonoBehaviour
{
    public Vector3 PerAxisSpeed = Vector3.up;
    public float SpeedMultiplier = 1.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = PerAxisSpeed * SpeedMultiplier * Time.deltaTime;

        transform.rotation = Quaternion.Euler( transform.eulerAngles + offset );
    }
}
