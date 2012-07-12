using System;
using System.Windows.Input;
using AAafeen.Models;
using AAafeen.Utils;

namespace AAafeen.ViewModels
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        public MainWindowViewModel()
        {
            main = new Main();

            main.PropertyChanged += (sender, e) => OnPropertyChanged(e.PropertyName);

            Excute = new DelegateCommand()
            {
                ExecuteHandler = args => main.Excute()
            };

            Authorize = new DelegateCommand()
            {
                ExecuteHandler = args =>
                {
                    try
                    {
                        main.Authorize();
                    }
                    catch(Exception ex)
                    {
                        TextBoxText += string.Format("\n\nエラー\n{0}\n\n詳細情報\n{1}", ex.Message, ex.ToString());
                    }
                }
            };
        }

        private Main main;

        public string TextBoxText
        {
            set
            {
                main.TextBoxText = value;
            }
            get
            {
                return main.TextBoxText;
            }
        }

        #region コマンド
        private ICommand exit = new DelegateCommand()
        {
            ExecuteHandler = args => App.Current.MainWindow.Close()
        };
        public ICommand Exit
        {
            get
            {
                return exit;
            }
        }

        public ICommand Authorize { private set; get; }

        public ICommand Excute { private set; get; }
        #endregion
    }
}
