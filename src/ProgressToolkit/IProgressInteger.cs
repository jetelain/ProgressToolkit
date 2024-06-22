namespace ProgressToolkit
{
    public interface IProgressInteger : IProgressBase, IProgressIncrement, IProgress<int>, IDisposable 
    {
    }
}