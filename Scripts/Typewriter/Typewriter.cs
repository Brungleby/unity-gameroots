using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using TMPro;

public class Typewriter : MonoBehaviour
{
    public UnityEvent OnFinishedPrint;

    public TMP_Text TMPObject;
    public AudioSource Audio;
    public SoundPoolPlayer SoundPoolPlayer;

    [ Tooltip( "How many characters to print out per second." ) ]
    public float PrintSpeed = 20.0f;
    [ Tooltip( "The audio clip that plays at the end of typewriting." ) ]
    public AudioClip SoundOnFinish;
    [ Tooltip( "How many sounds to play from the sound pool per second." ) ]
    public float SoundTriggerSpeed = 5.0f;

    private int CharIndex {
        get {
            return Mathf.FloorToInt( _cursorIndex );
        }
    }
    private bool AudioEnabled {
        get {
            return Audio != null && Audio.enabled;
        }
    }

    private TimeEvent.PlayState _playState = TimeEvent.PlayState.Stopped;
    private float _cursorIndex = 0.0f;

    private float _soundIndex = 1.0f;

    void Awake()
    {
        if ( !TMPObject )
            TMPObject = GetComponent<TMP_Text>();

        Clear( false );
    }

    void Start()
    {
        Print( "This is in <b>bold</b>." );
    }

    void Update()
    {
        if ( _playState == TimeEvent.PlayState.Playing )
        {
            _cursorIndex += PrintSpeed * Time.deltaTime;

            TMPObject.maxVisibleCharacters = CharIndex;

            if ( _cursorIndex >= (float) TMPObject.text.Length )
            {
                Stop();
                OnFinishedPrint.Invoke();
            }
            else
            {
                _soundIndex += SoundTriggerSpeed * Time.deltaTime;

                if ( _soundIndex >= 1.0f )
                {
                    if ( AudioEnabled )
                    {
                        SoundPoolPlayer.Play();

                        _soundIndex = _soundIndex % 1.0f;
                    }
                }
            }
        }
    }

    public void Print( string s )
    {
        Clear();

        TMPObject.text = s;

        _playState = TimeEvent.PlayState.Playing;

        _cursorIndex = 0.0f;
        _soundIndex = 1.0f;
    }
    public void Print()
    {
        Print( TMPObject.text );
    }

    public void Play()
    {
        if ( _playState == TimeEvent.PlayState.Paused )
            _playState = TimeEvent.PlayState.Playing;
    }

    public void Pause()
    {
        if ( _playState == TimeEvent.PlayState.Playing )
            _playState = TimeEvent.PlayState.Paused;
    }

    public void TogglePlayPause()
    {
        if ( _playState == TimeEvent.PlayState.Playing )
            Pause();
        else if ( _playState == TimeEvent.PlayState.Paused )
            Play();
    }

    public void Stop()
    {
        _playState = TimeEvent.PlayState.Stopped;

        if ( AudioEnabled )
        {
            Audio.clip = SoundOnFinish;
            Audio.Play();
        }
    }

    void Clear( bool clearText = true )
    {
        if ( clearText )
            TMPObject.text = null;

        _cursorIndex = 0.0f;
        TMPObject.maxVisibleCharacters = 0;
    }
}
