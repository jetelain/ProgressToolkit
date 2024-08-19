namespace Pmad.ProgressTracking.Test
{
    internal class ProgressRenderMock : ProgressRenderBase
    {
        public override void Finished(ProgressBase progressBase)
        {
        }

        public override void PercentChanged(ProgressBase progressBase)
        {
        }

        public override void Started(ProgressScope progressScope, ProgressBase item)
        {
        }

        public override void TextChanged(ProgressBase progressBase)
        {
        }

        public override void WriteLine(ProgressBase progressBase, string message)
        {
        }
    }
}
