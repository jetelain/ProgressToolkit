using System.Collections.Concurrent;
using System.Windows;
using System.Windows.Threading;

namespace Pmad.ProgressTracking.Wpf
{
    public class WpfProgressRender : ProgressRenderBase, IDisposable
    {
        private readonly ConcurrentDictionary<int, ProgressItemViewModel> table = new ConcurrentDictionary<int, ProgressItemViewModel>();
        protected readonly Dispatcher dispatcher;
        private readonly DispatcherTimer timer;

        public WpfProgressRender(CancellationToken cancellationToken = default)
            : this(Application.Current.Dispatcher, cancellationToken)
        {

        }

        public ProgressItemViewModel RootItem { get; }

        public WpfProgressRender(Dispatcher dispatcher, CancellationToken cancellationToken = default)
            : base(cancellationToken) 
        {
            this.dispatcher = dispatcher;
            this.RootItem = GetViewModel(Root);
            this.timer = new DispatcherTimer(TimeSpan.FromMilliseconds(250), DispatcherPriority.Normal, Update, dispatcher);
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

        public ProgressItemViewModel GetViewModel(ProgressBase item)
        {
            return table.GetOrAdd(item.Id, _ => new ProgressItemViewModel(item));
        }

        public ProgressItemViewModel? GetExistingViewModel(ProgressBase item)
        {
            if (table.TryGetValue(item.Id, out var viewModel))
            {
                return viewModel;
            }
            return null;
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
            dispatcher.BeginInvoke(() => WriteLine(GetViewModel(progressBase), message));
        }

        protected virtual void WriteLine(ProgressItemViewModel progressItemViewModel, string message)
        {

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing); 

            lock (timer)
            {
                if (timer.IsEnabled)
                {
                    timer.IsEnabled = false;
                }
            }
        }
    }
}
