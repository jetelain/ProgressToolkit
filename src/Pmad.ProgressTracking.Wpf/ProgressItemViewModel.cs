﻿using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Pmad.ProgressTracking.Wpf
{
    public sealed class ProgressItemViewModel : INotifyPropertyChanged
    {
        private readonly ProgressBase item;
        private double percentDone;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<ProgressItemViewModel> Children { get; } = new ObservableCollection<ProgressItemViewModel>();

        public ProgressItemViewModel? Parent { get; private set; }

        internal ProgressItemViewModel(ProgressBase item)
        {
            this.item = item;
        }

        public string Name => item.Name;

        public bool IsDone => item.IsDone;

        public bool IsRunning => !item.IsDone;

        public bool IsIndeterminate => item.IsIndeterminate;

        public bool IsFailed => item.Error != null;

        public bool IsSuccess => !IsFailed && IsDone;

        public ProgressBase Progress => item;

        public double PercentDone 
        {
            get { return percentDone; }
            set
            {
                if (percentDone != value)
                {
                    percentDone = value;
                    NotifyPropertyChanged(nameof(PercentDone));
                }
            }
        }

        public string Status => item.Text ?? item.GetDefaultStatusText();

        internal void AddChild(ProgressItemViewModel child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        internal void Finished()
        {
            PercentDone = 100.0;
            NotifyPropertyChanged(nameof(IsDone));
            NotifyPropertyChanged(nameof(IsRunning));
            NotifyPropertyChanged(nameof(Status));
            NotifyPropertyChanged(nameof(IsIndeterminate));
            if (IsFailed)
            {
                NotifyPropertyChanged(nameof(IsFailed));
            }
            else
            {
                NotifyPropertyChanged(nameof(IsSuccess));
            }
            Parent?.UpdatePercent();
        }

        internal void UpdatePercent()
        {
            PercentDone = item.PercentDone;
            if (string.IsNullOrEmpty(item.Text) && item.IsTimeLinear)
            {
                NotifyPropertyChanged(nameof(Status));
            }
        }

        internal void TextChanged()
        {
            NotifyPropertyChanged(nameof(Status));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}