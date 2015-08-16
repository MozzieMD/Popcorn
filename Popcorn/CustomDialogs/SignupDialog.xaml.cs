using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;

namespace Popcorn.CustomDialogs
{
    public class SignupDialogSettings : MetroDialogSettings
    {
        private const string DefaultTitle = "Signup";
        private const string DefaultSignupButtonText = "Signup";
        private const string DefaultMessage = "Connect with your account to access social functionalities of Popcorn.";
        private const string DefaultUsernameWatermark = "User name...";
        private const string DefaultFirstNameWatermark = "First name...";
        private const string DefaultLastNameWatermark = "Last name...";
        private const string DefaultEmailWatermark = "Email...";
        private const string DefaultPasswordWatermark = "Password...";
        private const string DefaultConfirmPasswordWatermark = "Confirm password...";
        private const bool DefaultEnablePasswordPreview = false;

        public SignupDialogSettings()
        {
            Title = DefaultTitle;
            Message = DefaultMessage;
            SignupButtonText = DefaultSignupButtonText;
            UsernameWatermark = DefaultUsernameWatermark;
            FirstNameWatermark = DefaultFirstNameWatermark;
            LastNameWatermark = DefaultLastNameWatermark;
            EmailWatermark = DefaultEmailWatermark;
            PasswordWatermark = DefaultPasswordWatermark;
            ConfirmPasswordWatermark = DefaultConfirmPasswordWatermark;
            EnablePasswordPreview = DefaultEnablePasswordPreview;
        }

        public string Title { get; set; }

        public string Message { get; set; }

        public string SignupButtonText { get; set; }

        public string InitialUsername { get; set; }

        public string InitialPassword { get; set; }

        public string UsernameWatermark { get; set; }

        public string FirstNameWatermark { get; set; }

        public string LastNameWatermark { get; set; }

        public string EmailWatermark { get; set; }

        public string PasswordWatermark { get; set; }

        public string ConfirmPasswordWatermark { get; set; }

        public bool EnablePasswordPreview { get; set; }
    }

    public class SignupDialogData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public partial class SignupDialog : INotifyDataErrorInfo
    {
        internal SignupDialog(SignupDialogSettings settings)
        {
            InitializeComponent();
            Username = settings.InitialUsername;
            Password = settings.InitialPassword;
            Message = settings.Message;
            Title = settings.Title;
            SignupButtonText = settings.SignupButtonText;
            UsernameWatermark = settings.UsernameWatermark;
            FirstNameWatermark = settings.FirstNameWatermark;
            LastNameWatermark = settings.LastNameWatermark;
            EmailWatermark = settings.EmailWatermark;
            PasswordWatermark = settings.PasswordWatermark;
            ConfirmPasswordWatermark = settings.ConfirmPasswordWatermark;
        }

        internal Task<SignupDialogData> WaitForButtonPressAsync()
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

            TaskCompletionSource<SignupDialogData> tcs = new TaskCompletionSource<SignupDialogData>();

            RoutedEventHandler signupHandler = null;
            KeyEventHandler signupKeyHandler = null;

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
                PART_TextBox.KeyDown -= signupKeyHandler;
                PART_TextBox2.KeyDown -= signupKeyHandler;
                PART_TextBox3.KeyDown -= signupKeyHandler;

                this.KeyDown -= escapeKeyHandler;

                PART_SignupButton.Click -= signupHandler;
                PART_CancelButton.Click -= cancelHandler;

