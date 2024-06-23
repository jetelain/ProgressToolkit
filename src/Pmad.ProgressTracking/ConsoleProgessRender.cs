using System.Text;

namespace Pmad.ProgressTracking
{
    /// <summary>
    /// Display progress on a console/terminal
    /// </summary>
    public sealed class ConsoleProgessRender : ProgressRenderBase
    {
        private List<LayoutEntry> currentLayout = new List<LayoutEntry>();
        private string[] outputBuffer = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        private int currentProgressHeight;
        private int currentOutputHeight;
        private int lastLayoutHeight;

        private const int BarWidth = 20;
        private const int NameWidth = 30;
        private const int PercentWidth = 10;

        private record struct LayoutEntry(ProgressBase Progress, int Offset);

        private readonly object locker = new object();
        private readonly int offset;
        private readonly bool isUnicode;
        private readonly Timer redrawTimer;
        private bool isRedrawTimerActive;
        private bool didOutput = false;

        /// <summary>
        /// Create an instance of <see cref="ConsoleProgessRender"/>
        /// </summary>
        /// <param name="token">Cancellation token to propagate to scopes.</param>
        public ConsoleProgessRender(CancellationToken token = default)
            : base(token)
        {
            offset = Console.WindowTop;
            isUnicode = Console.OutputEncoding is UTF8Encoding || Console.OutputEncoding is UnicodeEncoding;
            redrawTimer = new Timer(_ => RedrawNow());
        }

        /// <inheritdoc />
        public override void Finished(ProgressBase progressBase)
        {
            RelayoutNow();
        }

        /// <inheritdoc />
        public override void PercentChanged(ProgressBase progressBase)
        {

        }

        /// <inheritdoc />
        public override void Started(ProgressScope progressScope, ProgressBase item)
        {
            RelayoutNow();
        }

        /// <inheritdoc />
        public override void TextChanged(ProgressBase progressBase)
        {
            RedrawNow();
        }

        /// <inheritdoc />
        public override void WriteLine(ProgressBase progressBase, string message)
        {
            didOutput = true;
            lock (outputBuffer)
            {
                for (int i = outputBuffer.Length - 1; i > 0; i--)
                {
                    outputBuffer[i] = outputBuffer[i - 1];
                }
                outputBuffer[0] = message.ReplaceLineEndings("");
            }
            DrawOutputNow();
        }

        private void RelayoutNow()
        {
            lock (locker)
            {
                SetTimerActive(ComputeLayout());
                FullPrint(GetMaxWidth());
            }
        }

        private void RedrawNow()
        {
            lock (locker)
            {
                try
                {
                    DrawPercentsOnly(GetMaxWidth());
                }
                catch(ArgumentOutOfRangeException)
                {
                    RelayoutNow();
                }
            }
        }

        private void DrawOutputNow()
        {
            lock (locker)
            {
                try
                {
                    DrawOutput(GetMaxWidth());
                }
                catch(ArgumentOutOfRangeException)
                {
                    RelayoutNow();
                }
            }
        }

        private void DrawOutput(int maxWidth)
        {
            if (currentOutputHeight > 0)
            {
                var top = currentProgressHeight;
                foreach (var output in outputBuffer.Reverse())
                {
                    Console.SetCursorPosition(0, top + offset);
                    WriteWidth(output, maxWidth);
                    top++;
                }
            }
        }

        private void FullPrint(int maxWidth)
        {
            DrawEntriesClean(maxWidth);
            if (didOutput)
            {
                DrawOutput(maxWidth);
            }
        }

        private void DrawEntriesClean(int maxWidth)
        {
            if (lastLayoutHeight > currentLayout.Count)
            {
                Console.Clear();
            }

            lastLayoutHeight = currentLayout.Count;

            for (int row = 0; row < currentProgressHeight; row++)
            {
                if (row < currentLayout.Count)
                {
                    DrawEntry(row, currentLayout[row], maxWidth);
                }
            }
        }

        private void DrawPercentsOnly(int maxWidth)
        {
            for (int row = 0; row < currentProgressHeight; row++)
            {
                if (row < currentLayout.Count)
                {
                    DrawEntryPercent(row, currentLayout[row], maxWidth);
                }
            }
        }

        private void DrawEntry(int row, LayoutEntry layoutEntry, int maxWidth)
        {
            DrawEntryName(row, layoutEntry);

            DrawEntryPercent(row, layoutEntry, maxWidth);
        }

