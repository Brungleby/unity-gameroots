using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ CreateAssetMenu( fileName = "New Playbook", menuName = "Playbook", order = 100 ) ]
public class Playbook : ScriptableObject
{
    public TextAsset Source;

    private List<BookNode> _labelledNodes;

    public void Initialize()
    {
        _labelledNodes = new List<BookNode>();

        ProcessScript( Source.text );
    }

    public BookNode GetBookNodeByLabel( string label )
    {
        foreach ( BookNode node in _labelledNodes )
        {
            if ( node.Label == label )
                return node;
        }

        throw new System.Exception( "Could not find a node with label \"" + label + "\" in Playbook \"" + ToString() + "\"." );
    }

    private void ProcessScript( string script )
    {
        BookNode start = new BookNode( "start" );

        BookNode e0 = new BookEvent( "You wake up in a house." );
        BookNode e1 = new BookEvent( "There is lots of dust." );
        BookNode e2 = new BookEvent( "In front of you there is a carton of rancid milk!" );
        
        BookNode jump = new BookNode();

        start.Next = e0;
        e0.Next = e1;
        e1.Next = e2;
        e2.Next = jump;
        jump.Next = start;

        _labelledNodes.Add( start );
    }
}
