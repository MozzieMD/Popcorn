using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using xZune.Vlc.Interop.Media;
using Popcorn.Messaging;
using Popcorn.ViewModel.Players;

namespace Popcorn.UserControls.Players
{
    /// <summary>
    /// Interaction logic for MoviePlayer.xaml
    /// </summary>
    public partial class MoviePlayer
    {
        #region Properties

        private bool _disposed;

        #region Property -> MediaPlayerIsPlaying

        /// <summary>
        /// Indicates if a movie is playing 
        /// </summary>
        private bool MediaPlayerIsPlaying { get; set; }

        #endregion

        #region DependencyProperty -> VolumeProperty

        /// <summary>
        /// Identifies the <see cref="Volume"/> dependency property. 
        /// </summary>
        internal static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume", typeof (int),
            typeof (MoviePlayer), new PropertyMetadata(100, OnVolumeChanged));

        #endregion

        #region Property -> Volume

        /// <summary>
        /// Get or set the movie volume 
        /// </summary>
        public int Volume
        {
            get { return (int) GetValue(VolumeProperty); }

            set { SetValue(VolumeProperty, value); }
        }

        #endregion

        #region Property -> ActivityMouse

        /// <summary>
        /// Used to update the activity mouse and mouse position.
        /// Used by <see cref="OnActivity"/> and <see cref="OnInactivity"/> to update PlayerStatusBar visibility.
        /// </summary>
        private DispatcherTimer _activityTimer;

        private Point _inactiveMousePosition = new Point(0, 0);

        #endregion

        #region Property -> UserIsDraggingMediaPlayerSlider

        /// <summary>
        /// Indicate if user is manipulating the timeline player
        /// </summary>
        private bool UserIsDraggingMediaPlayerSlider { get; set; }

        #endregion

        #region Property -> MediaPlayerTimer

