using System.Diagnostics;

namespace Pmad.ProgressToolkit
{
    public abstract class ProgressBase : IProgressBase
    {
        private static readonly IReadOnlyCollection<ProgressBase> NoChildren = new List<ProgressBase>();

        protected readonly Stopwatch elapsed;
        protected readonly ProgressScope? parent;
        protected readonly ProgressRenderBase render;
        private long finished = long.MaxValue;

        internal ProgressBase(ProgressScope parent, string name)
            : this(parent.render, name)
        {
            this.parent = parent;
        }

        internal ProgressBase(ProgressRenderBase render, string name)
        {
            this.render = render;
            elapsed = Stopwatch.StartNew();
            Name = name;
            Id = render.AllocateId();
        }

        public void Report(string value)
        {
            Text = value;
            render.TextChanged(this);
        }

        // called when PercentDone changed
        protected void Updated()
        {
            render.PercentChanged(this);
        }

        public void WriteLine(string message)
        {
            render.WriteLine(this, message);
        }

        protected abstract void Ensure100Percent();

        public void Dispose()
        {
            if (elapsed.IsRunning)
            {
                elapsed.Stop();
                finished = Stopwatch.GetTimestamp();
            }
            Ensure100Percent();
            if (parent != null)
            {
                render.Finished(this);
            }
        }

        public abstract double PercentDone { get; }

        public abstract bool IsDone { get; }

        public string Name { get; }

        public int Id { get; }

        public string? Text { get; private set; }

        public abstract bool IsIndeterminate { get; }

        public long ElapsedMilliseconds => elapsed.ElapsedMilliseconds;

        public long FinishedTimestamp => finished;

        public TimeSpan Elapsed => elapsed.Elapsed;

        public virtual IReadOnlyCollection<ProgressBase> Children => NoChildren;

        public virtual bool IsTimeLinear => false;

        public string GetDefaultStatusText()
        {
            if (IsDone)
            {
                var elapsedTime = Elapsed;
                if (elapsedTime.TotalHours > 1)
                {
                    return $"Done in {elapsedTime.TotalHours:0.0} hours";
                }
                if (elapsedTime.TotalMinutes > 2)
                {
                    return $"Done in {elapsedTime.TotalMinutes:0.0} min";
                }
                if (elapsedTime.TotalSeconds > 2)
                {
                    return $"Done in {elapsedTime.TotalSeconds:0} sec";
                }
                return $"Done in {elapsedTime.TotalMilliseconds:0} msec";
            }
            if (IsTimeLinear)
            {
                var percent = PercentDone;
                if (percent > 0)
                {
                    var remainTime = TimeSpan.FromMilliseconds(ElapsedMilliseconds * (100.0 - percent) / percent);
                    if (remainTime.TotalMinutes > 2)
                    {
                        return $"{Math.Ceiling(remainTime.TotalMinutes):0} min left";
                    }
                    if (remainTime.TotalSeconds > 2)
                    {
                        return $"{Math.Ceiling(remainTime.TotalSeconds):0} sec left";
                    }
                    return "Almost done";
                }
            }
            return string.Empty;
        }
    }
}
