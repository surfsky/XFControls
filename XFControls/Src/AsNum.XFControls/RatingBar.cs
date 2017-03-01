using Xamarin.Forms;

namespace AsNum.XFControls
{
    /// <summary>
    /// 打分条
    /// </summary>
    public class RatingBar : View /*UniformGrid*/
    {
        // BindabelProperty
        public static readonly BindableProperty IsIndicatorProperty = BindableProperty.Create("IsIndicator", typeof(bool), typeof(RatingBar), false);
        public static readonly BindableProperty RateProperty = BindableProperty.Create("Rate", typeof(float), typeof(RatingBar), 0f, BindingMode.TwoWay);
        public static readonly BindableProperty StarCountProperty = BindableProperty.Create("StarCount", typeof(int), typeof(RatingBar), 5);
        public static readonly BindableProperty StepProperty = BindableProperty.Create("Step", typeof(float), typeof(RatingBar), 1f);
        public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create("SelectedColor", typeof(Color), typeof(RatingBar), Color.Default);
        public static readonly BindableProperty UnSelectedColorProperty = BindableProperty.Create("UnSelectedColor", typeof(Color), typeof(RatingBar), Color.Default);

        #region Property
        /// <summary>
        /// 是否仅指示,默认 false
        /// </summary>
        public bool IsIndicator
        {
            get { return (bool)this.GetValue(IsIndicatorProperty); }
            set { this.SetValue(IsIndicatorProperty, value); }
        }


        /// <summary>
        /// 星星数量
        /// </summary>
        public int StarCount
        {
            get { return (int)this.GetValue(StarCountProperty); }
            set { this.SetValue(StarCountProperty, value); }
        }

        /// <summary>
        /// 当前分数
        /// </summary>
        public float Rate
        {
            get { return (float)this.GetValue(RateProperty); }
            set { this.SetValue(RateProperty, value); }
        }

        /// <summary>
        /// 打分步长,默认1
        /// </summary>
        public float Step
        {
            get { return (float)this.GetValue(StepProperty); }
            set { this.SetValue(StepProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color UnSelectedColor
        {
            get { return (Color)this.GetValue(UnSelectedColorProperty); }
            set { this.SetValue(UnSelectedColorProperty, value); }
        }
        #endregion
    }
}
