﻿using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SSCMobile.iOS.RenderersiOS;
using SSCMobile.Renderers;
using SSCMobile.CustomControl;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(BorderlessBorderlessEntryRenderer))]

namespace SSCMobile.iOS.RenderersiOS
{
    public class BorderlessBorderlessEntryRenderer : EntryRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control == null) return;
            Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;

        }
    }
}