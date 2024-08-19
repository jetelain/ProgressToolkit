using System.Diagnostics;

namespace Pmad.ProgressTracking.Test
{
    public class ProgressBaseTest
    {
        private class ProgressMock : ProgressBase
        {
            public bool IsDonePublic;
            public double PercentDonePublic;
            public bool IsIndeterminatePublic;
            public bool IsTimeLinearPublic;

            public ProgressMock(string name)
                : base(new ProgressRenderMock(), name)
            {
            }

            public override double PercentDone => PercentDonePublic;

            public override bool IsDone => IsDonePublic;

            public override bool IsIndeterminate => IsIndeterminatePublic;

            public override bool IsTimeLinear => IsTimeLinearPublic;

            protected override void Ensure100Percent()
            {
                PercentDonePublic = 100;
            }

            public void SetElapsed(long msec)
            {
                typeof(Stopwatch).GetField("_elapsed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.SetValue(elapsed, msec * Stopwatch.Frequency / 1000);
            }

            public void StopSW()
            {
                elapsed.Stop();
            }

            public void StartSW()
            {
                elapsed.Start();
            }
        }

        [Fact]
        public void GetDefaultStatusText_Children()
        {
            Assert.Empty(new ProgressMock("Name").Children);
        }

        [Fact]
        public void GetDefaultStatusText_Id()
        {
            Assert.Equal(2, new ProgressMock("Name").Id);
        }

        [Fact]
        public void GetDefaultStatusText_ReportString()
        {
            var step = new ProgressMock("Name");
            Assert.Null(step.Text);
            step.Report("Hello");
            Assert.Equal("Hello", step.Text);
        }

        [Fact]
        public void GetDefaultStatusText_FinishedTimestamp()
        {
            var step = new ProgressMock("Name");
            Assert.Equal(long.MaxValue, step.FinishedTimestamp);
            step.Dispose();
            Assert.NotEqual(long.MaxValue, step.FinishedTimestamp);
        }

        [Fact]
        public void GetDefaultStatusText_Running_Linear()
        {
            var step = new ProgressMock("Name");
            step.StopSW();

            step.IsDonePublic = false;
            step.IsTimeLinearPublic = true;

            Assert.Equal(string.Empty, step.GetDefaultStatusText());

            step.PercentDonePublic = 50;

            step.SetElapsed(100);
            Assert.Equal("Almost done", step.GetDefaultStatusText());

            step.SetElapsed(1_000);
            Assert.Equal("Almost done", step.GetDefaultStatusText());

            step.SetElapsed(10_000);
            Assert.Equal("10 sec left", step.GetDefaultStatusText());

            step.SetElapsed(100_000);
            Assert.Equal("100 sec left", step.GetDefaultStatusText());

            step.SetElapsed(1_000_000);
            Assert.Equal("17 min left", step.GetDefaultStatusText());

            step.PercentDonePublic = 100;
            Assert.Equal("Almost done", step.GetDefaultStatusText());

            step.PercentDonePublic = 110;
            Assert.Equal("Almost done", step.GetDefaultStatusText());
        }

        [Fact]
        public void GetDefaultStatusText_Running_NonLinear()
        {
            var step = new ProgressMock("Name");
            step.StopSW();

            step.IsDonePublic = false;
            step.IsTimeLinearPublic = false;

            Assert.Equal(string.Empty, step.GetDefaultStatusText());

            step.PercentDonePublic = 50;

            step.SetElapsed(100);
            Assert.Equal(string.Empty, step.GetDefaultStatusText());
        }

        [Fact]
        public void GetDefaultStatusText_Done()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var step = new ProgressMock("Name");
            step.StopSW();

            step.IsDonePublic = true;
            step.PercentDonePublic = 100;

            step.SetElapsed(0);
            Assert.Equal("Done in 0 msec", step.GetDefaultStatusText());

            step.SetElapsed(100);
            Assert.Equal("Done in 100 msec", step.GetDefaultStatusText());

            step.SetElapsed(1_000);
            Assert.Equal("Done in 1000 msec", step.GetDefaultStatusText());

            step.SetElapsed(10_000);
            Assert.Equal("Done in 10 sec", step.GetDefaultStatusText());

            step.SetElapsed(100_000);
            Assert.Equal("Done in 100 sec", step.GetDefaultStatusText());

            step.SetElapsed(1_000_000);
            Assert.Equal("Done in 16.7 min", step.GetDefaultStatusText());

            step.SetElapsed(10_000_000);
            Assert.Equal("Done in 2.8 hours", step.GetDefaultStatusText());
        }

        [Fact]
        public void GetDefaultStatusText_Failed()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            var step = new ProgressMock("Name");
            step.StopSW();
            step.Failed(new Exception("FAILED"));
            step.IsDonePublic = true;
            step.PercentDonePublic = 100;

            step.SetElapsed(0);
            Assert.Equal("Failed after 0 msec", step.GetDefaultStatusText());

            step.SetElapsed(100);
            Assert.Equal("Failed after 100 msec", step.GetDefaultStatusText());

            step.SetElapsed(1_000);
            Assert.Equal("Failed after 1000 msec", step.GetDefaultStatusText());

            step.SetElapsed(10_000);
            Assert.Equal("Failed after 10 sec", step.GetDefaultStatusText());

            step.SetElapsed(100_000);
            Assert.Equal("Failed after 100 sec", step.GetDefaultStatusText());

            step.SetElapsed(1_000_000);
            Assert.Equal("Failed after 16.7 min", step.GetDefaultStatusText());

            step.SetElapsed(10_000_000);
            Assert.Equal("Failed after 2.8 hours", step.GetDefaultStatusText());
        }
    }
}
