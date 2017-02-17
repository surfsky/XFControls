using System;
using System.Diagnostics;
using AsNum.XFControls.iOS;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(TapEffect), "TapEffect")]
namespace AsNum.XFControls.iOS
{

    /// <summary>
    /// 点击效果
    /// http://blog.csdn.net/iosweb/article/details/51555651
    /// http://www.jianshu.com/p/f80ef6219d6b
    /// http://www.cnblogs.com/wendingding/p/3800961.html
    /// 
    /// 动画
    /// https://github.com/xamarin/ios-samples/blob/master/CustomPropertyAnimation/AppDelegate.cs
    /// </summary>
    public class TapEffect : PlatformEffect
    {
        private MyTap Reg;

        protected override void OnAttached()
        {
            this.Reg = new MyTap();
            this.Container.AddGestureRecognizer(this.Reg);
        }

        protected override void OnDetached()
        {
            this.Container.RemoveGestureRecognizer(this.Reg);
            this.Reg.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public class RadialGradientLayer : CALayer
        {

            [Export("center")]
            public CGPoint Center { get; set; }

            [Export("radius")]
            public nfloat Radius { get; set; }

            public RadialGradientLayer()
            {
            }

            //MUST
            [Export("initWithLayer:")]
            public RadialGradientLayer(CALayer other) : base(other)
            {
            }

            // 绘制渐变
            public override void DrawInContext(CGContext ctx)
            {
                base.DrawInContext(ctx);
                var colors = new CGColor[] { UIColor.White.CGColor, new CGColor(1, 1, 1, 0) };
                var locations = new nfloat[] { 0f, 1f };
                var startCenter = this.Center;
                var endCenter = this.Center;
                nfloat startRadius = 0f;
                nfloat endRadius = this.Radius;

                using (var colorSpace = CGColorSpace.CreateDeviceRGB())
                    using (var gradient = new CGGradient(colorSpace, colors, locations))
                        ctx.DrawRadialGradient(gradient, startCenter, startRadius, endCenter, endRadius, CGGradientDrawingOptions.DrawsAfterEndLocation);
            }

            //MUST
            [Export("needsDisplayForKey:")]
            static bool NeedsDisplayForKey(NSString key)
            {
                switch (key.ToString())
                {
                    case "radius":
                    case "center":
                        return true;
                    default:
                        return CALayer.NeedsDisplayForKey(key);
                }
            }

            public override void Clone(CALayer other)
            {
                var o = (RadialGradientLayer)other;
                Radius = o.Radius;
                Center = o.Center;
                base.Clone(other);
            }
        }


        /// <summary>
        /// 点击后显示放射渐变涟漪
        /// </summary>
        public class MyTap : UITapGestureRecognizer
        {
            private RadialGradientLayer Layer = null;

            private void SetAnimation(float from, float to, double duration = 0.3)
            {
                this.Layer.RemoveAnimation("radius");
                using (var ani = CABasicAnimation.FromKeyPath("radius"))
                {
                    ani.Duration = duration;
                    ani.From = NSNumber.FromNFloat(from);
                    ani.To = NSNumber.FromFloat(to);
                    //ani.SetTo(NSNumber.FromFloat(to));
                    ani.AutoReverses = false;
                    ani.RepeatCount = 1;

                    //http://www.jianshu.com/p/02c341c748f9
                    ani.RemovedOnCompletion = false;
                    ani.FillMode = CAFillMode.Forwards;
                    ani.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
                    this.Layer.AddAnimation(ani, "radius");
                }
            }

            public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
            {
                base.TouchesBegan(touches, evt);
                if (this.Layer == null)
                {
                    this.Layer = new RadialGradientLayer();
                    this.Layer.Frame = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height);
                    this.View.Layer.AddSublayer(this.Layer);
                }

                var t = (UITouch)touches.AnyObject;
                var point = t.LocationInView(this.View);
                this.Layer.Center = point;
                this.SetAnimation(0, 50);
            }

            public override bool CanPreventGestureRecognizer(UIGestureRecognizer preventedGestureRecognizer)
            {
                return false;
            }

            public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
            {
                base.TouchesEnded(touches, evt);
                if (this.Layer != null)
                    this.SetAnimation(50, 0);
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                if (disposing && this.Layer != null)
                {
                    this.Layer.Dispose();
                    this.Layer = null;
                }
            }
        }
    }
}
