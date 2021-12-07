
public class BookEvent : BookNode
{
    public string Text;

    public BookEvent( string label, string text ) : base( label )
    {
        Text = text;
    }
    public BookEvent( string text ) : base()
    {
        Text = text;
    }

    public override void Execute( PlaybookPlayer player )
    {
        player.PlayEvent( this );
    }

    public virtual void FinishExecute( PlaybookPlayer player )
    {
        player.Execute( Next );
    }
}
