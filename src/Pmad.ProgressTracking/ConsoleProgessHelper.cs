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
    }
}
