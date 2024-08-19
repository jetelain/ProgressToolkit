using System.Text;

namespace Pmad.ProgressTracking
{
    public static class ConsoleProgessHelper
    {
        public static ProgressRenderBase Create(CancellationToken token = default)
        {
            if (Console.IsOutputRedirected)
            {
                return new TextProgressRender(Console.Out, token);
            }
            Console.OutputEncoding = Encoding.UTF8;
            return new ConsoleProgessRender(token);
        }

        public static ProgressRenderBase CreateWithGracefulShutdown()
        {
            var cts = new CancellationTokenSource();
            var render = Create(cts.Token);
            Console.CancelKeyPress += (s, e) =>
            {
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                    e.Cancel = true;
                    render.WriteLine("Cancellation requested, attempt graceful shutdown.");
                }
                else
                {
                    render.WriteLine("Cancellation requested again, hard shutdown.");
                }
            };
            return render;
        }

    }
}
