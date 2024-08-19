namespace Pmad.ProgressTracking
{
    public sealed class TextProgressRender : ProgressRenderBase
    {
        private readonly TextWriter textWriter;
        private readonly object locker = new object();

        public TextProgressRender(TextWriter textWriter, CancellationToken token = default)
            : base(token)
        {
            this.textWriter = textWriter;
        }

        public override void Finished(ProgressBase progressBase)
        {
            lock (locker)
            {
                textWriter.WriteLine($"End [{progressBase.Id}] {progressBase.Name}, done in {progressBase.ElapsedMilliseconds} msec");
            }
        }

        public override void PercentChanged(ProgressBase progressBase)
        {

        }

        public override void Started(ProgressScope progressScope, ProgressBase progressBase)
        {
            lock (locker)
            {
                textWriter.WriteLine($"Start [{progressBase.Id}] {progressBase.Name}");
            }
        }

        public override void TextChanged(ProgressBase progressBase)
        {
            lock (locker)
            {
                textWriter.WriteLine($"Status [{progressBase.Id}] {progressBase.Text}");
            }
        }

        public override void WriteLine(ProgressBase progressBase, string message)
        {
            lock (locker)
            {
                textWriter.WriteLine($"[{progressBase.Id}] {message}");
            }
        }
    }
}
