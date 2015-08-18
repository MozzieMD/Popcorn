using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;

namespace Popcorn.Dialogs
{
    public class ExceptionDialogSettings : MetroDialogSettings
    {
        public ExceptionDialogSettings(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public string Title { get; }

        public string Message { get; }
    }

    public partial class ExceptionDialog
    {
        internal ExceptionDialog(ExceptionDialogSettings settings)
        {
            InitializeComponent();
            Message = settings.Message;
            Title = settings.Title;
        }

        internal Task WaitForButtonPressAsync()
        {
            TaskCompletionSource<SigninDialogData> tcs = new TaskCompletionSource<SigninDialogData>();

            RoutedEventHandler okHandler = null;
            KeyEventHandler okKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = DialogSettings.CancellationToken.Register(() =>
            {
                cleanUpHandlers();
                tcs.TrySetResult(null);
            });

            cleanUpHandlers = () => {
                KeyDown -= escapeKeyHandler;

                PART_OkButton.Click -= okHandler;

                PART_OkButton.KeyDown -= okKeyHandler;

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

            okKeyHandler = (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(null);
                }
            };

            okHandler = (sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(null);

                e.Handled = true;
            };

            PART_OkButton.KeyDown += okKeyHandler;

            KeyDown += escapeKeyHandler;

            PART_OkButton.Click += okHandler;

            return tcs.Task;
        }

        protected override void OnLoaded()
        {
            switch (DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    PART_OkButton.Style = FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    break;
            }
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(ExceptionDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty OkButtonTextProperty = DependencyProperty.Register("OkButtonText", typeof(string), typeof(ExceptionDialog), new PropertyMetadata("Ok"));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public string OkButtonText
        {
            get { return (string)GetValue(OkButtonTextProperty); }
            set { SetValue(OkButtonTextProperty, value); }
        }
    }
}
