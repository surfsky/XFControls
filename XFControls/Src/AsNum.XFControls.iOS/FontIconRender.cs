﻿using AsNum.XFControls;
using AsNum.XFControls.iOS;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FontIcon), typeof(FontIconRender))]
namespace AsNum.XFControls.iOS
{
    /// <summary>
    /// 图标字库渲染器
    /// </summary>
    public class FontIconRender : ViewRenderer<FontIcon, UILabel>
    {

        protected override void OnElementChanged(ElementChangedEventArgs<FontIcon> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var ctrl = new UILabel();
                this.SetNativeControl(ctrl);
                this.UpdateNativeControl();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName.Equals(FontIcon.FontFamilyProperty.PropertyName) ||
                e.PropertyName.Equals(FontIcon.FontSizeProperty.PropertyName) ||
                    e.PropertyName.Equals(FontIcon.GlyphProperty.PropertyName) ||
                    e.PropertyName.Equals(FontIcon.ColorProperty.PropertyName) ||
                    e.PropertyName.Equals("CurrentColor"))
            {

                this.UpdateNativeControl();
            }
        }


        private void UpdateNativeControl()
        {
            var lbl = this.Control;
            lbl.Font = ToUIFont(this.Element.FontFamily, (nfloat)this.Element.FontSize);
            lbl.Text = this.Element.Glyph;
            lbl.TextColor = this.Element.CurrentColor.ToUIColor();
        }

        private UIFont ToUIFont(string fontfamilary, nfloat? fontSize = null)
        {
            try
            {
                return UIFont.FromName(fontfamilary, fontSize ?? UIFont.SystemFontSize);
            }
            catch
            {
                return UIFont.PreferredBody;
            }
        }

    }
}
