using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AAafeen.Utils;

namespace AAafeen.Views
{
    /// <summary>
    /// InputPinCodeWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InputPinCodeWindow : Window
    {
        public InputPinCodeWindow()
        {
            InitializeComponent();

            okButton.Command = new DelegateCommand()
            {
                ExecuteHandler = args =>
                {
                    PinCode = args.ToString().Trim();
                    this.Close();
                }
            };

            cancelButton.Command = new DelegateCommand()
            {
                ExecuteHandler = args => this.Close()
            };
        }

        public string PinCode { private set; get; }

        private void pinCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && okButton.Command.CanExecute(pinCode.Text))
                okButton.Command.Execute(pinCode.Text);
        }
    }
}
