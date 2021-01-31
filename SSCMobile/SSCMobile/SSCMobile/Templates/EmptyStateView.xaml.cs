using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSCMobile.Templates
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmptyStateView : ContentView
    {
        public EmptyStateView()
        {
            InitializeComponent();
        }
        public static readonly BindableProperty EmptyStateTitleProperty =
      BindableProperty.Create(nameof(EmptyStateTitle), typeof(string), typeof(EmptyStateView), DefaultTitle, BindingMode.TwoWay);
        public string EmptyStateTitle
        {
            get { return (string)GetValue(EmptyStateTitleProperty); }
            set { SetValue(EmptyStateTitleProperty, value); }
        }
        public static readonly BindableProperty EmptyStateSubtitleProperty =
        BindableProperty.Create(nameof(EmptyStateSubtitle), typeof(string), typeof(EmptyStateView), DefaultSubtitle, BindingMode.TwoWay);

        public string EmptyStateSubtitle
        {
            get { return (string)GetValue(EmptyStateSubtitleProperty); }
            set { SetValue(EmptyStateSubtitleProperty, value); }
        }
        public static string DefaultTitle => "No results found";
        public static string DefaultSubtitle => "";

    }
}