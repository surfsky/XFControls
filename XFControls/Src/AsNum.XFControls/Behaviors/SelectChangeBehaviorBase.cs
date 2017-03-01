using Xamarin.Forms;

namespace AsNum.XFControls.Behaviors
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SelectChangeBehaviorBase : BindableBehavior<VisualElement>
    {
        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create("IsSelected",
                typeof(bool),
                typeof(SelectChangeBehaviorBase),
                false,
                BindingMode.Default,
                propertyChanged: IsSelectedChanged);

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void IsSelectedChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var behavior = (SelectChangeBehaviorBase)bindable;
            behavior.OnSelectedChanged();
        }

        protected virtual void OnSelectedChanged()
        {

        }
    }
}