                PART_SignupButton.KeyDown -= signupKeyHandler;
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
                    var isValid = IsUsernameValid(Username)
                                   && IsEmailValid(Email)
                                   && IsFirstNameValid(FirstName)
                                   && IsLastNameValid(LastName)
                                   && IsPasswordValid(Password)
                                   && IsConfirmPasswordValid(ConfirmPassword);
                    if (isValid)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(new SignupDialogData
                        {
                            Username = Username,
                            FirstName = FirstName,
                            LastName = LastName,
                            Email = Email,
                            Password = Password,
                            ConfirmPassword = ConfirmPassword
                        });
                    }
                }
            };

            signupHandler = (sender, e) =>
            {
                var isValid = IsUsernameValid(Username)
                               && IsEmailValid(Email)
                               && IsFirstNameValid(FirstName)
                               && IsLastNameValid(LastName)
                               && IsPasswordValid(Password)
                               && IsConfirmPasswordValid(ConfirmPassword);
                if (isValid)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(new SignupDialogData
                    {
                        Username = Username,
                        FirstName = FirstName,
                        LastName = LastName,
                        Email = Email,
                        Password = Password,
                        ConfirmPassword = ConfirmPassword
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
            PART_CancelButton.KeyDown += cancelKeyHandler;

            PART_TextBox.KeyDown += cancelKeyHandler;
            PART_TextBox2.KeyDown += cancelKeyHandler;
            PART_TextBox3.KeyDown += cancelKeyHandler;

            this.KeyDown += escapeKeyHandler;

            PART_SignupButton.Click += signupHandler;
            PART_CancelButton.Click += cancelHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            var settings = this.DialogSettings as SignupDialogSettings;
            if (settings != null && settings.EnablePasswordPreview)
            {
                var win8MetroPasswordStyle = this.FindResource("Win8MetroPasswordBox") as Style;
                if (win8MetroPasswordStyle != null)
                {
                    PART_TextBox2.Style = win8MetroPasswordStyle;
                    PART_TextBox3.Style = win8MetroPasswordStyle;
                }
            }

            switch (DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    this.PART_SignupButton.Style = this.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    PART_TextBox.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox2.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    PART_TextBox3.SetResourceReference(ForegroundProperty, "BlackColorBrush");
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty UsernameWatermarkProperty = DependencyProperty.Register("UsernameWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FirstNameProperty = DependencyProperty.Register("FirstName", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FirstNameWatermarkProperty = DependencyProperty.Register("FirstNameWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty LastNameProperty = DependencyProperty.Register("LastName", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty LastNameWatermarkProperty = DependencyProperty.Register("LastNameWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EmailProperty = DependencyProperty.Register("Email", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty EmailWatermarkProperty = DependencyProperty.Register("EmailWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ConfirmPasswordProperty = DependencyProperty.Register("ConfirmPassword", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty PasswordWatermarkProperty = DependencyProperty.Register("PasswordWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty ConfirmPasswordWatermarkProperty = DependencyProperty.Register("ConfirmPasswordWatermark", typeof(string), typeof(SignupDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SignupButtonTextProperty = DependencyProperty.Register("SignupButtonText", typeof(string), typeof(SignupDialog), new PropertyMetadata("Signup"));
        public static readonly DependencyProperty CancelButtonTextProperty = DependencyProperty.Register("CancelButtonText", typeof(string), typeof(SignupDialog), new PropertyMetadata("Cancel"));

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

        public string FirstName
        {
            get { return (string)GetValue(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public string FirstNameWatermark
        {
            get { return (string)GetValue(FirstNameWatermarkProperty); }
            set { SetValue(FirstNameWatermarkProperty, value); }
        }

        public string LastName
        {
            get { return (string)GetValue(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public string LastNameWatermark
        {
            get { return (string)GetValue(LastNameWatermarkProperty); }
            set { SetValue(LastNameWatermarkProperty, value); }
        }

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        public string EmailWatermark
        {
            get { return (string)GetValue(EmailWatermarkProperty); }
            set { SetValue(EmailWatermarkProperty, value); }
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public string ConfirmPassword
        {
            get { return (string)GetValue(ConfirmPasswordProperty); }
            set { SetValue(ConfirmPasswordProperty, value); }
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

        public string ConfirmPasswordWatermark
        {
            get { return (string)GetValue(ConfirmPasswordWatermarkProperty); }
            set { SetValue(ConfirmPasswordWatermarkProperty, value); }
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

        // Validates the FirstName property, updating the errors collection as needed.
        public bool IsFirstNameValid(string value)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("FirstName", FIRSTNAME_EMPTY_ERROR, false);
                isValid = false;
            }
            else RemoveError("FirstName", FIRSTNAME_EMPTY_ERROR);

            return isValid;
        }

        // Validates the LastName property, updating the errors collection as needed.
        public bool IsLastNameValid(string value)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("LastName", LASTNAME_EMPTY_ERROR, false);
                isValid = false;
            }
            else RemoveError("LastName", LASTNAME_EMPTY_ERROR);

            return isValid;
        }

        // Validates the Email property, updating the errors collection as needed.
        public bool IsEmailValid(string value)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(value))
            {
                AddError("Email", EMAIL_EMPTY_ERROR, false);
                isValid = false;
            }
            else RemoveError("Email", EMAIL_EMPTY_ERROR);

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

            if (!string.IsNullOrEmpty(value) && value.Length < 6) AddError("Password", PASSWORD_FORMAT_ERROR, true);
            else RemoveError("Password", PASSWORD_FORMAT_ERROR);

            return isValid;
        }

        // Validates the ConfirmPassword property, updating the errors collection as needed.
        public bool IsConfirmPasswordValid(string value)
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(value) || value != Password)
            {
                AddError("ConfirmPassword", CONFIRMPASSWORD_MISMATCH_ERROR, false);
                isValid = false;
            }
            else RemoveError("ConfirmPassword", CONFIRMPASSWORD_MISMATCH_ERROR);

            return isValid;
        }

        private Dictionary<String, List<String>> errors =
            new Dictionary<string, List<string>>();
        private const string USERNAME_EMPTY_ERROR = "Username must be filled.";
        private const string EMAIL_EMPTY_ERROR = "Email must be filled.";
        private const string FIRSTNAME_EMPTY_ERROR = "First name must be filled.";
        private const string LASTNAME_EMPTY_ERROR = "Last name must be filled.";
        private const string PASSWORD_EMPTY_ERROR = "Password must be filled.";
        private const string PASSWORD_FORMAT_ERROR = "Password must contain at least 6 characters.";
        private const string CONFIRMPASSWORD_MISMATCH_ERROR = "Passwords must match.";

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
