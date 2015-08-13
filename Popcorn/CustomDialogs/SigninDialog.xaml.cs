using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public bool ShouldSignup { get; set; }
    }

    public partial class SigninDialog : INotifyDataErrorInfo
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

            this.KeyDown += escapeKeyHandler;

            PART_SignupButton.Click += signupHandler;
            PART_SigninButton.Click += signinHandler;
            PART_CancelButton.Click += cancelHandler;

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
        public bool IsUsernameValid(string value)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Username", USERNAME_EMPTY_ERROR, false);
                isValid = false;
            }
            else RemoveError("Username", USERNAME_EMPTY_ERROR);

            return isValid;
        }

        // Validates the Password property, updating the errors collection as needed.
        public bool IsPasswordValid(string value)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Password", PASSWORD_EMPTY_ERROR, false);
                isValid = false;
            }
            else RemoveError("Password", PASSWORD_EMPTY_ERROR);

            return isValid;
        }

        private Dictionary<String, List<String>> errors =
            new Dictionary<string, List<string>>();
        private const string USERNAME_EMPTY_ERROR = "Username must be filled.";
        private const string PASSWORD_EMPTY_ERROR = "Password must be filled.";

        // Adds the specified error to the errors collection if it is not 
        // already present, inserting it in the first position if isWarning is 
        // false. Raises the ErrorsChanged event if the collection changes. 
        public void AddError(string propertyName, string error, bool isWarning)
        {
            if (!errors.ContainsKey(propertyName))
                errors[propertyName] = new List<string>();

            if (!errors[propertyName].Contains(error))
            {
                if (isWarning) errors[propertyName].Add(error);
                else errors[propertyName].Insert(0, error);
                RaiseErrorsChanged(propertyName);
            }
        }

        // Removes the specified error from the errors collection if it is
        // present. Raises the ErrorsChanged event if the collection changes.
        public void RemoveError(string propertyName, string error)
        {
            if (errors.ContainsKey(propertyName) &&
                errors[propertyName].Contains(error))
            {
                errors[propertyName].Remove(error);
                if (errors[propertyName].Count == 0) errors.Remove(propertyName);
                RaiseErrorsChanged(propertyName);
            }
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #region INotifyDataErrorInfo Members

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName) ||
                !errors.ContainsKey(propertyName))
                return null;
            return errors[propertyName];
        }

        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        #endregion
    }
}
