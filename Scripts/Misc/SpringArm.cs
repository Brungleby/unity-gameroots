using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    public LayerMask CollisionLayers;
    public float ProbeSize = 0.12f;
    public float MaxLength = 3.0f;

    [ Space( 5 ) ]

    public bool SmoothMove = true;
    public float MoveSmoothTime = 0.1f;
    
    [ Space( 5 ) ]
    
    public bool SmoothZoomOut = true;
    public bool SmoothZoomIn = false;
    public float ZoomSmoothTime = 0.1f;

    private Transform _Parent;
    private Vector3 _direction;

    private Vector3 _currentLagPosition;
    private Vector3 _currentLagVelocity;

    private float _currentLength;
    private float _zoomVelocity;

    protected Vector3 Origin {
        get {
            return _Parent.position;
        }
    }
    protected Vector3 Direction {
        get {
            return -_Parent.forward;
        }
    }
    public Vector3 MaxPosition {
        get {
            return Origin + Direction * MaxLength;
        }
    }

    protected virtual void OnValidate()
    {
        _Parent = transform.parent;

        _currentLagPosition = Origin;
        _currentLength = MaxLength;
        
        transform.position = MaxPosition;
    }

    void Awake()
    {
        // transform.parent = null;
    }

    protected virtual void Update()
    {
        RaycastHit hit;
        bool isBlocked = Physics.SphereCast(
            Origin, ProbeSize,
            Direction, out hit, MaxLength,
            CollisionLayers, QueryTriggerInteraction.Ignore
        );

        float targetLength;
        if ( isBlocked )
            targetLength = hit.distance;
        else
            targetLength = MaxLength;

        _currentLength = Mathf.SmoothDamp( _currentLength, targetLength, ref _zoomVelocity, ZoomSmoothTime );
        if ( !SmoothZoomIn )
            _currentLength = Mathf.Min( _currentLength, targetLength );
        if ( !SmoothZoomOut )
            _currentLength = Mathf.Max( _currentLength, targetLength );

        if ( SmoothMove )
            _currentLagPosition = Vector3.SmoothDamp( _currentLagPosition, Origin, ref _currentLagVelocity, MoveSmoothTime );
        else
            _currentLagPosition = transform.parent.position;

        transform.position = _currentLagPosition + Direction * _currentLength;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine( Origin, MaxPosition );
    }
}
