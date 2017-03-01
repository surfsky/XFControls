using AsNum.XFControls.Binders;
using AsNum.XFControls.Templates;
using System.Collections;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace AsNum.XFControls
{
    /// <summary>
    /// 单选按钮组的基类
    /// </summary>
    public abstract class RadioGroupBase : ContentView
    {
        /// <summary>
        /// 布局方向
        /// </summary>
        public enum RadioGroupOrientation
        {
            Vertical = 0,
            Horizontal = 1,
            HorizontalWrap = 2
        }


        // BindaleProperty
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create("SelectedItem", typeof(object), typeof(RadioGroupBase), null, BindingMode.TwoWay, propertyChanged: SelectedItemChanged);
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(RadioGroupBase), null, propertyChanged: ItemsSourceChanged);
        public static readonly BindableProperty DisplayPathProperty = BindableProperty.Create("DisplayPath", typeof(string), typeof(RadioGroupBase));
        private static readonly BindableProperty RadioSizeProperty = BindableProperty.Create("Size", typeof(double), typeof(RadioGroupBase), 25D);
        public static readonly BindableProperty SelectedItemControlTemplateProperty = BindableProperty.Create("SelectedItemControlTemplate", typeof(ControlTemplate), typeof(RadioButtonGroup), null);
        public static readonly BindableProperty UnSelectedItemControlTemplateProperty = BindableProperty.Create("UnSelectedItemControlTemplate", typeof(ControlTemplate), typeof(RadioButtonGroup), null);
        public static readonly BindableProperty OnImgProperty = BindableProperty.Create("OnImg", typeof(ImageSource), typeof(RadioGroupBase), ImageSource.FromResource("AsNum.XFControls.Imgs.Radio-Checked.png"));
        public static readonly BindableProperty OffImgProperty = BindableProperty.Create("OffImg", typeof(ImageSource), typeof(RadioGroupBase), ImageSource.FromResource("AsNum.XFControls.Imgs.Radio-Unchecked.png"));

        #region Property
        /// <summary>
        /// 选中的数据
        /// </summary>
        private static void SelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rg = (RadioGroupBase)bindable;
            if (!rg.IsInnerChanged)
                rg.UpdateSelected();
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
        /// 数据源
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rg = (RadioGroupBase)bindable;
            rg.Container.Children.Clear();
            rg.WrapItemsSource();
            rg.UpdateSelected();
        }

        /// <summary>
        /// 要作为标签文本显示的属性路径
        /// </summary>
        public string DisplayPath
        {
            get { return (string)this.GetValue(DisplayPathProperty); }
            set { this.SetValue(DisplayPathProperty, value); }
        }


        /// <summary>
        /// 按钮大小,默认25
        /// </summary>
        public double RadioSize
        {
            get { return (double)this.GetValue(RadioSizeProperty); }
            set { this.SetValue(RadioSizeProperty, value); }
        }

        /// <summary>
        /// 选中时的 ControlTemplate
        /// </summary>
        public ControlTemplate SelectedItemControlTemplate
        {
            get { return (ControlTemplate)this.GetValue(SelectedItemControlTemplateProperty); }
            set { this.SetValue(SelectedItemControlTemplateProperty, value); }
        }


        /// <summary>
        /// 未选中时的 ControlTemplate
        /// </summary>
        public ControlTemplate UnSelectedItemControlTemplate
        {
            get { return (ControlTemplate)this.GetValue(UnSelectedItemControlTemplateProperty); }
            set { this.SetValue(UnSelectedItemControlTemplateProperty, value); }
        }

        //
        public ImageSource OnImg
        {
            get { return (ImageSource)this.GetValue(OnImgProperty); }
            set { this.SetValue(OnImgProperty, value); }
        }


        public ImageSource OffImg
        {
            get { return (ImageSource)this.GetValue(OffImgProperty); }
            set { this.SetValue(OffImgProperty, value); }
        }
        #endregion


        // private
        private ICommand SelectedCmd { get; } /// 内部使用的选中命令
        private Radio SelectedRadio = null; /// 当前选中的 Radio
        internal Layout<View> Container { get; private set; }
        private static readonly ControlTemplate DefaultControlTemplate = new DefaultControlTemplate();

        /// <summary>
        /// 获取父容器,抽象方法
        /// </summary>
        protected abstract Layout<View> GetContainer();
        private bool IsInnerChanged = false;

        /// <summary>
        /// 
        /// </summary>
        public RadioGroupBase()
        {
            this.Container = this.GetContainer();
            this.Content = this.Container;

            this.SelectedCmd = new Command((o) => {
                if (o == null)
                    return;

                this.IsInnerChanged = true;
                var item = (Radio)o;
                if (this.SelectedRadio != null)
                {
                    this.SelectedRadio.IsSelected = false;
                    this.SelectedRadio.ControlTemplate = this.UnSelectedItemControlTemplate ?? DefaultControlTemplate;
                }

                this.SelectedItem = item.Value;
                this.SelectedRadio = item;
                this.SelectedRadio.ControlTemplate = this.SelectedItemControlTemplate ?? DefaultControlTemplate;

                item.IsSelected = true;

                this.IsInnerChanged = false;
            });

            this.WrapItemsSource();
        }

        private void WrapItemsSource()
        {
            new NotifyCollectionWrapper(this.ItemsSource,
                add: (datas, idx) => this.Add(datas, idx),
                remove: (datas, idx) => this.Remove(datas, idx),
                reset: () => this.Reset(),
                finished: () => { });
        }

        private void Add(IList datas, int idx)
        {
            var c = this.Container.Children.Count;

            foreach (var d in datas)
            {
                var v = this.GetRadio(d);
                if (idx < c)
                    this.Container.Children.Insert(idx++, v);
                else
                    this.Container.Children.Add(v);
            }
        }

        private void Remove(IList datas, int idx)
        {
            var rms = this.Container.Children.Skip(idx).Take(datas.Count);
            foreach (var rm in rms)
            {
                this.Container.Children.Remove(rm);
            }
        }

        private void Reset()
        {
            this.Container.Children.Clear();
            if (this.ItemsSource != null)
                foreach (var d in this.ItemsSource)
                {
                    var v = this.GetRadio(d);
                    this.Container.Children.Add(v);
                }
        }

        protected virtual Radio GetRadio(object data)
        {
            Radio item = null;
            if (data is Radio)
            {
                item = (Radio)data;
            }
            else
            {
                item = new Radio();
                item.Value = data;

                if (!string.IsNullOrWhiteSpace(this.DisplayPath))
                {
                    item.SetBinding(Radio.TextProperty, new Binding(this.DisplayPath, source: data));
                }
                else
                    item.Text = data.ToString();
            }

            //item.Size = this.RadioSize;
            item.SetBinding(Radio.SizeProperty, new Binding(nameof(RadioSize), source: this));
            item.SetBinding(Radio.OnImgProperty, new Binding(nameof(OnImg), source: this));
            item.SetBinding(Radio.OffImgProperty, new Binding(nameof(OffImg), source: this));

            if (this.UnSelectedItemControlTemplate != null)
            {
                item.ControlTemplate = this.UnSelectedItemControlTemplate;
            }

            //TapBinder.SetCmd(item, this.SelectedCmd);
            //TapBinder.SetParam(item, item);
            item.SetTap(this.SelectedCmd);

            return item;
        }

        private void UpdateSelected()
        {
            var item = this.SelectedItem;
            //TODO 地址引用型数据，这里会找不到相同的值的
            var radio = this.Container.Children.FirstOrDefault(r => ((Radio)r).Value.Equals(item));
            this.SelectedCmd.Execute(radio);
        }

    }
}
