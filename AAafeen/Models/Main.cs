using System;
using System.Diagnostics;
using System.Reflection;
using AAafeen.Models.Commands;
using AAafeen.Views;
using AAafeen.Utils;
using LinqToTwitter;

namespace AAafeen.Models
{
    public class Main : NotifyPropertyChangedBase
    {
        private string textBoxText;
        public string TextBoxText
        {
            set
            {
                textBoxText = value;
                OnPropertyChanged("TextBoxText");
            }
            get
            {
                return textBoxText;
            }
        }

        public void Authorize()
        {
            var auth = new PinAuthorizer()
            {
                UserAgent = "AAafeen v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Credentials = new InMemoryCredentials()
                {
                    ConsumerKey = "BFtEaHnLWCMp9UjJEjBg",
                    ConsumerSecret = "VYwKqXpJsLPwYhdSzmQD0ri3K2Fh4n4MQyFIxZ6ME"
                },
                GoToTwitterAuthorization = uri => Process.Start(uri),
                GetPin = () =>
                {
                    var f = new InputPinCodeWindow();
                    f.ShowDialog();
                    return f.PinCode;
                }
            };

            try
            {
                auth.Authorize();
            }
            catch (Exception ex)
            {
                throw new Exception("認証に失敗しました。", ex);
            }

            Methods.twCtx.AuthorizedClient = auth;
        }

        public void Excute()
        {
            try
            {
                var result = CommandsExcute.Excute(TextBoxText);
                TextBoxText += "\n\n\n" + result;
            }
            catch (Exception ex)
            {
                TextBoxText += string.Format("\n\n\n\nエラー\n{0}\n\n詳細情報\n{1}", ex.Message, ex.ToString());
            }
        }
    }
}
