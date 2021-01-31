using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SSCMobile.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityProgressBar : ContentView
    {
        public ActivityProgressBar()
        {
            InitializeComponent();
        }
    }
}