namespace KeyViewer.Types
{
    public interface IDrawable
    {
        DrawContext Context { get; }
        void OnChange();
    }
}
