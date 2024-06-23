namespace Pmad.ProgressToolkit
{
    internal sealed class ProgressLong : ProgressBase, IProgressLong
    {
        private readonly long total;
        private long done;

        internal ProgressLong(ProgressScope parent, string name, long total)
            : base(parent, name)
        {
            this.total = total;
        }

        public override double PercentDone => Interlocked.Read(ref done) * 100.0 / total;

        public override bool IsDone => done == total;

        public override bool IsIndeterminate => total == 1 && done == 0;

        public override bool IsTimeLinear => true;

        protected override void Ensure100Percent()
        {
            done = total;
        }

        public void Report(long value)
        {
            done = value;
            Updated();
        }

        public void ReportOneDone()
        {
            Interlocked.Increment(ref done);
            Updated();
        }
    }
}
