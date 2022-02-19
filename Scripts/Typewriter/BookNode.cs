
// The base class for any "line of code" in a Playbook. Each one has a node that will execute next.
// Giving one a label will allow us to jump there anytime later.
public class BookNode
{
    public string Label {
        get {
            return _label;
        }
    } private string _label;

    public BookNode Next;

    public BookNode( string label )
    {
        _label = label;
    }
    public BookNode()
    {
        _label = null;
    }

    public virtual void Execute( PlaybookPlayer player )
    {
        player.Execute( Next );
    }
}
