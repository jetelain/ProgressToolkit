namespace Pmad.ProgressToolkit
{
    public interface IProgressInteger : IProgressBase, IProgressIncrement, IProgress<int>, IDisposable 
    {
    }
}