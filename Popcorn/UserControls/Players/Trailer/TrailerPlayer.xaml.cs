using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using xZune.Vlc.Interop.Media;
using Popcorn.ViewModel.Players.Trailer;

namespace Popcorn.UserControls.Players.Trailer
{
    /// <summary>
    /// Interaction logic for MoviePlayer.xaml
    /// </summary>
    public partial class TrailerPlayer : IDisposable
    {
        #region Property -> Volume

        /// <summary>
        /// Get or set the media volume 
        /// </summary>
        public int Volume
        {
            get { return (int) GetValue(VolumeProperty); }

            set { SetValue(VolumeProperty, value); }
        }

        #endregion

        #region DependencyProperty -> VolumeProperty

        /// <summary>
        /// Identifies the <see cref="Volume"/> dependency property. 
        /// </summary>
        internal static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume", typeof (int),
            typeof (TrailerPlayer), new PropertyMetadata(100, OnVolumeChanged));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the MoviePlayer class.
        /// </summary>
        public TrailerPlayer()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #endregion

        #region Method -> Onloaded

        /// <summary>
        /// Subscribe to events and play the movie when control has been loaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnLoaded(object sender, EventArgs e)
        {
            if (Player.State == MediaState.Paused)
            {
                PlayMedia();
            }
            else
            {
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Closing += (s1, e1) => Dispose();
                }

                var vm = DataContext as TrailerPlayerViewModel;
                if (vm == null)
                    return;

                if (vm.MediaUri == null)
                    return;

                // start the timer used to report time on MediaPlayerSliderProgress
                MediaPlayerTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
                MediaPlayerTimer.Tick += MediaPlayerTimerTick;
                MediaPlayerTimer.Start();

                // start the activity timer used to manage visibility of the PlayerStatusBar
                ActivityTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(3)};
                ActivityTimer.Tick += OnInactivity;
                ActivityTimer.Start();

                InputManager.Current.PreProcessInput += OnActivity;

                vm.StoppedPlayingMedia += OnStoppedPlayingMedia;
                Player.VlcMediaPlayer.EndReached += MediaPlayerEndReached;

