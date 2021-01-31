using Prism.Navigation;
using Xamarin.Forms;

namespace SSCMobile.Views.Dashboard
{
    public partial class SSCMasterPage : MasterDetailPage, IMasterDetailPageOptions
    {
        public SSCMasterPage()
        {
            InitializeComponent();
        }

        public bool IsPresentedAfterNavigation { get { return false; } }
    }
}