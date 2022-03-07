using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static GameObject[] GetChildren( this GameObject o )
    {
        GameObject[] result = new GameObject[ o.transform.childCount ];
        for ( int i = 0; i < result.Length; i++ )
        {
            result[ i ] = o.transform.GetChild( i ).gameObject;
        }

        return result;
    }
    public static Transform[] GetChildren( this Transform t )
    {
        Transform[] result = new Transform[ t.childCount ];
        for ( int i = 0; i < result.Length; i++ )
        {
            result[ i ] = t.GetChild( i );
        }

        return result;
    }

    public static bool Contains<T>( this T[] array, T item )
    {
        for ( int i = 0; i < array.Length; i++ )
        {
            if ( item.Equals( array[i] ) )
                return true;
        }

        return false;
    }

    public static string AllToString( this ICollection collection, int limit = -1 )
    {
        string result = "[ ";

        int i = 0;

        foreach ( var item in collection )
        {
            if ( limit < 0 || i < limit )
            {
                result += item.ToString();
            }

            if ( i != limit - 1 && i != collection.Count - 1 )
            {
                result += ", ";
            }
            else
            {
                result += " ";
                break;
            }

            i++;
        }

        return result + "]";
    }
    public static string AllToString<T>( this T[] array, int limit = -1 )
    {
        string result = "[ ";

        for ( int i = 0; i < array.Length; i++ )
        {
            if ( limit < 0 || i < limit )
            {
                result += array[i].ToString();
            }
            
            if ( i != limit - 1 && i != array.Length - 1 )
            {
                result += ", ";
            }
            else
            {
                result += " ";
                break;
            }
        }

        return result + "]";
    }

    public static bool Between( this int x, int inMin, int inMax )
    {
        return x >= inMin && x <= inMax;
    }
    public static int Mobius( int x, int inMin, int inMax )
    {
        return ( ( x - inMin ) % ( inMax - inMin + 1 ) ) + inMin;
    }

    public static bool Between( this float x, float inMin, float inMax )
    {
        return x >= inMin && x <= inMax;
    }
    public static float Mobius( float x, float inMin, float inMax )
    {
        return ( ( x - inMin ) % ( inMax - inMin ) ) + inMin;
    }
    public static float Convlerp( float inMin, float inMax, float outMin, float outMax, float alpha, bool clamp = false )
    {
        float inverse = Mathf.InverseLerp( inMin, inMax, alpha );
        
        if ( clamp )
            return Mathf.Lerp( outMin, outMax, inverse ); 
        else
            return Mathf.LerpUnclamped( outMin, outMax, inverse );
    }
    
    static public float ModularClamp( float val, float min, float max, float rangemin = -180f, float rangemax = 180f )
    {
        var modulus = Mathf.Abs( rangemax - rangemin );
        if ( ( val %= modulus ) < 0f ) val += modulus;
        return Mathf.Clamp( val + Mathf.Min( rangemin, rangemax ), min, max);
    }

    public static float ClampAngle( float angle, float inMin, float inMax )
    {
        while ( angle < -360f )
            angle += 360f;
        while ( angle >  360f )
            angle -= 360f;

        return Mathf.Clamp( angle, inMin, inMax );

        // return ModularClamp( angle, inMin, inMax, 0f, 360 );
    }

    public static float ClampAngleMinMax( float angle, float minMax )
    {
        if ( angle < 180f )
            return Mathf.Clamp( angle, 0f, minMax );
        else
            return Mathf.Clamp( angle, 360f - minMax, 360f );
    }

    /// <summary>
    /// Returns Vector3 which points in the direction of the positive planar axes described by input v. The input IS normalized, but the return value is NOT normalized. For instance, a in input vector of ( 0, 1, 0 ) will return ( 1, 0, 1 ).
    /// </summary>
    public static Vector3 Plane( this Vector3 v )
    {
        return Vector3.one - v.normalized;
    }
    /// <summary>
    /// Combines a Vector2 with an optional y value to create a Vector3.
    /// </summary>
    public static Vector3 XYtoXZ( this Vector2 v, float y = 0f )
    {
        return new Vector3( v.x, y, v.y );
    }
    /// <summary>
    /// Creates a Vector2 whose XY components are the input vector's XZ components.
    /// </summary>
    public static Vector2 XZtoXY( this Vector3 v )
    {
        return new Vector2( v.x, v.z );
    }

    public static Vector3 AddLength( this Vector3 v, float length )
    {
        return v.normalized * ( v.magnitude + length );
    }

    public static void DrawPoint( Vector3 position, float size, Color color, float duration )
    {
        Vector3 unit = Vector3.one * size; 

        Debug.DrawLine( position - new Vector3(  1f, 1f,  1f ) * size, position + new Vector3(  1f, 1f,  1f ) * size, color, duration );
        Debug.DrawLine( position - new Vector3( -1f, 1f,  1f ) * size, position + new Vector3( -1f, 1f,  1f ) * size, color, duration );
        Debug.DrawLine( position - new Vector3( -1f, 1f, -1f ) * size, position + new Vector3( -1f, 1f, -1f ) * size, color, duration );
        Debug.DrawLine( position - new Vector3(  1f, 1f, -1f ) * size, position + new Vector3(  1f, 1f, -1f ) * size, color, duration );

    }
    public static void DrawPoint( Vector3 position, float size, Color color )
    {
        DrawPoint( position, size, color, Time.deltaTime );
    }
    public static void DrawPoint( Vector3 position, float size )
    {
        DrawPoint( position, size, Color.white );
    }
    public static void DrawPoint( Vector3 position )
    {
        DrawPoint( position, 0.1f );
    }

    public static void DrawBox( Vector3 origin, Vector3 size, Color color, float duration )
    {
        size /= 2f;

        Vector3 a = origin + new Vector3(  size.x,  size.y, size.z );
        Vector3 b = origin + new Vector3( -size.x,  size.y, size.z );
        Vector3 c = origin + new Vector3( -size.x, -size.y, size.z );
        Vector3 d = origin + new Vector3(  size.x, -size.y, size.z );

        Debug.DrawLine( a, b, color, duration );
        Debug.DrawLine( b, c, color, duration );
        Debug.DrawLine( c, d, color, duration );
        Debug.DrawLine( d, a, color, duration );
    }
}
