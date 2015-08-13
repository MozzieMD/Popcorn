using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;

namespace Popcorn.CustomDialogs
{
    public class SigninDialogSettings : MetroDialogSettings
    {
        private const string DefaultTitle = "Signin";
        private const string DefaultSigninButtonText = "Signin";
        private const string DefaultSignupButtonText = "Signup";
        private const string DefaultMessage = "Connect with your account to access social functionalities of Popcorn.";
        private const string DefaultUsernameWatermark = "Username...";
        private const string DefaultPasswordWatermark = "Password...";
        private const bool DefaultEnablePasswordPreview = false;

        public SigninDialogSettings()
        {
            Title = DefaultTitle;
            Message = DefaultMessage;
            SigninButtonText = DefaultSigninButtonText;
            SignupButtonText = DefaultSignupButtonText;
            UsernameWatermark = DefaultUsernameWatermark;
            PasswordWatermark = DefaultPasswordWatermark;
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public string SigninButtonText { get; set; }

        public string SignupButtonText { get; set; }

        public string InitialUsername { get; set; }

        public string InitialPassword { get; set; }

        public string UsernameWatermark { get; set; }

        public string PasswordWatermark { get; set; }

        public bool EnablePasswordPreview { get; set; }
    }

    public class SigninDialogData
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public partial class SigninDialog
    {
        internal SigninDialog(SigninDialogSettings settings)
        {
            InitializeComponent();
            Username = settings.InitialUsername;
            Password = settings.InitialPassword;
            Message = settings.Message;
            Title = settings.Title;
            SigninButtonText = settings.SigninButtonText;
            SignupButtonText = settings.SignupButtonText;
            UsernameWatermark = settings.UsernameWatermark;
            PasswordWatermark = settings.PasswordWatermark;
        }

        internal Task<SigninDialogData> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Focus();
                if (string.IsNullOrEmpty(PART_TextBox.Text))
                {
                    PART_TextBox.Focus();
                }
                else
                {
                    PART_TextBox2.Focus();
                }
            }));

            TaskCompletionSource<SigninDialogData> tcs = new TaskCompletionSource<SigninDialogData>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanUpHandlers();
                tcs.TrySetResult(null);
            });

            cleanUpHandlers = () => {
                PART_TextBox.KeyDown -= affirmativeKeyHandler;
                PART_TextBox2.KeyDown -= affirmativeKeyHandler;

                this.KeyDown -= escapeKeyHandler;

                PART_SignupButton.Click -= negativeHandler;
                PART_SigninButton.Click -= affirmativeHandler;

                PART_SignupButton.KeyDown -= negativeKeyHandler;
                PART_SigninButton.KeyDown -= affirmativeKeyHandler;

                cancellationTokenRegistration.Dispose();
            };

            escapeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            negativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            affirmativeKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();
                    tcs.TrySetResult(new SigninDialogData { Username = Username, Password = PART_TextBox2.Password });
                }
            };

            negativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(null);

                e.Handled = true;
            };

            affirmativeHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(new SigninDialogData { Username = Username, Password = PART_TextBox2.Password });

                e.Handled = true;
            };

            PART_SignupButton.KeyDown += negativeKeyHandler;
            PART_SigninButton.KeyDown += affirmativeKeyHandler;

            PART_TextBox.KeyDown += affirmativeKeyHandler;
            PART_TextBox2.KeyDown += affirmativeKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_SignupButton.Click += negativeHandler;
            PART_SigninButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            var settings = this.DialogSettings as SigninDialogSettings;
            if (settings != null && settings.EnablePasswordPreview)
            {
                var win8MetroPasswordStyle = this.FindResource("Win8MetroPasswordBox") as Style;
                if (win8MetroPasswordStyle != null)
                {
                    PART_TextBox2.Style = win8MetroPasswordStyle;
                }
            }

            switch (DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_SignupButton.Style = this.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register("UsernameWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(LoginDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SigninButtonTextProperty = DependencyProperty.Register("SigninButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty SignupButtonTextProperty = DependencyProperty.Register("SignupButtonText", typeof(string), typeof(LoginDialog), new PropertyMetadata("Cancel"));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public string UsernameWatermark
        {
            get { return (string)GetValue(UsernameWatermarkProperty); }
            set { SetValue(UsernameWatermarkProperty, value); }
        }

        public string PasswordWatermark
        {
            get { return (string)GetValue(PasswordWatermarkProperty); }
            set { SetValue(PasswordWatermarkProperty, value); }
        }

        public string SigninButtonText
        {
            get { return (string)GetValue(SigninButtonTextProperty); }
            set { SetValue(SigninButtonTextProperty, value); }
        }

        public string SignupButtonText
        {
            get { return (string)GetValue(SignupButtonTextProperty); }
            set { SetValue(SignupButtonTextProperty, value); }
        }
    }
}
