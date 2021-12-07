using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybookPlayer : MonoBehaviour
{
    public Typewriter Typewriter;
    public Playbook Playbook;
    
    private BookNode _node;

    void Awake()
    {
        Playbook.Initialize();
    }

    public void ExecuteNext()
    {
        if ( _node != null )
            _node = _node.Next;
        else
            _node = Playbook.GetBookNodeByLabel( "start" );

        Execute( _node );
    }

    public virtual void Execute( BookNode node )
    {
        node.Execute( this );
    }

    public virtual void PlayEvent( BookEvent e )
    {
        Typewriter.Print( e.Text );
    }
}
