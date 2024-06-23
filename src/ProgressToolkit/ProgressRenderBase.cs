namespace Pmad.ProgressToolkit
{
    public abstract class ProgressRenderBase : IProgressScope
    {
        private int lastId = 0;
        private readonly ProgressScope root;

        public ProgressRenderBase(CancellationToken token = default)
        {
            root = new ProgressScope(this, string.Empty, token);
        }

        internal int AllocateId()
        {
            return Interlocked.Increment(ref lastId);
        }

        public abstract void Finished(ProgressBase progressBase);

        public abstract void TextChanged(ProgressBase progressBase);

        public abstract void Started(ProgressScope progressScope, ProgressBase item);

        public abstract void PercentChanged(ProgressBase progressBase);

        public abstract void WriteLine(ProgressBase progressBase, string message);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            root.Dispose();
        }

        public IProgressInteger CreateInteger(string name, int total)
        {
            return root.CreateInteger(name, total);
        }

        public IProgressLong CreateLong(string name, long total)
        {
            return root.CreateLong(name, total);
        }

        public IProgressBase CreateSingle(string name)
        {
            return root.CreateSingle(name);
        }

        public IProgressPercent CreatePercent(string name)
        {
            return root.CreatePercent(name);
        }

        public IProgressScope CreateScope(string name, int estimatedChildrenCount = 0)
        {
            return root.CreateScope(name, estimatedChildrenCount);
        }

        public void WriteLine(string message)
        {
            root.WriteLine(message);
        }

        public void Report(string value)
        {
            root.Report(value);
        }

        public ProgressScope Root => root;

        public CancellationToken CancellationToken => root.CancellationToken;
    }
}
