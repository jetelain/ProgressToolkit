namespace ProgressToolkit
{
    internal sealed class ProgressPercent : ProgressBase, IProgressPercent
    {
        private double done;

        public ProgressPercent(ProgressScope parent, string name)
            : base(parent, name)
        {
        }

        public override double PercentDone => done;

        public override bool IsDone => PercentDone == 100;

        public override bool IsIndeterminate => false;

        public override bool IsTimeLinear => true;

        public void Report(double value)
        {
            done = value;
            Updated();
        }

        protected override void Ensure100Percent()
        {
            done = 100;
        }
    }
}