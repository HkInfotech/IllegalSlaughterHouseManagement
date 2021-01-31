using SSCMobile.CustomControl;
using SSCMobile.iOS.RenderersiOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]

namespace SSCMobile.iOS.RenderersiOS
{
    public class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BackgroundColor = UIColor.Clear;
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}