using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to a GameObject. This component contains a "rich" value between 0 and a maximum amount. Great for tracking health, stamina, etc.
/// </summary>
public class Cookie : MonoBehaviour
{
    private class CookieGradualModifier
    {
        public float Delta;
        public float Duration;

        public bool IsExpired {
            get {
                return _remaining <= 0f && Duration > 0f;
            }
        }

        private float _remaining;

        public CookieGradualModifier( float delta, float duration )
        {
            Delta = delta;
            Duration = duration;
            _remaining = Duration;
        }

        public float ConsumeOnUpdate( float deltaTime )
        {
            _remaining -= deltaTime;

            if ( Duration > 0f )
                return Delta * ( deltaTime / Duration );
            else
                return Delta * deltaTime;
        }
    }

    public string Name;

    [ SerializeField ]
    private float _value;
    public float Value {
        get {
            return _value;
        }
        set {
            _value = Mathf.Clamp( value, 0f, Maximum );
        }
    }

    [ SerializeField ]
    private float _maximum;
    public float Maximum {
        get {
            return _maximum;
        }
        set {
            _maximum = value;
        }
    }

    private float _velocity;
    private float _lastVelocity;
    public float Velocity {
        get {
            return _lastVelocity;
        }
    }

    public float PersistentModifier;

    private List<CookieGradualModifier> _modifiers;


    void OnValidate()
    {
        _modifiers = new List<CookieGradualModifier>();
    }

    protected virtual void Awake()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        AddGradual( PersistentModifier );

        List<CookieGradualModifier> toRemove = new List<CookieGradualModifier>();
        foreach ( CookieGradualModifier mod in _modifiers )
        {
            AddGradual( mod.ConsumeOnUpdate( Time.deltaTime ) );

            if ( mod.IsExpired )
                toRemove.Add( mod );
        }

        foreach ( CookieGradualModifier mod in toRemove )
        {
            _modifiers.Remove( mod );
        }

        Value += _velocity * Time.deltaTime;

        _lastVelocity = _velocity;
        _velocity = 0f;
    }

    public void AddBurst( float delta )
    {
        Value += delta;
    }

    public void AddGradual( float delta )
    {
        _velocity += delta;
    }

    public void AddGradualModifier( float delta, float duration = 0f )
    {
        _modifiers.Add( new CookieGradualModifier( delta, duration ) );
    }
}
