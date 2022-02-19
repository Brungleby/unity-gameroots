using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ CreateAssetMenu( fileName = "New Sound Pool", menuName = "Sound Pool", order = 100 ) ]
public class SoundPool : ScriptableObject
{
    public class SoundSplashInst
    {
        public AudioClip Clip;

        public float Volume;
        public float Pitch;

        public SoundSplashInst( AudioClip clip, float volume = 1f, float pitch = 1f )
        {
            Clip = clip;
            Volume = volume;
            Pitch = pitch;
        }
    }

    public enum Shuffle
    {
        [ Tooltip( "Selects sounds completely at random." ) ]
        Random,
        [ Tooltip( "Selects sounds at random, but guarantees the next sound to be played will be new." ) ]
        PickNew,
        [ Tooltip( "Shuffles the sound list once, then plays them in sequence. Reshuffles when all have been played." ) ]
        Shuffle,
        [ Tooltip( "Plays all sounds in order from first to last." ) ]
        Sequential,
        [ Tooltip( "Plays only the 0th sound in the pool." ) ]
        PrimaryOnly,
    }

    public void PlayOneShot( Vector3 position )
    {
        AudioSource.PlayClipAtPoint( GetRandomSound(), position, GetRandomVolume() );
    }

    [ Tooltip( "The list of sounds that can be randomly played." ) ]
    public AudioClip[] SoundClips;
    [ Tooltip( "Determines how sounds are selected. This variable doesn't change, use `ShuffleMethod` instead." ) ]
    public Shuffle DefaultShuffleMethod;

    [ Space( 10 ) ]

    [ Range( 0f, 1f ) ]
    public float MinVolume = 1.0f;
    [ Range( 0f, 1f ) ]
    public float MaxVolume = 1.0f;

    [ Space( 10 ) ]

    [ Range( 0f, 2f ) ]
    public float MinPitch = 1.0f;
    [ Range( 0f, 2f ) ]
    public float MaxPitch = 1.0f;

    public Shuffle ShuffleMethod {
        get {
            return _shuffleMethod;
        }
        set {
            if ( _shuffleMethod != value )
            {
                _shuffleMethod = value;
                
                switch ( value )
                {
                    case Shuffle.Shuffle:
                        ShufflePlayQueue();
                        break;
                    case Shuffle.Sequential:
                        ReorderPlayQueue();
                        break;
                }
            }
        }
    } private Shuffle _shuffleMethod = 0;

    private Queue<AudioClip> _playQueue;
    private AudioClip _justPlayed;

    private void Awake()
    {
        _playQueue = new Queue<AudioClip>();
        ShuffleMethod = DefaultShuffleMethod;
    }

    public SoundSplashInst DrawSound()
    {
        return new SoundSplashInst( DrawAudioClip(), GetRandomVolume(), GetRandomPitch() );
    }

    private AudioClip DrawAudioClip()
    {
        AudioClip picked;

        switch ( SoundClips.Length )
        {
            case 0:
            
                picked = null;
                break;
            
            case 1:
                
                picked = GetFirstSound();
                break;
            
            default:

                switch ( _shuffleMethod )
                {
                    case Shuffle.Random:
                        
                        picked = GetRandomSound();
                        
                        break;
                    
                    case Shuffle.PickNew:
                        
                        picked = GetRandomSound();

                        while ( picked == _justPlayed )
                            picked = GetRandomSound();
                        
                        break;
                        
                    case Shuffle.Shuffle:
                    
                        picked = GetQueuedSound();
                        
                        break;
                    
                    case Shuffle.Sequential:
                    
                        picked = GetQueuedSound();

                        _playQueue.Enqueue( picked );

                        break;
                    
                    default:
                    
                        picked = GetFirstSound();

                        break; 
                }

                break;
        }

        _justPlayed = picked;

        if ( _shuffleMethod == Shuffle.Shuffle && _playQueue.Count == 0 )
            ShufflePlayQueue();

        return picked;
    }

    public void ShufflePlayQueue()
    {
        _playQueue.Clear();

        List<AudioClip> available = new List<AudioClip>();
        available.AddRange( SoundClips );

        for ( int i = 0; i < SoundClips.Length; i++ )
        {
            AudioClip picked = available[ Random.Range( 0, available.Count ) ];

            if ( i == 0 )
            {
                while ( picked == _justPlayed )
                    picked = available[ Random.Range( 0, available.Count ) ];
            }
            else 
            {
                picked = available[ Random.Range( 0, available.Count ) ];
            }

            _playQueue.Enqueue( picked );
            available.Remove( picked );
        }
    }

    public void ReorderPlayQueue()
    {
        _playQueue.Clear();
        
        for ( int i = 0; i < SoundClips.Length; i++ )
        {
            _playQueue.Enqueue( SoundClips[ i ] );
        }
    }

    private AudioClip GetQueuedSound()
    {
        return _playQueue.Dequeue();
    }

    public AudioClip GetRandomSound()
    {
        return SoundClips[ Random.Range( 0, SoundClips.Length ) ];
    }

    public AudioClip GetFirstSound()
    {
        return SoundClips[ 0 ];
    }

    public float GetRandomVolume()
    {
        return Random.Range( MinVolume, MaxVolume );
    }

    public float GetRandomPitch()
    {
        return Random.Range( MinPitch, MaxPitch );
    }
}
