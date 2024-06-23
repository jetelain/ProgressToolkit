using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pmad.ProgressTracking;
using Pmad.ProgressTracking.Wpf;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WpfProgressRender progress;

        public MainWindow()
        {
            InitializeComponent();

            progress = new WpfProgressRender();
            ProgressView.ProgressRender = progress;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var t1 = Task.Run(() => DoAction(progress, "Action1"));
            var t2 = Task.Run(() => DoAction(progress, "Action2"));
            var t3 = Task.Run(() => DoAction(progress, "Action3"));
            var t4 = Task.Run(() => DoAction(progress, "Action4"));
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