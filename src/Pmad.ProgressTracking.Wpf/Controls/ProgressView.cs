using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pmad.ProgressTracking.Wpf.Controls
{
    public class ProgressView : Control
    {
        static ProgressView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressView), new FrameworkPropertyMetadata(typeof(ProgressView)));
        }

        public WpfProgressRender? ProgressRender
        {
            get { return (WpfProgressRender?)GetValue(ProgressRenderProperty); }
            set { SetValue(ProgressRenderProperty, value); }
        }

        public static readonly DependencyProperty ProgressRenderProperty =
            DependencyProperty.Register("ProgressRender", typeof(WpfProgressRender), typeof(ProgressView), new PropertyMetadata(null, ProgressRenderAttached));

        private static void ProgressRenderAttached(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressView)d).ProgressItems = (e.NewValue as WpfProgressRender)?.RootItem.Children;
        }

        public IEnumerable<ProgressItemViewModel>? ProgressItems
        {
            get { return (IEnumerable<ProgressItemViewModel>?)GetValue(ProgressItemsProperty); }
            set { SetValue(ProgressItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProgressItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressItemsProperty =
            DependencyProperty.Register("ProgressItems", typeof(IEnumerable<ProgressItemViewModel>), typeof(ProgressView), new PropertyMetadata(null));

    }
}
