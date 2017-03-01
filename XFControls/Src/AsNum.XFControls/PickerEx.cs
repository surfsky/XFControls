using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace AsNum.XFControls
{

    /// <summary>
    /// Picker 扩展,
    /// XF 中的Picker没有数据源,数据只能是string的集合,不方便MVVM绑定
    /// </summary>
    public class PickerEx : Picker
    {
        // BindableProperty
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable<object>), typeof(PickerEx), Enumerable.Empty<object>(), propertyChanged: ItemsSourceChanged );
        public static readonly BindableProperty DefaultIndexProperty = BindableProperty.Create("DefaultIndex", typeof(int), typeof(PickerEx), 0 );
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(PickerEx), null, propertyChanged: SelectedItemChanged);
        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create( "HorizontalTextAlignment", typeof(TextAlignment), typeof(TimePickerEx), TextAlignment.Start, BindingMode.OneWay );
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create( "FontSize", typeof(double), typeof(TimePickerEx), 15D);

        #region Property
        /// <summary>
        /// 文本大小
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, (object)value); }
        }

        /// <summary>
        /// 文本水平对齐
        /// </summary>
        public TextAlignment HorizontalTextAlignment
        {
            get { return (TextAlignment)this.GetValue(HorizontalTextAlignmentProperty); }
            set { this.SetValue(HorizontalTextAlignmentProperty, value); }
        }


        /// <summary>
        /// 数据源
        /// </summary>
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// 选中的数据
        /// </summary>
        public object SelectedItem
        {
            get { return this.GetValue(SelectedItemProperty); }
            set { this.SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// 要显示的文本的属性路径
        /// </summary>
        public string DisplayMember
        {
            get; set;
        }

        /// <summary>
        /// 默认选中项序号
        /// </summary>
        public int DefaultIndex
        {
            get { return (int)this.GetValue(DefaultIndexProperty); }
            set { this.SetValue(DefaultIndexProperty, value); }
        }
        #endregion
        
        //
        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (PickerEx)bindable;
            picker.Items.Clear();
            var datas = (IEnumerable<object>)newValue;
            if (datas != null)
            {
                foreach (var o in datas)
                {
                    var d = Helper.TryGetProperty(o, picker.DisplayMember);
                    if (d != null)
                    {
                        picker.Items.Add(d.ToString());
                    }
                }
            }
        }

        //
        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var picker = (PickerEx)bindable;
            if (newValue != null)
            {
                var v = Helper.TryGetProperty(newValue, picker.DisplayMember);
                if (v != null)
                {
                    var idx = picker.Items.IndexOf(v.ToString());
                    picker.SelectedIndex = idx;
                }
            }
        }
    }
}
