using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleTest : CapsuleCharacterMovement
{
    protected override void Update()
    {
        Move( WalkInputAxis.XYtoXZ() * Time.deltaTime * MaxWalkSpeed );
    }
}
