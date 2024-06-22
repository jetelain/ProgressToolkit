using System.Text;

namespace ProgressToolkit
{
    public sealed class ConsoleProgessRender : ProgressRenderBase, IDisposable
    {
        private List<LayoutEntry> currentLayout = new List<LayoutEntry>();
        private string[] outputBuffer = new[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        private int currentProgressHeight;
        private int currentOutputHeight;

        private const int BarWidth = 20;
        private const int NameWidth = 30;
        private const int PercentWidth = 10;

        private record struct LayoutEntry(ProgressBase Progress, int Offset, int ChildIndex); // 128 bits on x64

        private readonly object locker = new object();
        private readonly bool isUnicode;
        private readonly Timer redrawTimer;
        private bool isRedrawTimerActive;
        private bool didOutput = false;

        public ConsoleProgessRender(CancellationToken token = default)
            : base(token)
        {
            isUnicode = Console.OutputEncoding is UTF8Encoding || Console.OutputEncoding is UnicodeEncoding;
            redrawTimer = new Timer(_ => RedrawNow());
        }

        public override void Finished(ProgressBase progressBase)
        {
            if (progressBase == Root)
            {
                return;
            }
            RelayoutNow();
        }

        public override void PercentChanged(ProgressBase progressBase)
        {

        }

        public override void Started(ProgressScope progressScope, ProgressBase item)
        {
            RelayoutNow();
        }

        public override void TextChanged(ProgressBase progressBase)
        {
            RedrawNow();
        }

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
                DrawPercentsOnly(GetMaxWidth());
            }
        }

        private void DrawOutputNow()
        {
            lock (locker)
            {
                DrawOutput(GetMaxWidth());
            }
        }

        private void DrawOutput(int maxWidth)
        {
            if (currentOutputHeight > 0)
            {
                var top = currentProgressHeight;
                foreach (var output in outputBuffer)
                {
                    Console.SetCursorPosition(0, top);
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
            Console.Clear();
            for (int row = 0; row < currentProgressHeight; row++)
            {
                if (row < currentLayout.Count)
                {
                    Console.SetCursorPosition(0, row);
                    DrawEntry(currentLayout[row], maxWidth);
                }
            }
        }

        private void DrawPercentsOnly(int maxWidth)
        {
            for (int row = 0; row < currentProgressHeight; row++)
            {
                if (row < currentLayout.Count)
                {
                    Console.SetCursorPosition(0, row);
                    DrawEntryPercent(currentLayout[row], maxWidth);
                }
            }
        }

        private void DrawEntry(LayoutEntry layoutEntry, int maxWidth)
        {
            DrawEntryName(layoutEntry);

            DrawEntryPercent(layoutEntry, maxWidth);
        }

        private void DrawEntryName(LayoutEntry layoutEntry)
        {
            var nameOffset = layoutEntry.Offset * 2;
            Console.CursorLeft = nameOffset;
            WriteWidth(layoutEntry.Progress.Name, NameWidth - nameOffset);
        }

        private void DrawEntryPercent(LayoutEntry layoutEntry, int maxWidth)
        {
            var progress = layoutEntry.Progress;
            if (progress.IsIndeterminate)
            {
                return;
            }
            var percent = progress.PercentDone;
            var cols = Math.Clamp((int)(percent * BarWidth / 100), 0, BarWidth);
            Console.CursorLeft = NameWidth;
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
            int indexToRemove = 0;
            while (candidates.Count > progressHeight && candidates.Count > active)
            {
                candidates.RemoveAll(e => e.ChildIndex == indexToRemove && e.Progress.IsDone);
                indexToRemove++;
            }
            currentProgressHeight = progressHeight;
            currentOutputHeight = outputHeight;
            currentLayout = candidates;
            return active > 0;
        }

        public void SetTimerActive(bool active)
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
            int num = 0;
            foreach (var child in children)
            {
                candidates.Add(new LayoutEntry(child, offset, num));
                if (!child.IsDone)
                {
                    active++;
                    active += FindCandiates(child.Children, candidates, offset + 1);
                }
                num++;
            }
            return active;
        }

        public void Dispose()
        {
            Root.Dispose();
            currentLayout = new List<LayoutEntry>();
            redrawTimer.Dispose();
            DrawFinalReport();
        }

        private void DrawFinalReport()
        {
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            DrawReport(Root.Children, GetMaxWidth());
        }

        private static int GetMaxWidth()
        {
            return Console.WindowWidth - 1;
        }

        private void DrawReport(IReadOnlyCollection<ProgressBase> children, int maxWidth, int offset = 0)
        {
            int num = 0;
            foreach (var child in children)
            {
                DrawEntry(new LayoutEntry(child, offset, num), maxWidth);
                Console.WriteLine();
                DrawReport(child.Children, maxWidth, offset + 1);
                num++;
            }
        }
    }
}
