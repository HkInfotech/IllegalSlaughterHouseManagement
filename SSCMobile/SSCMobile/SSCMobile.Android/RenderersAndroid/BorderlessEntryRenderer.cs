using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using SSCMobile.Droid.RenderersAndroid;
using SSCMobile.Renderers;
using SSCMobile.CustomControl;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(BorderlessBorderlessEntryRenderer))]
namespace SSCMobile.Droid.RenderersAndroid
{
    public class BorderlessBorderlessEntryRenderer : EntryRenderer
    {
        public BorderlessBorderlessEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                Control.Background = null;
            }
        }
    }
}