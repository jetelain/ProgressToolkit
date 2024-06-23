namespace Pmad.ProgressTracking
{
    public interface IProgressLong : IProgressBase, IProgressIncrement, IProgress<long>, IDisposable
    {

    }
}