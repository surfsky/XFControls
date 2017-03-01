using System;
using Xamarin.Forms;

namespace AsNum.XFControls
{
    /// <summary>
    /// 圆形
    /// </summary>
    public class Round : ContentView
    {
        /// <summary>
        /// 半径,默认 40
        /// </summary>
        public static readonly BindableProperty RadiusProperty = BindableProperty.Create( "Radius", typeof(double), typeof(Round), 40d );

        /// <summary>
        /// 半径,默认 40
        /// </summary>
        public double Radius
        {
            get { return (Double)base.GetValue(RadiusProperty); }
            set { base.SetValue(RadiusProperty, value); }
        }


        public Round()
        {
            this.HorizontalOptions = LayoutOptions.Center;
            this.VerticalOptions = LayoutOptions.Center;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            //设置高宽为半径的2倍
            var w = this.Radius * 2;
            return new SizeRequest(new Size(w, w));
        }
    }
}
