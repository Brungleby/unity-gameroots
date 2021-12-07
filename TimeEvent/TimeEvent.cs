using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeEvent : MonoBehaviour
{
    public enum PlayState {
        Stopped,
        Paused,
        Playing
    }

    [ Header( "Properties" ) ]
    
    [ Min( 0f ) ][ Tooltip( "Amount of time, per cycle, in seconds." ) ]
    public float Duration = 1.0f;
    [ Min( 0 ) ][ Tooltip( "Number of cycles to perform before the event stops. Set to 0 for infinite loop." ) ]
    public int Cycles = 1;
    [ Tooltip( "If enabled, this event will begin when it is spawned." ) ]
    public bool PlayOnStart = false;
    [ Tooltip( "If enabled, this event will pause after a single cycle is completed." ) ]
    public bool PauseOnCycle = false;

    [ Header( "Completion Events" ) ]

    [ Tooltip( "Event called when ALL cycles are completed." ) ]
    public UnityEvent OnCompleted;

    [ Tooltip( "Event called when ANY cycle is completed." ) ]
    public UnityEvent<int> OnCycleCompleted;

    [ Header( "Other Events" ) ]

    [ Tooltip( "Event called when the event is started." ) ]
    public UnityEvent OnStarted;

    [ Tooltip( "Event called when the event is played after being paused." ) ]
    public UnityEvent OnResumed;

    [ Tooltip( "Event called when the event is paused, and not completed." ) ]
    public UnityEvent OnPaused;

    [ Tooltip( "Event called when the event is stopped before all cycles are completed." ) ]
    public UnityEvent OnAborted;


    public bool IsPlaying {
        get {
            return _playState == PlayState.Playing;
        }
    }

    public float ElapsedTime {
        get {
            if ( IsPlaying )
                return Time.time - _whenStarted;
            return 0f;
        }
    }
    public float CycleElapsedTime {
        get {
            return ElapsedTime % Duration;
        }
    }
    public float PercentTime {
        get {
            return ElapsedTime / ( Duration * Cycles );
        }
    }
    public float CyclePercentTime {
        get {
            return CycleElapsedTime / Duration;
        }
    }
    public int CyclesElapsed {
        get {
            return Mathf.FloorToInt( ElapsedTime / Duration );
        }
    }

    private PlayState _playState = PlayState.Stopped;
    
    private float _whenStarted;
    private float _whenCycled;
    private float _whenPaused;

    void Start()
    {
        if ( PlayOnStart )
        {
            Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( IsPlaying && Time.time >= _whenCycled + Duration )
        {
            FinishCycle();
        }
    }

    public void Play()
    {
        if ( !IsPlaying )
        {
            if ( _playState == PlayState.Paused )
            {
                float pauseDuration = Time.time - _whenPaused;
                
                _whenStarted += pauseDuration;
                _whenCycled += pauseDuration;

                OnResumed.Invoke();
            }
            else // if Stopped
            {
                _whenStarted = Time.time;
                _whenCycled = _whenStarted;
            
                OnStarted.Invoke();
            }

            _playState = PlayState.Playing;
        }
    }

    public void Toggle()
    {
        if ( IsPlaying )
            Pause();
        else
            Play();
    }

    public void Pause()
    {
        if ( IsPlaying )
        {
            _playState = PlayState.Paused;

            _whenPaused = Time.time;

            OnPaused.Invoke();
        }
    }

    public void Stop()
    {
        Stop( false );
    }
    private void Stop( bool completed )
    {
        if ( _playState != PlayState.Stopped )
        {
            _playState = PlayState.Stopped;

            if ( completed )
                OnCompleted.Invoke();
            else
                OnAborted.Invoke();
        }
    }

    private void FinishCycle()
    {
        _whenCycled = Time.time;

        OnCycleCompleted.Invoke( CyclesElapsed );

        if ( Cycles > 0 && CyclesElapsed >= Cycles )
        {
            Stop( true );
        }
        else if ( PauseOnCycle )
        {
            Pause();
        }
    }

    public void PrintDebugMessage()
    {
        Debug.Log( "Timer \"" + this.name + "\" completed at " + Time.time + "." );
    }

}
