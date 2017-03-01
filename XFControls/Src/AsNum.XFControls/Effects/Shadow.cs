using System.Linq;
using Xamarin.Forms;

namespace AsNum.XFControls.Effects
{
    /// <summary>
    /// 
    /// </summary>
    //http://xfcomplete.net/general/2016/01/20/using-effects/
    public class Shadow
    {
        // BindableProperty
        public static readonly BindableProperty RadiusProperty = BindableProperty.CreateAttached("Radius", typeof(float), typeof(Shadow), 0F, propertyChanged: Changed);
        public static readonly BindableProperty ColorProperty = BindableProperty.CreateAttached("Color", typeof(Color), typeof(Shadow), Color.Gray, propertyChanged: Changed);
        public static readonly BindableProperty XProperty = BindableProperty.CreateAttached("X", typeof(float), typeof(Shadow), 5F, propertyChanged: Changed);
        public static readonly BindableProperty YProperty = BindableProperty.CreateAttached("Y", typeof(float), typeof(Shadow), 5F, propertyChanged: Changed);

        // radius
        public static void SetRadius(BindableObject view, float radius)
        {
            view.SetValue(RadiusProperty, radius);
        }

        public static float GetRadius(BindableObject view)
        {
            return (float)view.GetValue(RadiusProperty);
        }

        // color
        public static void SetColor(BindableObject view, Color color)
        {
            view.SetValue(ColorProperty, color);
        }

        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

        // X
        public static void SetX(BindableObject view, float x)
        {
            view.SetValue(XProperty, x);
        }

        public static float GetX(BindableObject view)
        {
            return (float)view.GetValue(XProperty);
        }

        // Y
        public static void SetY(BindableObject view, float y)
        {
            view.SetValue(YProperty, y);
        }

        public static float GetY(BindableObject view)
        {
            return (float)view.GetValue(YProperty);
        }

        // Changed
        private static void Changed(BindableObject bindable, object oldValue, object newValue)
        {
            var view = (View)bindable;
            if (view != null)
            {
                var effect = view.Effects.FirstOrDefault(e => e is ShadowEffect);
                if (effect == null)
                {
                    effect = new ShadowEffect();
                    view.Effects.Add(effect);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        class ShadowEffect : RoutingEffect
        {
            public ShadowEffect() : base("AsNum.ShadowEffect")
            {
            }
        }
    }
}
