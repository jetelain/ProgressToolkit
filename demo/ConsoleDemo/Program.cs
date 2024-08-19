using Pmad.ProgressTracking;

namespace ConsoleDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            using (var progress = ConsoleProgessHelper.CreateWithGracefulShutdown())
            {
                progress.Root.WriteLine("This is a very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very very long message ");
                var t1 = Task.Run(() => DoAction(progress, "Action1"));
                var t2 = Task.Run(() => DoAction(progress, "Action2"));
                var t3 = Task.Run(() => DoAction(progress, "Action3"));
                var t4 = Task.Run(() => DoAction(progress, "Action4"));
                await t1;
                await t2;
                await t3;
                await t4;
            }

            Console.WriteLine("Hello, World!");
        }
        
        private static void DoAction(IProgressScope render, string name)
        {
            using var scope = render.CreateScope(name);
            
            using (var rep = scope.CreateInteger("Integer1", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(50);
                    rep.ReportOneDone();
                }
            }
            using (var rep = scope.CreateInteger("Integer2", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(50);
                    rep.ReportOneDone();
                }
            }
            using (var rep = scope.CreateInteger("Integer3", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(50);
                    rep.ReportOneDone();
                }
            }
            using (var rep = scope.CreateInteger("Integer4", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(50);
                    rep.ReportOneDone();
                }
            }
            using (var rep = scope.CreateInteger("Integer5", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(50);
                    rep.ReportOneDone();
                }
            }
            using (var rep = scope.CreateInteger("Integer6", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(50);
                    rep.ReportOneDone();
                }
            }
        }
    }
}
