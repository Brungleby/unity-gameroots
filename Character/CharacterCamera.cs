using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class CharacterCamera : MonoBehaviour
{
    [ Tooltip( "Enabled or disable axes. Most common application is to disable X when using a First Person character." ) ] [ SerializeField ]
    private bool2 EnableAxes = new bool2( true, true );
    [ Tooltip( "If enabled, inputs will be inverted." ) ] [ SerializeField ]
    private bool2 InvertAxes = new bool2( false, false );
    [ Tooltip( "If enabled, axis limit will be put in place." ) ] [ SerializeField ]
    private bool2 LimitAxes = new bool2( false, true );

    [ Tooltip( "Angular limits for each axis. E.g. A value of 90 will give this camera a rotation limit of 180 degrees." ) ] [ SerializeField ]
    private Vector2 RotationLimits = new Vector2( 0f, 89.0f );
    [ Tooltip( "How fast the camera moves on each axis." ) ] [ SerializeField ]
    private Vector2 RotationSpeed = new Vector2( 50.0f, 50.0f );
    [ Tooltip( "Camera lag / smoothness. Higher values are faster." ) ]
    public float DampingSpeed = 60.0f;

    private Vector2 _inputRotation;
    private Vector2 _rotation;

    public Vector2 Rotation {
        get {
            return _rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Note: It may be necessary to replace instances of '0.01f' with 'Time.deltaTime'. I tried removing it because Time.deltaTime is already used in the Lerp call below and that seemed to keep consistent rotation speed when using fewer/more frames.
        //
        float yaw   = _inputRotation.x * RotationSpeed.x * Time.deltaTime * ( InvertAxes.x ? -1f : 1f );
        float pitch = _inputRotation.y * RotationSpeed.y * Time.deltaTime * (!InvertAxes.y ? -1f : 1f );

        _rotation += new Vector2( yaw, pitch );

        if ( LimitAxes.x )
            _rotation.x = Extensions.ClampAngle( _rotation.x, -RotationLimits.x, RotationLimits.x );
        else
            _rotation.x = Extensions.Mobius( _rotation.x, 0f, 360f );
        if ( LimitAxes.y )
            _rotation.y = Extensions.ClampAngle( _rotation.y, -RotationLimits.y, RotationLimits.y );
        else
            _rotation.y = Extensions.Mobius( _rotation.y, 0f, 360f );

        Quaternion target = Quaternion.Euler( EnableAxes.y ? _rotation.y : 0f, EnableAxes.x ? _rotation.x : 0f, 0f );

        transform.localRotation = Quaternion.LerpUnclamped( transform.localRotation, target, DampingSpeed * Time.deltaTime );
    }

    public void OnCameraRotate( InputAction.CallbackContext context )
    {
        _inputRotation = context.ReadValue<Vector2>();
    }
}
