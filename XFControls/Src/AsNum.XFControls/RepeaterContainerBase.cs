using Xamarin.Forms;

namespace AsNum.XFControls
{
    /// <summary>
    /// 
    /// </summary>
    internal static class RepeaterContainerFactory
    {
        public static RepeaterContainerBase Get(RepeaterOrientation orientation)
        {
            switch (orientation)
            {
                case RepeaterOrientation.Horizontal:
                    return new HorizontalRepeaterContainer();
                case RepeaterOrientation.Vertical:
                    return new VerticalRepeaterContainer();
                default:
                    return new WrapRepeaterContainer();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal abstract class RepeaterContainerBase
    {
        public abstract Layout<View> Layout {get;}
    }

    /// <summary>
    /// 
    /// </summary>
    internal class WrapRepeaterContainer : RepeaterContainerBase
    {
        public override Layout<View> Layout
        {
            get {return new WrapLayout();}
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class VerticalRepeaterContainer : RepeaterContainerBase
    {
        public override Layout<View> Layout
        {
            get
            {
                return new StackLayout()
                {
                    Orientation = StackOrientation.Vertical
                };
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class HorizontalRepeaterContainer : RepeaterContainerBase
    {
        public override Layout<View> Layout
        {
            get
            {
                return new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };
            }
        }
    }
}
