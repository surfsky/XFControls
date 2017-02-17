using AsNum.XFControls.iOS.Effects;
using System;
using System.ComponentModel;
using System.Windows.Input;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using AsNum.XFControls.Effects;

//同一个解决方案只允许一个
//[assembly: ResolutionGroupName("AsNum")]
[assembly: ExportEffect(typeof(KeyboardEnterEffect), "KeyboardEnterEffect")]
namespace AsNum.XFControls.iOS.Effects
{
    /// <summary>
    /// 键盘聚焦效果
    /// </summary>
    public class KeyboardEnterEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (this.Control is UITextField)
                this.Update();
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
            if (args.PropertyName.Equals(KeyboardEnter.TypeProperty.PropertyName))
                this.Update();
        }

        private void Update()
        {
            var txt = (UITextField)this.Control;
            txt.ReturnKeyType = this.Convert(KeyboardEnter.GetType(this.Element));
            txt.PrimaryActionTriggered += Txt_PrimaryActionTriggered;
        }

        private void Txt_PrimaryActionTriggered(object sender, EventArgs e)
        {
            var cmd = (ICommand)this.Element.GetValue(KeyboardEnter.CmdProperty);
            var param = this.Element.GetValue(KeyboardEnter.ParamProperty);
            if (cmd != null && cmd.CanExecute(param))
                cmd.Execute(param);
        }

        private UIReturnKeyType Convert(KeyboardEnterTypes key)
        {
            switch (key)
            {
                case KeyboardEnterTypes.Done:    return UIReturnKeyType.Done;
                case KeyboardEnterTypes.Go:      return UIReturnKeyType.Go;
                case KeyboardEnterTypes.Next:    return UIReturnKeyType.Next;
                case KeyboardEnterTypes.Search:  return UIReturnKeyType.Search;
                case KeyboardEnterTypes.Send:    return UIReturnKeyType.Send;
                default:                         return UIReturnKeyType.Default;
            }
        }
    }
}