                Player.LoadMedia(vm.MediaUri);
                PlayMedia();
            }
        }

        #endregion

        #region Method -> OnUnloaded

        /// <summary>
        /// Pause media when control has been unloaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnUnloaded(object sender, EventArgs e)
        {
            PauseMedia();
        }

        #endregion

        #region Method -> OnVolumeChanged

        /// <summary>
        /// When media's volume changed, update volume
        /// </summary>
        /// <param name="e">e</param>
        /// <param name="obj">obj</param>
        private static void OnVolumeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var moviePlayer = obj as TrailerPlayer;
            if (moviePlayer == null)
                return;

            var newVolume = (int) e.NewValue;
            moviePlayer.ChangeMediaVolume(newVolume);
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

        #region Method -> MediaPlayerEndReached

        /// <summary>
        /// When a movie has been fully played, save seen property into database and send a StopPlayingMovieMessage message
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void MediaPlayerEndReached(object sender, EventArgs e)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var vm = DataContext as TrailerPlayerViewModel;
                if (vm == null)
                    return;

                vm.StopPlayingMediaCommand.Execute(null);
            });
        }

        #endregion

        #region Method -> PlayMedia

        /// <summary>
        /// Play the movie
        /// </summary>
        private void PlayMedia()
        {
            Player.Play();
            MediaPlayerIsPlaying = true;

            MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
            MediaPlayerStatusBarItemPause.Visibility = Visibility.Visible;
        }

        #endregion

        #region Method -> PauseMedia

        /// <summary>
        /// Pause the movie
        /// </summary>
        private void PauseMedia()
        {
            Player.PauseOrResume();
            MediaPlayerIsPlaying = false;

            MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
            MediaPlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Method -> OnStoppedPlayingMedia

        /// <summary>
        /// When media has finished playing, stop player and dispose control
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void OnStoppedPlayingMedia(object sender, EventArgs e)
        {
            Dispose();
        }

        #endregion

        #region Method -> MediaPlayerTimerTick

        /// <summary>
        /// Report the playing progress on the timeline
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        private void MediaPlayerTimerTick(object sender, EventArgs e)
        {
            if ((Player != null) && (!UserIsDraggingMediaPlayerSlider))
            {
                MediaPlayerSliderProgress.Minimum = 0;
                MediaPlayerSliderProgress.Maximum = Player.Length.TotalSeconds;
                MediaPlayerSliderProgress.Value = Player.Time.TotalSeconds;
            }
        }

        #endregion

        #region Method -> MediaPlayerPlayCanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        private void MediaPlayerPlayCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MediaPlayerStatusBarItemPlay != null && MediaPlayerStatusBarItemPause != null)
            {
                e.CanExecute = Player != null;
                if (MediaPlayerIsPlaying)
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
                    MediaPlayerStatusBarItemPause.Visibility = Visibility.Visible;
                }
                else
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
                    MediaPlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Method -> MediaPlayerPauseCanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the media player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        private void MediaPlayerPauseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (MediaPlayerStatusBarItemPlay != null && MediaPlayerStatusBarItemPause != null)
            {
                e.CanExecute = MediaPlayerIsPlaying;
                if (MediaPlayerIsPlaying)
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Collapsed;
                    MediaPlayerStatusBarItemPause.Visibility = Visibility.Visible;
                }
                else
                {
                    MediaPlayerStatusBarItemPlay.Visibility = Visibility.Visible;
                    MediaPlayerStatusBarItemPause.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Method -> MediaSliderProgressDragCompleted

        /// <summary>
        /// Report when user has finished dragging the media player progress
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DragCompletedEventArgs</param>
        private void MediaSliderProgressDragCompleted(object sender, DragCompletedEventArgs e)
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
        private void MediaSliderProgressValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MoviePlayerTextProgressStatus.Text =
                TimeSpan.FromSeconds(MediaPlayerSliderProgress.Value)
                    .ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture) + " / " +
                TimeSpan.FromSeconds(Player.Length.TotalSeconds)
                    .ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
        }

        #endregion

        #region Method -> MediaPlayerPlayExecuted

        /// <summary>
        /// Play media
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">ExecutedRoutedEventArgs</param>
        private void MediaPlayerPlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PlayMedia();
        }

        #endregion

        #region Method -> MediaPlayerPauseExecuted

        /// <summary>
        /// Pause media
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        private void MediaPlayerPauseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PauseMedia();
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
            InactiveMousePosition = Mouse.GetPosition(Container);

            if (!PlayerStatusBar.Opacity.Equals(1.0))
                return;

            var opacityAnimation = new DoubleAnimationUsingKeyFrames
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                KeyFrames = new DoubleKeyFrameCollection
                {
                    new EasingDoubleKeyFrame(1.0, KeyTime.FromPercent(0)),
                    new EasingDoubleKeyFrame(0.0, KeyTime.FromPercent(1.0), new PowerEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    })
                }
            };

            PlayerStatusBar.BeginAnimation(OpacityProperty, opacityAnimation);
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                await Task.Delay(500);
                PlayerStatusBar.Visibility = Visibility.Collapsed;
            });
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
                        InactiveMousePosition == mouseEventArgs.GetPosition(Container))
                        return;
                }

                if (!PlayerStatusBar.Opacity.Equals(0.0))
                    return;

                var opacityAnimation = new DoubleAnimationUsingKeyFrames
                {
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    KeyFrames = new DoubleKeyFrameCollection
                    {
                        new EasingDoubleKeyFrame(0.0, KeyTime.FromPercent(0)),
                        new EasingDoubleKeyFrame(1.0, KeyTime.FromPercent(1.0), new PowerEase
                        {
                            EasingMode = EasingMode.EaseInOut
                        })
                    }
                };

                PlayerStatusBar.BeginAnimation(OpacityProperty, opacityAnimation);
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    PlayerStatusBar.Visibility = Visibility.Visible;
                });
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Free resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                Loaded -= OnLoaded;
                Unloaded -= OnUnloaded;

                MediaPlayerTimer.Tick -= MediaPlayerTimerTick;
                MediaPlayerTimer.Stop();

                ActivityTimer.Tick -= OnInactivity;
                ActivityTimer.Stop();

                InputManager.Current.PreProcessInput -= OnActivity;

                Player.VlcMediaPlayer.EndReached -= MediaPlayerEndReached;
                MediaPlayerIsPlaying = false;

                await Player.StopAsync();
                Player.Dispose();

                var vm = DataContext as TrailerPlayerViewModel;
                if (vm != null)
                {
                    vm.StoppedPlayingMedia -= OnStoppedPlayingMedia;
                }

                Disposed = true;

                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            });
        }

        #endregion
    }
}