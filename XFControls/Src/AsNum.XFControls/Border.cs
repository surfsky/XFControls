using Xamarin.Forms;

namespace AsNum.XFControls
{

    /// <summary>
    /// 边框定义，具体实现由各平台的Render实现（如BorderRender）
    /// </summary>
    public class Border : ContentView
    {
        // 绑定属性
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create("CornerRadius", typeof(CornerRadius), typeof(Border), default(CornerRadius));
        public static readonly BindableProperty StrokeProperty = BindableProperty.Create("Stroke", typeof(Color), typeof(Border), Color.Default);
        public static readonly BindableProperty StrokeThicknessProperty = BindableProperty.Create("StrokeThickness", typeof(Thickness),  typeof(Border), default(Thickness));
        public static readonly BindableProperty IsClippedToBorderProperty = BindableProperty.Create("IsClippedToBorder", typeof(bool), typeof(bool), true);


        /// <summary>
        /// 圆角大小
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// 边框颜色
        /// </summary>
        public Color Stroke
        {
            get { return (Color)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        /// <summary>
        /// 边框宽度
        /// </summary>
        public Thickness StrokeThickness
        {
            get { return (Thickness)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// 是否裁剪超出部分
        /// </summary>
        public bool IsClippedToBorder
        {
            get { return (bool)GetValue(IsClippedToBorderProperty); }
            set { SetValue(IsClippedToBorderProperty, value); }
        }

    }
}