        /// <summary>
        /// Timer used for report time on the timeline
        /// </summary>
        private DispatcherTimer MediaPlayerTimer { get; set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MediaPlayer class.
        /// </summary>
        public MoviePlayer()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Method -> Onloaded

        /// <summary>
        /// Do action when loaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnLoaded(object sender, EventArgs e)
        {
            if (Player.State == MediaState.Paused)
            {
                Player.Play();
            }
            else
            {
                var vm = DataContext as MoviePlayerViewModel;
                if (vm == null)
                {
                    return;
                }
                // start the timer used to report time on MediaPlayerSliderProgress
                MediaPlayerTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
                MediaPlayerTimer.Tick += MediaPlayerTimer_Tick;
                MediaPlayerTimer.Start();

                // start the activity timer used to manage visibility of the PlayerStatusBar
                InputManager.Current.PreProcessInput += OnActivity;
                _activityTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(3)};
                _activityTimer.Tick += OnInactivity;

                vm.StoppedPlayingMovie += OnStoppedPlayingMovie;

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Closing += (s1, e1) => Dispose();
                }

                if (vm.MediaUri == null)
                {
                    return;
                }

                Player.LoadMedia(vm.MediaUri);
                Player.VlcMediaPlayer.EndReached += MediaPlayer_EndReached;
                if (!string.IsNullOrEmpty(vm.Movie.SelectedSubtitle?.FilePath))
                {
                    Player.AddOption("--sub-file = " + vm.Movie.SelectedSubtitle.FilePath);
                }

                PlayMedia();
            }
        }

        #endregion

        #region Method -> OnUnloaded

        /// <summary>
        /// Do action when unloaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnUnloaded(object sender, EventArgs e)
        {
            Player.PauseOrResume();
        }

        #endregion

        #region Method -> MediaPlayer_EndReached

        /// <summary>
        /// When a media has been fully played, save Seen property into database
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void MediaPlayer_EndReached(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                var vm = DataContext as MoviePlayerViewModel;
                if (vm == null)
                {
                    return;
                }

                await vm.UserDataService.SeenMovieAsync(vm.Movie);
                Messenger.Default.Send(new StopPlayingMovieMessage());
            });
        }

        #endregion

        #region Method -> OnVolumeChanged

        /// <summary>
        /// When media's volume changed, update volume for all MediaPlayer instances (normal screen and fullscreen)
        /// </summary>
        /// <param name="e">e</param>
        /// <param name="obj">obj</param>
        private static void OnVolumeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var moviePlayer = obj as MoviePlayer;
            if (moviePlayer != null)
            {
                var newVolume = (int) e.NewValue;
                moviePlayer.ChangeMediaVolume(newVolume);
            }
        }

        #endregion

        #region Method -> ChangeMediaVolume

        /// <summary>
        /// Change the media's volume
        /// </summary>
        /// <param name="newValue">New volume value</param>
        private void ChangeMediaVolume(int newValue)
        {
            Player.Volume = newValue;
        }

        #endregion

        #region Method -> OnStoppedPlayingMovie

        /// <summary>
        /// When media has finished playing, stop player and reset timer
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnStoppedPlayingMovie(object sender, EventArgs e)
        {
            Dispose();
        }

        #endregion

        #region Method -> PlayMedia

        /// <summary>
        /// Play the movie when buffered
        /// </summary>
        private void PlayMedia()
        {
            Player.Play();
            MediaPlayerIsPlaying = true;
        }

        #endregion

        #region Method -> MediaPlayerTimer_Tick

        /// <summary>
        /// Report the playing progress on the timeline
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void MediaPlayerTimer_Tick(object sender, EventArgs e)
        {
            if ((Player != null) && (!UserIsDraggingMediaPlayerSlider))
            {
                MediaPlayerSliderProgress.Minimum = 0;
                MediaPlayerSliderProgress.Maximum = Player.Length.TotalSeconds;
                MediaPlayerSliderProgress.Value = Player.Time.TotalSeconds;
            }
        }

        #endregion

        #region Method -> MediaPlayerPlay_CanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        private void MediaPlayerPlay_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MediaPlayerStatusBarItemPlay != null && MoviePlayerStatusBarItemPause != null)
            {
                e.CanExecute = Player != null;
                if (MediaPlayerIsPlaying)
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
                    MoviePlayerStatusBarItemPause.Visibility = Visibility.Visible;
                }
                else
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
                    MoviePlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Method -> MediaPlayerPlay_Executed

        /// <summary>
        /// Play the current movie
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">ExecutedRoutedEventArgs</param>
        private void MediaPlayerPlay_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Player.Play();
            MediaPlayerIsPlaying = true;

            MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
            MoviePlayerStatusBarItemPause.Visibility = Visibility.Visible;
        }

        #endregion

        #region Method -> MediaPlayerPause_CanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the media player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        private void MediaPlayerPause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MediaPlayerStatusBarItemPlay != null && MoviePlayerStatusBarItemPause != null)
            {
                e.CanExecute = MediaPlayerIsPlaying;
                if (MediaPlayerIsPlaying)
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
                    MoviePlayerStatusBarItemPause.Visibility = Visibility.Visible;
                }
                else
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
                    MoviePlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Method -> MediaPlayerPause_Executed

        /// <summary>
        /// Pause the media
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        private void MediaPlayerPause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Player.VlcMediaPlayer?.Pause();
            MediaPlayerIsPlaying = false;

            MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
            MoviePlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Method -> MediaSliderProgress_DragStarted

        /// <summary>
        /// Report when dragging is used on media player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DragStartedEventArgs</param>
        private void MediaSliderProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            UserIsDraggingMediaPlayerSlider = true;
        }

        #endregion

        #region Method -> MediaSliderProgress_DragCompleted

        /// <summary>
        /// Report when user has finished dragging the media player progress
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DragCompletedEventArgs</param>
        private void MediaSliderProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            UserIsDraggingMediaPlayerSlider = false;
            Player.Time = TimeSpan.FromSeconds(MediaPlayerSliderProgress.Value);
        }

        #endregion

        #region Method -> MovieSliderProgress_ValueChanged

        /// <summary>
        /// Report runtime when movie player progress changed
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">RoutedPropertyChangedEventArgs</param>
        private void MediaSliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var vm = DataContext as MoviePlayerViewModel;
            if (vm != null)
            {
                MoviePlayerTextProgressStatus.Text =
                    TimeSpan.FromSeconds(MediaPlayerSliderProgress.Value)
                        .ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture) + " / " +
                    TimeSpan.FromSeconds(Player.Length.TotalSeconds)
                        .ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
            }
        }

        #endregion

        #region Method -> MouseWheelMediaPlayer

        /// <summary>
        /// When user uses the mousewheel, update the volume
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">MouseWheelEventArgs</param>
        private void MouseWheelMediaPlayer(object sender, MouseWheelEventArgs e)
        {
            if ((Volume <= 190 && e.Delta > 0) || (Volume >= 10 && e.Delta < 0))
            {
                Volume += (e.Delta > 0) ? 10 : -10;
            }
        }

        #endregion

        #region Method -> OnInactivity

        /// <summary>
        /// Hide the PlayerStatusBar on mouse inactivity 
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnInactivity(object sender, EventArgs e)
        {
            // remember mouse position
            _inactiveMousePosition = Mouse.GetPosition(Container);

            if (PlayerStatusBar.Opacity.Equals(1.0))
            {
                // set UI on inactivity

                #region Fade in PlayerStatusBar opacity

                var opacityAnimation = new DoubleAnimationUsingKeyFrames();
                opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                var opacityEasingFunction = new PowerEase();
                opacityEasingFunction.EasingMode = EasingMode.EaseInOut;
                var startOpacityEasing = new EasingDoubleKeyFrame(1.0, KeyTime.FromPercent(0));
                var endOpacityEasing = new EasingDoubleKeyFrame(0.0, KeyTime.FromPercent(1.0),
                    opacityEasingFunction);
                opacityAnimation.KeyFrames.Add(startOpacityEasing);
                opacityAnimation.KeyFrames.Add(endOpacityEasing);

                PlayerStatusBar.BeginAnimation(OpacityProperty, opacityAnimation);

                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    await Task.Delay(500);
                    PlayerStatusBar.Visibility = Visibility.Collapsed;
                });

                #endregion
            }
        }

        #endregion

        #region Method -> OnActivity

        /// <summary>
        /// Show the PlayerStatusBar on mouse activity 
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnActivity(object sender, PreProcessInputEventArgs e)
        {
            var inputEventArgs = e.StagingItem.Input;

            if (inputEventArgs is MouseEventArgs || inputEventArgs is KeyboardEventArgs)
            {
                if (e.StagingItem.Input is MouseEventArgs)
                {
                    var mouseEventArgs = (MouseEventArgs) e.StagingItem.Input;

                    // no button is pressed and the position is still the same as the application became inactive
                    if (mouseEventArgs.LeftButton == MouseButtonState.Released &&
                        mouseEventArgs.RightButton == MouseButtonState.Released &&
                        mouseEventArgs.MiddleButton == MouseButtonState.Released &&
                        mouseEventArgs.XButton1 == MouseButtonState.Released &&
                        mouseEventArgs.XButton2 == MouseButtonState.Released &&
                        _inactiveMousePosition == mouseEventArgs.GetPosition(Container))
                        return;
                }

                if (PlayerStatusBar.Opacity.Equals(0.0))
                {
                    // set UI on activity

                    #region Fade out PlayerStatusBar opacity

                    var opacityAnimation = new DoubleAnimationUsingKeyFrames();
                    opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                    var opacityEasingFunction = new PowerEase();
                    opacityEasingFunction.EasingMode = EasingMode.EaseInOut;
                    var startOpacityEasing = new EasingDoubleKeyFrame(0.0, KeyTime.FromPercent(0));
                    var endOpacityEasing = new EasingDoubleKeyFrame(1.0, KeyTime.FromPercent(1.0),
                        opacityEasingFunction);
                    opacityAnimation.KeyFrames.Add(startOpacityEasing);
                    opacityAnimation.KeyFrames.Add(endOpacityEasing);

                    PlayerStatusBar.BeginAnimation(OpacityProperty, opacityAnimation);

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        PlayerStatusBar.Visibility = Visibility.Visible;
                    });

                    #endregion
                }

                _activityTimer.Stop();
                _activityTimer.Start();
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Free resources
        /// </summary>
        private void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                Loaded -= OnLoaded;
                Unloaded -= OnUnloaded;

                MediaPlayerTimer.Tick -= MediaPlayerTimer_Tick;
                MediaPlayerTimer.Stop();

                InputManager.Current.PreProcessInput -= OnActivity;
                _activityTimer.Tick -= OnInactivity;
                _activityTimer.Stop();

                Player.VlcMediaPlayer.EndReached -= MediaPlayer_EndReached;
                MediaPlayerIsPlaying = false;

                await Player.StopAsync();
                Player.Dispose();

                var vm = DataContext as MoviePlayerViewModel;
                if (vm != null)
                {
                    vm.StoppedPlayingMovie -= OnStoppedPlayingMovie;
                }

                _disposed = true;

                GC.SuppressFinalize(this);
            });
        }

        #endregion
    }
}