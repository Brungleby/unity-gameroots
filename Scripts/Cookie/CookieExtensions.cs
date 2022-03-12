using UnityEngine;

public static class CookieExtensions
{
    private static Cookie GetCookieByNameInList( Cookie[] list, string name )
    {
        foreach ( var i in list )
        {
            if ( i.Name == name )
                return i;
        }

        return null;
    }

    public static Cookie GetCookieByName( this Component c, string name ) {
        return GetCookieByNameInList( c.GetComponents< Cookie >(), name );
    }
    public static Cookie GetCookieByNameInChildren( this Component c, string name ) {
        return GetCookieByNameInList( c.GetComponentsInChildren< Cookie >(), name );
    }
    public static Cookie GetCookieByNameInParent( this Component c, string name ) {
        return GetCookieByNameInList( c.GetComponentsInParent< Cookie >(), name );
    }
}