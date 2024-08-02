namespace Pmad.ProgressTracking
{
    public sealed class ProgressScope : ProgressBase, IProgressScope
    {
        private readonly List<ProgressBase> children = new List<ProgressBase>();
        private readonly int estimatedChildrenCount;

        internal ProgressScope(ProgressRenderBase render, string name, CancellationToken token = default)
            : base(render, name)
        {
            CancellationToken = token;
        }

        private ProgressScope(ProgressScope parent, string name, int estimatedChildrenCount, CancellationToken token)
            : base(parent, name)
        {
            this.estimatedChildrenCount = estimatedChildrenCount;
            CancellationToken = token;
        }

        public override double PercentDone
        {
            get
            {
                if (IsDone)
                {
                    return 100.0;
                }
                if (estimatedChildrenCount > 0)
                {
                    var total = Math.Max(estimatedChildrenCount, children.Count);
                    var done = GetDone();
                    return done * 100.0 / total;
                }
                return 0.0;
            }
        }

        private int GetDone()
        {
            if (children.Count == 0)
            {
                return 0;
            }
            lock (children)
            {
                return children.Count(c => c.IsDone);
            }
        }

        public override bool IsDone => !elapsed.IsRunning;

        public override bool IsIndeterminate => estimatedChildrenCount == 0 && !IsDone;

        public IProgressInteger CreateInteger(string name, int total)
        {
            return AddLocked(new ProgressInteger(this, name, total));
        }

        public IProgressLong CreateLong(string name, long total)
        {
            return AddLocked(new ProgressLong(this, name, total));
        }

        private T AddLocked<T>(T item) where T : ProgressBase
        {
            lock (children)
            {
                children.Add(item);
            }
            render.Started(this, item);
            return item;
        }

        public IProgressPercent CreatePercent(string name)
        {
            return AddLocked(new ProgressPercent(this, name));
        }

        public IProgressScope CreateScope(string name, int estimatedChildrenCount = 0)
        {
            return AddLocked(new ProgressScope(this, name, estimatedChildrenCount, CancellationToken));
        }

        public IProgressScope CreateScope(string name, int estimatedChildrenCount, CancellationToken token)
        {
            return AddLocked(new ProgressScope(this, name, estimatedChildrenCount, CancellationToken));
        }

        public IProgressBase CreateSingle(string name)
        {
            return AddLocked(new ProgressInteger(this, name, 1));
        }

        public CancellationToken CancellationToken { get; }

        protected override void Ensure100Percent()
        {

        }

        public override IReadOnlyCollection<ProgressBase> Children
        {
            get
            {
                lock (children)
                {
                    return children.ToList();
                }
            }
        }
    }
}