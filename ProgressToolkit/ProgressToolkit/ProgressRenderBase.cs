namespace ProgressToolkit
{
    public abstract class ProgressRenderBase
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

        public ProgressScope Root => root;
    }
}
