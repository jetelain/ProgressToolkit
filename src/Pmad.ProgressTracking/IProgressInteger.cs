namespace Pmad.ProgressTracking
{
    public interface IProgressInteger : IProgressBase, IProgressIncrement, IProgress<int>, IDisposable 
    {
    }
}