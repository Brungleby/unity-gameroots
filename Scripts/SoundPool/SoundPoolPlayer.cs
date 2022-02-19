using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPoolPlayer : MonoBehaviour
{
    [ Space( 10 ) ]

    [ Tooltip( "The selected pool from which to draw sounds." ) ]
    public SoundPool Pool;

    [ Tooltip( "If specified, the sound pool will update the AssociatedSource's clip and play sounds using the Source's settings.\nIf not specified, the SoundPoolPlayer will play OneShotAudios (and pitch randomization will not work)." ) ]
    public AudioSource AssociatedSource;

    public void Play()
    {
        SoundPool.SoundSplashInst splash = Pool.DrawSound();

        if ( splash.Clip )
        {
            if ( AssociatedSource )
            {
                AssociatedSource.clip = splash.Clip;
                AssociatedSource.volume = splash.Volume;
                AssociatedSource.pitch = splash.Pitch;
                AssociatedSource.Play();
            }
            else
            {
                AudioSource.PlayClipAtPoint( splash.Clip, transform.position, splash.Volume );
            }
        }
        else
        {
            Debug.LogError( "SoundPool " + this.name + " has no sounds to choose from." );
        }
    }
}
