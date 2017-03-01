using System;
using System.ComponentModel;
using AsNum.XFControls;
using AsNum.XFControls.iOS;
using UIKit;
using CoreAnimation;
using CoreGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Border), typeof(BorderRender))]
namespace AsNum.XFControls.iOS
{
    /// <summary>
    /// Border 的渲染器（IOS平台）
    /// </summary>
    public class BorderRender : VisualElementRenderer<Border>
    {
        /// <summary>
        /// 边框位置
        /// </summary>
        enum BorderPosition
        {
            Left,
            Top,
            Right,
            Bottom
        }

        // 4块边角
        private CALayer[] borderLayers = new CALayer[4];

        // 可视元素变更
        protected override void OnElementChanged(ElementChangedEventArgs<Border> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
                this.SetupLayer();
        }

        // 属性变更事件
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
                    e.PropertyName == Border.StrokeProperty.PropertyName ||
                    e.PropertyName == Border.StrokeThicknessProperty.PropertyName ||
                    e.PropertyName == Border.CornerRadiusProperty.PropertyName ||
                    e.PropertyName == Border.WidthProperty.PropertyName ||
                    e.PropertyName == Border.HeightProperty.PropertyName)
            {
                this.SetupLayer();
            }
        }

        //
        private void SetupLayer()
        {
            if (Element == null || Element.Width <= 0 || Element.Height <= 0)
                return;

            // 整体形状
            Layer.CornerRadius = (nfloat)Element.CornerRadius.TopLeft;   // 圆角大小统一用左上角???
            if (Element.BackgroundColor != Color.Default)
                Layer.BackgroundColor = Element.BackgroundColor.ToCGColor();
            else
                Layer.BackgroundColor = UIColor.White.CGColor;
            Layer.BorderColor = Element.Stroke.ToCGColor();
            Layer.MasksToBounds = true;

            // 画四个边框
            UpdateBorderLayer(BorderPosition.Left, (nfloat)Element.StrokeThickness.Left);
            UpdateBorderLayer(BorderPosition.Top, (nfloat)Element.StrokeThickness.Top);
            UpdateBorderLayer(BorderPosition.Right, (nfloat)Element.StrokeThickness.Right);
            UpdateBorderLayer(BorderPosition.Bottom, (nfloat)Element.StrokeThickness.Bottom);

            // 其它属性
            Layer.RasterizationScale = UIScreen.MainScreen.Scale;
            Layer.ShouldRasterize = true;
            Layer.BorderColor = Element.Stroke.ToCGColor();
            Layer.BorderWidth = (float)Element.StrokeThickness.Left;
        }

        // 用笨笨的办法添加4个边框（小矩形）
        // 边框不同宽度的意义并不是很大，可以取消掉。圆角不一样大倒是经常遇到，如对话框按钮。
        void UpdateBorderLayer(BorderPosition borderPosition, nfloat thickness)
        {
            var borderLayer = borderLayers[(int)borderPosition];
            if (thickness <= 0)
            {
                if (borderLayer != null)
                {
                    borderLayer.RemoveFromSuperLayer();
                    borderLayers[(int)borderPosition] = null;
                }
            }
            else
            {
                if (borderLayer == null)
                {
                    borderLayer = new CALayer();
                    Layer.AddSublayer(borderLayer);
                    borderLayers[(int)borderPosition] = borderLayer;
                }

                switch (borderPosition)
                {
                    case BorderPosition.Left:
                        borderLayer.Frame = new CGRect(0, 0, thickness, (nfloat)Element.Height);
                        break;
                    case BorderPosition.Top:
                        borderLayer.Frame = new CGRect(0, 0, (nfloat)Element.Width, thickness);
                        break;
                    case BorderPosition.Right:
                        borderLayer.Frame = new CGRect((nfloat)Element.Width - thickness, 0, thickness, (nfloat)Element.Height);
                        break;
                    case BorderPosition.Bottom:
                        borderLayer.Frame = new CGRect(0, (nfloat)Element.Height - thickness, (nfloat)Element.Width, thickness);
                        break;
                }

                borderLayer.BackgroundColor = Element.Stroke.ToCGColor();
                borderLayer.CornerRadius = (nfloat)Element.CornerRadius.TopLeft; //?????
            }
        }
    }
}