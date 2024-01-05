namespace KeyViewer.Core.Interfaces
{
    public interface ICopyable<T> where T : ICopyable<T>
    {
        T Copy();
    }
}
