using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Threading;

namespace ProgressToolkit.Wpf
{
    public sealed class ProgressViewModel : ProgressRenderBase, IDisposable
    {
        private readonly ConcurrentDictionary<int, ProgressItemViewModel> table = new ConcurrentDictionary<int, ProgressItemViewModel>();
        private readonly Dispatcher dispatcher;
        private readonly DispatcherTimer timer;

        public ProgressViewModel()
            : this(Application.Current.Dispatcher)
        {

        }

        public ProgressItemViewModel RootItem { get; }

        public ProgressViewModel(Dispatcher dispatcher) 
        {
            this.dispatcher = dispatcher;
            this.RootItem = GetViewModel(Root);
            this.timer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, Update, dispatcher);
        }

        private void Update(object? sender, EventArgs e)
        {
            foreach (var item in table.Values)
            {
                if (item.IsRunning)
                {
                    item.UpdatePercent();
                }
            }
        }

        private ProgressItemViewModel GetViewModel(ProgressBase item)
        {
            return table.GetOrAdd(item.Id, _ => new ProgressItemViewModel(item));
        }

        public override void Finished(ProgressBase progressBase)
        {
            GetViewModel(progressBase).Finished();
            UpdateTimer();
        }

        public override void PercentChanged(ProgressBase progressBase)
        {

        }

        public override void Started(ProgressScope progressScope, ProgressBase item)
        {
            var parent = GetViewModel(progressScope);
            var child = GetViewModel(item);
            dispatcher.BeginInvoke(() => parent.AddChild(child));
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            lock(timer)
            {
                var wantedState = table.Values.Any(t => t != RootItem && t.IsRunning);
                if (wantedState != timer.IsEnabled)
                {
                    timer.IsEnabled = wantedState;
                }
            }
        }

        public override void TextChanged(ProgressBase progressBase)
        {
            GetViewModel(progressBase).TextChanged();
        }

        public override void WriteLine(ProgressBase progressBase, string message)
        {
            dispatcher.BeginInvoke(() => GetViewModel(progressBase).WriteLine(message));
        }

        public void Dispose()
        {
            lock(timer)
            {
                if (timer.IsEnabled)
                {
                    timer.IsEnabled = false;
                }
            }
        }
    }
}
