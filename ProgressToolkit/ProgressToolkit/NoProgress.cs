namespace ProgressToolkit
{
    public sealed class NoProgress : IProgressBase, IProgressInteger, IProgressLong, IProgressPercent, IProgressScope
    {
        public CancellationToken CancellationToken => CancellationToken.None;

        public IProgressInteger CreateInteger(string name, int total)
        {
            return this;
        }

        public IProgressLong CreateLong(string name, long total)
        {
            return this;
        }

        public IProgressPercent CreatePercent(string name)
        {
            return this;
        }

        public IProgressScope CreateScope(string name, int estimatedChildrenCount = 0)
        {
            return this;
        }

        public IProgressBase CreateSingle(string name)
        {
            return this;
        }

        public void Dispose()
        {

        }

        public void Report(string value)
        {

        }

        public void Report(int value)
        {

        }

        public void Report(long value)
        {

        }

        public void Report(double value)
        {

        }

        public void ReportOneDone()
        {

        }

        public void WriteLine(string message)
        {

        }
    }
}
