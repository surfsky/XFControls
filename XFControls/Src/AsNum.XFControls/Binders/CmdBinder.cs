using System;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace AsNum.XFControls.Binders
{
    /// <summary>
    /// 
    /// </summary>
    public class CmdBinder
    {
        #region
        public static readonly BindableProperty EventProperty = BindableProperty.CreateAttached("Event", typeof(string), typeof(CmdBinder), null );
        public static readonly BindableProperty CmdProperty =  BindableProperty.CreateAttached("Cmd", typeof(ICommand), typeof(CmdBinder), null, propertyChanged: CommandChanged );
        public static readonly BindableProperty ParamProperty = BindableProperty.CreateAttached("Param", typeof(object), typeof(CmdBinder), null);

        public static object GetParam(BindableObject o)
        {
            return o.GetValue(ParamProperty);
        }

        public static ICommand GetCmd(BindableObject o)
        {
            return (ICommand)o.GetValue(CmdProperty);
        }

        public static string GetEvent(BindableObject o)
        {
            return (string)o.GetValue(EventProperty);
        }
        #endregion

        private static MethodInfo _createDelegateMethod = null;
        private static MethodInfo _fireMethod = null;
        private static bool _canUse = false;

        static CmdBinder()
        {
            _createDelegateMethod = typeof(Delegate).GetRuntimeMethod("CreateDelegate", new Type[] { typeof(Type), typeof(MethodInfo) });
            if (_createDelegateMethod != null)
            {
                _fireMethod = typeof(CmdBinder).GetTypeInfo().GetDeclaredMethod("Fire");
                _canUse = true;
            }
        }

        private static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (_canUse)
            {
                var evt = bindable.GetType().GetTypeInfo().GetDeclaredEvent(GetEvent(bindable));
                if (evt != null)
                {
                    var handler = (Delegate)_createDelegateMethod.Invoke(null, new object[] { evt.EventHandlerType, _fireMethod });
                    evt.AddEventHandler(bindable, handler);
                }
            }
        }

        private static void Fire(object sender, EventArgs e)
        {
            var ele = (BindableObject)sender;
            var param = GetParam(ele);
            var cmd = GetCmd(ele);
            if (cmd.CanExecute(param))
                cmd.Execute(param);
        }
    }
}
