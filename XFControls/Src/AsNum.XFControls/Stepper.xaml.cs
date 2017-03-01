﻿using AsNum.XFControls.Binders;

using Xamarin.Forms;

namespace AsNum.XFControls {
    /// <summary>
    /// Stepper
    /// XF 的 Stepper 难看
    /// </summary>
    public partial class Stepper : ContentView
    {
        // BindableProperty
        public static readonly BindableProperty MinProperty = BindableProperty.Create("Min", typeof(double), typeof(Stepper), double.MinValue);
        public static readonly BindableProperty MaxProperty = BindableProperty.Create("Max", typeof(double), typeof(Stepper), double.MaxValue);
        public static readonly BindableProperty StepProperty = BindableProperty.Create("Step", typeof(double), typeof(Stepper), 1d);
        public static readonly BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(double), typeof(Stepper), 0d, BindingMode.TwoWay, propertyChanged: ValueChanged);
        public static readonly BindableProperty FormatProperty = BindableProperty.Create("Format", typeof(string), typeof(Stepper), "0", propertyChanged: FmtChanged);
        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create("BorderColor", typeof(Color), typeof(Stepper), Color.FromHex("#cccccc"), propertyChanged: ColorChanged);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create("TextColor", typeof(Color), typeof(Stepper), Color.FromHex("#333333"), propertyChanged: TextColorChanged);


        #region Property
        /// <summary>
        /// 最小值
        /// </summary>
        public double Min
        {
            get { return (double)this.GetValue(MinProperty); }
            set { this.SetValue(MinProperty, value); }
        }


        /// <summary>
        /// 最大值
        /// </summary>
        public double Max
        {
            get { return (double)this.GetValue(MaxProperty); }
            set { this.SetValue(MaxProperty, value); }
        }

        /// <summary>
        /// 步长, 默认1
        /// </summary>
        public double Step
        {
            get { return (double)this.GetValue(StepProperty); }
            set
            {
                if (value < 1)
                    value = 1;
                this.SetValue(StepProperty, value);
            }
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set {
                if (value < this.Min)
                    value = this.Min;
                if (value > this.Max)
                    value = this.Max;
                this.SetValue(ValueProperty, value);
            }
        }

        private static void ValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var stepper = (Stepper)bindable;
            stepper.Update();
        }

        /// <summary>
        /// 显示格式
        /// </summary>
        public string Format
        {
            get { return (string)this.GetValue(FormatProperty); }
            set { this.SetValue(FormatProperty, value); }
        }

        private static void FmtChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var stepper = (Stepper)bindable;
            stepper.lbl.Text = stepper.Value.ToString(stepper.Format ?? "");
        }

        /// <summary>
        /// 线框颜色
        /// </summary>
        public Color BorderColor
        {
            get { return (Color)this.GetValue(BorderColorProperty); }
            set { this.SetValue(BorderColorProperty, value); }
        }

        private static void ColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var s = (Stepper)bindable;
            s.Resources["BorderColor"] = (Color)newValue;
        }

        /// <summary>
        /// 线框颜色
        /// </summary>
        public Color TextColor
        {
            get { return (Color)this.GetValue(TextColorProperty); }
            set { this.SetValue(TextColorProperty, value); }
        }

        private static void TextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var s = (Stepper)bindable;
            s.Resources["TextColor"] = (Color)newValue;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Stepper()
        {
            InitializeComponent();

            this.Resources = new ResourceDictionary();
            this.Resources.Add("BorderColor", this.BorderColor);
            this.Resources.Add("TextColor", this.TextColor);

            TapBinder.SetCmd(this.btnIncrease, new Command(() => {
                this.Value += this.Step;
                this.Update();
            }));

            TapBinder.SetCmd(this.btnReduce, new Command(() => {
                this.Value -= this.Step;
                this.Update();
            }));
            this.Update();
        }

        private void Update()
        {
            this.lbl.Text = this.Value.ToString(this.Format ?? "");
        }
    }
}
