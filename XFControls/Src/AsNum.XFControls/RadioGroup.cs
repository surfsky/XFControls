using Xamarin.Forms;

namespace AsNum.XFControls
{
    /// <summary>
    /// 单选按钮组
    /// </summary>
    public class RadioGroup : RadioGroupBase
    {
        // BindableProperty
        public static readonly BindableProperty OrientationProperty = BindableProperty.Create("Orientation", typeof(StackOrientation), typeof(RadioGroup), StackOrientation.Horizontal, propertyChanged: OrientationChanged);


        /// <summary>
        /// 方向
        /// </summary>
        public StackOrientation Orientation
        {
            get { return (StackOrientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        private static void OrientationChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var rg = (RadioGroup)bindable;
            ((StackLayout)rg.Container).Orientation = (StackOrientation)newValue;

        }

        //
        protected override Layout<View> GetContainer()
        {
            return new StackLayout() { Orientation = this.Orientation};
        }
    }
}