        private void DrawEntryName(int row, LayoutEntry layoutEntry)
        {
            var nameOffset = layoutEntry.Offset * 2;
            Console.SetCursorPosition(0, row + offset);
            Console.Write(new string(' ', nameOffset));
            WriteWidth(layoutEntry.Progress.Name, NameWidth - nameOffset);
        }

        private void DrawEntryPercent(int row, LayoutEntry layoutEntry, int maxWidth)
        {
            Console.SetCursorPosition(NameWidth, row + offset);
            var progress = layoutEntry.Progress;
            if (progress.IsIndeterminate)
            {
                Console.Write(new string(' ', BarWidth + PercentWidth));
                DrawEntryStatusText(maxWidth, progress, maxWidth - PercentWidth - BarWidth - NameWidth);
                return;
            }
            var percent = progress.PercentDone;
            var cols = Math.Clamp((int)(percent * BarWidth / 100), 0, BarWidth);
            if (isUnicode)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(new string('▮', cols));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(new string('▮', BarWidth - cols));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(new string('#', cols));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(new string('-', BarWidth - cols));
            }
            Console.Write($"{percent:0.0} % ".PadLeft(10));

            DrawEntryStatusText(maxWidth, progress, maxWidth - PercentWidth - BarWidth - NameWidth);
        }

        private void DrawEntryStatusText(int maxWidth, ProgressBase progress, int remainWidth)
        {
            if (!string.IsNullOrEmpty(progress.Text))
            {
                WriteWidth(progress.Text, remainWidth);
            }
            else
            {
                WriteWidth(progress.GetDefaultStatusText(), remainWidth);
            }
        }

        private void WriteWidth(string name, int width)
        {
            if (name.Length > width)
            {
                if (isUnicode)
                {
                    Console.Write(name.Substring(0, width - 1));
                    Console.Write("…");
                }
                else
                {
                    Console.Write(name.Substring(0, width - 3));
                    Console.Write("...");
                }
            }
            else
            {
                Console.Write(name);
                Console.Write(new string(' ', width - name.Length));
            }
        }

        private bool ComputeLayout()
        {
            var candidates = new List<LayoutEntry>();
            var active = FindCandiates(Root.Children, candidates);
            var progressHeight = Console.WindowHeight;
            var outputHeight = 0;
            if (progressHeight > 15)
            {
                outputHeight = outputBuffer.Length;
                progressHeight -= outputHeight;
            }

            if (candidates.Count > progressHeight)
            {
                var toRemove = candidates.Where(c => c.Progress.IsDone)
                    .OrderBy(c => c.Progress.FinishedTimestamp)
                    .Take(candidates.Count - progressHeight)
                    .ToList();
                candidates.RemoveAll(toRemove.Contains);
            }

            currentProgressHeight = progressHeight;
            currentOutputHeight = outputHeight;
            currentLayout = candidates;
            return active > 0;
        }

        private void SetTimerActive(bool active)
        {
            if (active != isRedrawTimerActive)
            {
                if (active)
                {
                    redrawTimer.Change(500, 500);
                    isRedrawTimerActive = true;
                }
                else
                {
                    redrawTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    isRedrawTimerActive = false;
                }
            }
        }

        private int FindCandiates(IReadOnlyCollection<ProgressBase> children, List<LayoutEntry> candidates, int offset = 0)
        {
            int active = 0;
            foreach (var child in children)
            {
                candidates.Add(new LayoutEntry(child, offset));
                if (!child.IsDone)
                {
                    active++;
                    active += FindCandiates(child.Children, candidates, offset + 1);
                }
            }
            return active;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            currentLayout = new List<LayoutEntry>();
            redrawTimer.Dispose();

            DrawFinalReport(); // XXX: Make it optional ?
        }

        private void DrawFinalReport()
        {
            Console.Clear();
            Console.SetCursorPosition(0, offset);
            DrawReport(Root.Children, GetMaxWidth());
        }

        private static int GetMaxWidth()
        {
            return Console.WindowWidth - 1;
        }

        private void DrawReport(IReadOnlyCollection<ProgressBase> children, int maxWidth, int offset = 0)
        {
            foreach (var child in children)
            {
                DrawEntry(Console.CursorTop, new LayoutEntry(child, offset), maxWidth);
                Console.WriteLine();
                DrawReport(child.Children, maxWidth, offset + 1);
            }
        }

        /// <summary>
        /// Suspend progress drawing process, to allow an external program to output on console
        /// </summary>
        public void Suspend()
        {
            SetTimerActive(false);
        }

        /// <summary>
        /// Resume progress drawing process
        /// </summary>
        public void Resume()
        {
            if (!Root.IsDone)
            {
                RelayoutNow();
            }
        }
    }
}
