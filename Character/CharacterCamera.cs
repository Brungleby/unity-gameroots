using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class CharacterCamera : MonoBehaviour
{
    public bool2 InvertAxes = new bool2( false, false );
    public bool2 LimitAxes = new bool2( true, false );

    public Vector2 RotationLimits = new Vector2( 0f, 89.0f );

    public Vector2 RotationSpeed = new Vector2( 60f, 60f );

    private Vector2 _inputRotation;
    private Vector2 _rotation;

    // Update is called once per frame
    void Update()
    {
        float pitch = _inputRotation.y * RotationSpeed.x * Time.deltaTime * (!InvertAxes.x ? -1f : 1f );
        float yaw   = _inputRotation.x * RotationSpeed.y * Time.deltaTime * ( InvertAxes.y ? -1f : 1f );

        _rotation += new Vector2( pitch, yaw );

        if ( LimitAxes.x )
            _rotation.x = Extensions.ClampAngle( _rotation.x, -RotationLimits.x, RotationLimits.x );
        else
            _rotation.x = Extensions.Mobius( _rotation.x, 0f, 360f );
        if ( LimitAxes.y )
            _rotation.y = Extensions.ClampAngle( _rotation.y, -RotationLimits.y, RotationLimits.y );
        else
            _rotation.y = Extensions.Mobius( _rotation.y, 0f, 360f );

        // Euler Angles. Gimbal Lock is still an issue.
        transform.localEulerAngles = new Vector3( _rotation.x, _rotation.y, 0f );

        // print( transform.rotation.eulerAngles.x );

        // transform.eulerAngles = new Vector3(
        //     LimitAxes.x ? Extensions.ClampAngleMinMax( transform.eulerAngles.x, RotationLimits.x ) : transform.eulerAngles.x,
        //     LimitAxes.y ? Extensions.ClampAngleMinMax( transform.eulerAngles.y, RotationLimits.y ) : transform.eulerAngles.y,
        //     transform.eulerAngles.z
        // );
    }

    public void OnCameraRotate( InputAction.CallbackContext context )
    {
        Vector2 axis = context.ReadValue<Vector2>();
        _inputRotation = axis;
    }
}
