using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;

namespace Popcorn.Dialogs
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

        public string Title { get; }

        public string Message { get; }

        public string SigninButtonText { get; }

        public string SignupButtonText { get; }

        public string UsernameWatermark { get; }

        public string PasswordWatermark { get; }

        public bool EnablePasswordPreview { get; }
    }

    public class SigninDialogData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool ShouldSignup { get; set; }
    }

    public partial class SigninDialog : INotifyDataErrorInfo
    {
        internal SigninDialog(SigninDialogSettings settings)
        {
            InitializeComponent();
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
                Focus();
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

            RoutedEventHandler signupHandler = null;
            KeyEventHandler signupKeyHandler = null;

            RoutedEventHandler signinHandler = null;
            KeyEventHandler signinKeyHandler = null;

            RoutedEventHandler cancelHandler = null;
            KeyEventHandler cancelKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanUpHandlers();
                tcs.TrySetResult(null);
            });

            cleanUpHandlers = () => {
                PART_TextBox.KeyDown -= signinKeyHandler;
                PART_TextBox2.KeyDown -= signinKeyHandler;

                this.KeyDown -= escapeKeyHandler;

                PART_SignupButton.Click -= signupHandler;
                PART_SigninButton.Click -= signinHandler;
                PART_CancelButton.Click -= cancelHandler;

                PART_SignupButton.KeyDown -= signupKeyHandler;
                PART_SigninButton.KeyDown -= signinKeyHandler;
                PART_CancelButton.KeyDown -= cancelKeyHandler;

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

            signupKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(new SigninDialogData { ShouldSignup = true });
                }
            };

            signupHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(new SigninDialogData { ShouldSignup = true });

                e.Handled = true;
            };

            signinKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    var isValid = IsUsernameValid(Username)
                                  && IsPasswordValid(Password);
                    if (isValid)
                    {
                        cleanUpHandlers();
                        tcs.TrySetResult(new SigninDialogData
                        {
                            Username = Username,
                            Password = PART_TextBox2.Password,
                            ShouldSignup = false
                        });
                    }
                }
            };

            signinHandler = (sender, e) =>
            {
                var isValid = IsUsernameValid(Username)
                              && IsPasswordValid(Password);
                if (isValid)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(new SigninDialogData
                    {
                        Username = Username,
                        Password = PART_TextBox2.Password,
                        ShouldSignup = false
                    });

                    e.Handled = true;
                }
            };

            cancelKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();
                    tcs.TrySetResult(null);
                }
            };

            cancelHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(null);

                e.Handled = true;
            };

            PART_SignupButton.KeyDown += signupKeyHandler;
            PART_SigninButton.KeyDown += signinKeyHandler;
            PART_CancelButton.KeyDown += cancelKeyHandler;

            PART_TextBox.KeyDown += signinKeyHandler;
            PART_TextBox2.KeyDown += signinKeyHandler;

            KeyDown += escapeKeyHandler;

            PART_SignupButton.Click += signupHandler;
            PART_SigninButton.Click += signinHandler;
            PART_CancelButton.Click += cancelHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            var settings = DialogSettings as SigninDialogSettings;
            if (settings != null && settings.EnablePasswordPreview)
            {
                var win8MetroPasswordStyle = FindResource("Win8MetroPasswordBox") as Style;
                if (win8MetroPasswordStyle != null)
                {
                    PART_TextBox2.Style = win8MetroPasswordStyle;
                }
            }

            switch (DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    PART_SignupButton.Style = FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(SigninDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(SigninDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register("UsernameWatermark", typeof(string), typeof(SigninDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(SigninDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(SigninDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SigninButtonTextProperty = DependencyProperty.Register("SigninButtonText", typeof(string), typeof(SigninDialog), new PropertyMetadata("Signin"));
        public static readonly DependencyProperty SignupButtonTextProperty = DependencyProperty.Register("SignupButtonText", typeof(string), typeof(SigninDialog), new PropertyMetadata("Signup"));
        public static readonly DependencyProperty CancelButtonTextProperty = DependencyProperty.Register("CancelButtonText", typeof(string), typeof(SigninDialog), new PropertyMetadata("Cancel"));

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

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }
            set { SetValue(CancelButtonTextProperty, value); }
        }

        // Validates the Username property, updating the errors collection as needed.
        private bool IsUsernameValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Username", UsernameEmptyError, false);
                isValid = false;
            }
            else RemoveError("Username", UsernameEmptyError);

            return isValid;
        }

        // Validates the Password property, updating the errors collection as needed.
        private bool IsPasswordValid(string value)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Password", PasswordEmptyError, false);
                isValid = false;
            }
            else RemoveError("Password", PasswordEmptyError);

            return isValid;
        }

        private readonly Dictionary<string, List<string>> _errors =
            new Dictionary<string, List<string>>();
        private const string UsernameEmptyError = "Username must be filled.";
        private const string PasswordEmptyError = "Password must be filled.";

        // Adds the specified error to the errors collection if it is not 
        // already present, inserting it in the first position if isWarning is 
        // false. Raises the ErrorsChanged event if the collection changes. 
        private void AddError(string propertyName, string error, bool isWarning)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            if (!_errors[propertyName].Contains(error))
            {
                if (isWarning) _errors[propertyName].Add(error);
                else _errors[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        // Removes the specified error from the errors collection if it is
        // present. Raises the ErrorsChanged event if the collection changes.
        private void RemoveError(string propertyName, string error)
        {
            if (_errors.ContainsKey(propertyName) &&
                _errors[propertyName].Contains(error))
            {
                _errors[propertyName].Remove(error);
                if (_errors[propertyName].Count == 0) _errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) ||
                !_errors.ContainsKey(propertyName))
                return null;
            return _errors[propertyName];
        }

        public bool HasErrors => _errors.Count > 0;

        #endregion
    }
}
