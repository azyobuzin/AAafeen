using System;
using System.Windows;
using System.Windows.Input;

namespace AAafeen.Utils
{
    //http://www.atmarkit.co.jp/fdotnet/chushin/introwpf_06/introwpf_06_03.html
    public class DelegateCommand : ICommand
    {
        public Action<object> ExecuteHandler { get; set; }
        public Func<object, bool> CanExecuteHandler { get; set; }

        #region ICommand メンバー

        public bool CanExecute(object parameter)
        {
            var d = CanExecuteHandler;
            return d == null ? true : d(parameter);
        }

        public void Execute(object parameter)
        {
            var d = ExecuteHandler;
            if (d != null)
                d(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            var d = CanExecuteChanged;
            if (d != null)
                d(this, null);
        }

        #endregion
    }
}