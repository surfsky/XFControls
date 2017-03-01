using Xamarin.Forms;

namespace AsNum.XFControls
{

    /// <summary>
    /// 时间选择器扩展版
    /// </summary>
    public class TimePickerEx : TimePicker
    {
        // BindableProperty
        public static readonly BindableProperty PlaceHolderColorProperty = BindableProperty.Create( "PlaceHolderColor", typeof(Color), typeof(TimePickerEx), Color.Default );
        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create( "PlaceHolder", typeof(string), typeof(TimePickerEx), null );
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create( "FontSize", typeof(double), typeof(TimePickerEx), 15D);
        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create( "HorizontalTextAlignment", typeof(TextAlignment), typeof(TimePickerEx), TextAlignment.Start, BindingMode.OneWay );

        /// <summary>
        /// 占位文本颜色
        /// </summary>
        public Color PlaceHolderColor
        {
            get { return (Color)this.GetValue(PlaceHolderColorProperty); }
            set { this.SetValue(PlaceHolderColorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string PlaceHolder
        {
            get { return (string)this.GetValue(PlaceHolderProperty); }
            set { this.SetValue(PlaceHolderProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, (object)value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public TextAlignment HorizontalTextAlignment
        {
            get { return (TextAlignment)this.GetValue(HorizontalTextAlignmentProperty); }
            set { this.SetValue(HorizontalTextAlignmentProperty, value); }
        }
    }
}
