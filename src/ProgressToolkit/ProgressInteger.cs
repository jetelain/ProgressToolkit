namespace ProgressToolkit
{
    internal sealed class ProgressInteger : ProgressBase, IProgressInteger
    {
        private readonly int total;
        private int done;

        internal ProgressInteger(ProgressScope parent, string name, int total)
            : base(parent, name)
        {
            this.total = total;
        }

        public override double PercentDone => done * 100.0 / total;

        public override bool IsDone => done == total;

        public override bool IsIndeterminate => total == 1 && done == 0;

        public override bool IsTimeLinear => true;

        protected override void Ensure100Percent()
        {
            done = total;
        }

        public void Report(int value)
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
