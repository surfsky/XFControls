using AsNum.XFControls.Binders;
using System.Windows.Input;
using Xamarin.Forms;
using System.ComponentModel;

namespace AsNum.XFControls
{
    /// <summary>
    /// 单选按钮(模拟)
    /// </summary>
    public class Radio : ContentView
    {
        //
        // Private 
        //
        private Image Icon;
        private Label Lbl;

        //
        // BindableProperty
        //
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(object), typeof(Radio));
        public static readonly BindableProperty TextAlignmentProperty = BindableProperty.Create("TextAlignment", typeof(TextAlignment), typeof(Radio), TextAlignment.Start);
        public static readonly BindableProperty ShowRadioProperty = BindableProperty.Create("ShowRadio", typeof(bool), typeof(Radio), true);
        public static readonly BindableProperty OnImgProperty = BindableProperty.Create("OnImg", typeof(ImageSource), typeof(Radio), ImageSource.FromResource("AsNum.XFControls.Imgs.Radio-Checked.png"), propertyChanged: ImgSourceChanged);
        public static readonly BindableProperty OffImgProperty = BindableProperty.Create("OffImg", typeof(ImageSource), typeof(Radio), ImageSource.FromResource("AsNum.XFControls.Imgs.Radio-Unchecked.png"), propertyChanged: ImgSourceChanged);
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(Radio), propertyChanged: TextChanged);
        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create("IsSelected", typeof(bool), typeof(Radio), false, BindingMode.TwoWay, propertyChanged: IsSelectedChanged);
        internal static readonly BindableProperty SizeProperty = BindableProperty.Create("Size", typeof(double), typeof(Radio), 25D, propertyChanged: SizePropertyChanged);

        //
        // functions
        // 
        public static void TextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var radio = (Radio)bindable;
            radio.Lbl.Text = (string)newValue;
        }
        public static void IsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var radio = (Radio)bindable;
            var source = (bool)newValue ? radio.OnImg : radio.OffImg;
            radio.Icon.Source = source;
        }
        public static void SizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var chk = (Radio)bindable;
            chk.Icon.WidthRequest = chk.Icon.HeightRequest = (double)newValue;
        }
        private static void ImgSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var chk = (Radio)bindable;
            chk.UpdateImageSource(chk.OnImg, chk.OffImg);
        }

        private void UpdateImageSource(ImageSource on, ImageSource off)
        {
            this.Icon.Source = this.IsSelected ? on : off;
        }

        //
        // property
        //
        /// <summary>
        /// 单选项的值
        /// </summary>
        public object Value
        {
            get { return this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// 标签文本
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 标签文本的对齐方式
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// 单选按钮的大小, 对标签文本无效
        /// </summary>
        internal double Size
        {
            get { return (double)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// 是否显示按钮图标(用于RadioButton)
        /// </summary>
        public bool ShowRadio
        {
            get { return (bool)this.GetValue(ShowRadioProperty); }
            set { this.SetValue(ShowRadioProperty, value); }
        }

        /// <summary>
        /// 选中状态图像
        /// </summary>
        public ImageSource OnImg
        {
            get { return (ImageSource)this.GetValue(OnImgProperty); }
            set { this.SetValue(OnImgProperty, value); }
        }

        /// <summary>
        /// 未选状态图像
        /// </summary>
        public ImageSource OffImg
        {
            get { return (ImageSource)this.GetValue(OffImgProperty); }
            set { this.SetValue(OffImgProperty, value); }
        }





        // constructor
        public Radio()
        {
            // layout
            //var layout = new StackLayout() {
            //    Orientation = StackOrientation.Horizontal
            //};
            var layout = new Grid()
            {
                ColumnDefinitions = new ColumnDefinitionCollection(),
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.StartAndExpand
            };
            layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            layout.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            this.Content = layout;

            // image
            this.Icon = new Image()
            {
                Source = this.OffImg,
                WidthRequest = this.Size,
                HeightRequest = this.Size
            };
            this.Icon.SetBinding(Image.IsVisibleProperty, new Binding("ShowRadio", source: this));
            layout.Children.Add(this.Icon);
            Grid.SetColumn(this.Icon, 0);

            // label
            this.Lbl = new Label()
            {
                VerticalTextAlignment = TextAlignment.Center,
                //HorizontalOptions = LayoutOptions.Fill,
                //HorizontalTextAlignment = TextAlignment.Center,
                //BackgroundColor = Color.Yellow
            };
            this.Lbl.SetBinding(Label.HorizontalTextAlignmentProperty, new Binding("TextAlignment", source: this));
            this.Lbl.Text = this.Text;
            layout.Children.Add(this.Lbl);
            Grid.SetColumn(this.Lbl, 1);
        }

        //
        internal void SetTap(ICommand cmd)
        {
            TapBinder.SetCmd(this.Content, cmd);
            TapBinder.SetParam(this.Content, this);
        }
    }
}
