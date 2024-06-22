namespace ProgressToolkit
{
    public interface IProgressBase : IProgress<string>, IDisposable
    {
        void WriteLine(string message);
    }
}