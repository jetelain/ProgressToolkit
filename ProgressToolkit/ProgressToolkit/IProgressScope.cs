namespace ProgressToolkit
{
    public interface IProgressScope : IProgressBase
    {
        CancellationToken CancellationToken { get; }

        IProgressInteger CreateInteger(string name, int total);

        IProgressLong CreateLong(string name, long total);

        IProgressBase CreateSingle(string name);

        IProgressPercent CreatePercent(string name);

        IProgressScope CreateScope(string name, int estimatedChildrenCount = 0);
    }
}
