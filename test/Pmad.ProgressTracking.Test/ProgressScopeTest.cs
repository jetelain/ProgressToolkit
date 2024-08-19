namespace Pmad.ProgressTracking.Test
{
    public class ProgressScopeTest
    {
        [Fact]
        public void CreateSingle()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressInteger>(render.Root.CreateSingle("Single"));
            Assert.Equal("Single", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Contains(item, render.Root.Children);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void CreatePercent()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressPercent>(render.Root.CreatePercent("Percent"));
            Assert.Equal("Percent", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Contains(item, render.Root.Children);

            item.Report(100.0);

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void CreateInteger()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressInteger>(render.Root.CreateInteger("Integer", 100));
            Assert.Equal("Integer", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Contains(item, render.Root.Children);

            item.Report(100);

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void CreateLong()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressLong>(render.Root.CreateLong("Long", 100L));
            Assert.Equal("Long", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Contains(item, render.Root.Children);

            item.Report(100L);

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void CreateInteger_Zero()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressInteger>(render.Root.CreateInteger("Bad", 0));
            Assert.Equal("Bad", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Contains(item, render.Root.Children);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void CreateLong_Zero()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressLong>(render.Root.CreateLong("Bad", 0));
            Assert.Equal("Bad", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Contains(item, render.Root.Children);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void CreateScope()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressScope>(render.Root.CreateScope("Scope"));
            Assert.Equal("Scope", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Equal(render.Root.CancellationToken, item.CancellationToken);
            Assert.Contains(item, render.Root.Children);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void CreateScope_Token()
        {
            var render = new ProgressRenderMock();
            var tks = new CancellationTokenSource();
            var item = Assert.IsType<ProgressScope>(render.Root.CreateScope("Scope", 0, tks.Token));
            Assert.Equal("Scope", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.Equal(tks.Token, item.CancellationToken);
            Assert.Contains(item, render.Root.Children);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void PercentDone()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressScope>(render.Root.CreateScope("Scope", 5));
            Assert.Equal("Scope", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.False(item.IsIndeterminate);
            Assert.Equal(render.Root.CancellationToken, item.CancellationToken);
            Assert.Contains(item, render.Root.Children);

            item.CreateSingle("").Dispose();
            Assert.Equal(20, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(40, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(60, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(80, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(100, item.PercentDone);
            Assert.False(item.IsDone);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void PercentDone_Token()
        {
            var render = new ProgressRenderMock();
            var tks = new CancellationTokenSource();
            var item = Assert.IsType<ProgressScope>(render.Root.CreateScope("Scope", 5, tks.Token));
            Assert.Equal("Scope", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.False(item.IsIndeterminate);
            Assert.Equal(tks.Token, item.CancellationToken);
            Assert.Contains(item, render.Root.Children);

            item.CreateSingle("").Dispose();
            Assert.Equal(20, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(40, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(60, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(80, item.PercentDone);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(100, item.PercentDone);
            Assert.False(item.IsDone);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
        }

        [Fact]
        public void PercentDone_NoEstimate_IsIndeterminate()
        {
            var render = new ProgressRenderMock();
            var item = Assert.IsType<ProgressScope>(render.Root.CreateScope("Scope", 0));
            Assert.Equal("Scope", item.Name);
            Assert.Equal(0, item.PercentDone);
            Assert.False(item.IsDone);
            Assert.True(item.IsIndeterminate);
            Assert.Equal(render.Root.CancellationToken, item.CancellationToken);
            Assert.Contains(item, render.Root.Children);

            item.CreateSingle("").Dispose();
            Assert.Equal(0, item.PercentDone);
            Assert.True(item.IsIndeterminate);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(0, item.PercentDone);
            Assert.True(item.IsIndeterminate);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(0, item.PercentDone);
            Assert.True(item.IsIndeterminate);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(0, item.PercentDone);
            Assert.True(item.IsIndeterminate);
            Assert.False(item.IsDone);

            item.CreateSingle("").Dispose();
            Assert.Equal(0, item.PercentDone);
            Assert.True(item.IsIndeterminate);
            Assert.False(item.IsDone);

            item.Dispose();

            Assert.Equal(100, item.PercentDone);
            Assert.True(item.IsDone);
            Assert.False(item.IsIndeterminate);
        }
    }
}