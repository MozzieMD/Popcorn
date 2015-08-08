using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using Popcorn.UserControls.Players.Movie;

namespace Popcorn.UserControls.Players
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    public class MediaPlayer : UserControl
    {
        #region Properties

        protected bool Disposed;

        #region Property -> MediaPlayerIsPlaying

        /// <summary>
        /// Indicates if a media is playing 
        /// </summary>
        protected bool MediaPlayerIsPlaying { get; set; }

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
        /// Get or set the media volume 
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
        protected DispatcherTimer ActivityTimer { get; set; }

        protected Point InactiveMousePosition { get; set; } = new Point(0, 0);

        #endregion

        #region Property -> UserIsDraggingMediaPlayerSlider

        /// <summary>
        /// Indicate if user is manipulating the timeline player
        /// </summary>
        protected bool UserIsDraggingMediaPlayerSlider { get; set; }

        #endregion

        #region Property -> MediaPlayerTimer

        /// <summary>
        /// Timer used for report time on the timeline
        /// </summary>
        protected DispatcherTimer MediaPlayerTimer { get; set; }

        #endregion

        #endregion

        #region Method -> Onloaded

        /// <summary>
        /// Subscribe to events and play the media when control has been loaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void OnLoaded(object sender, EventArgs e)
        {
        }

        #endregion

        #region Method -> OnUnloaded

        /// <summary>
        /// Pause media when control has been unloaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected void OnUnloaded(object sender, EventArgs e)
        {
            PauseMedia();
        }

        #endregion

        #region Method -> MediaPlayerEndReached

        /// <summary>
        /// When a media has been fully played
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void MediaPlayerEndReached(object sender, EventArgs e)
        {
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
            var moviePlayer = obj as MediaPlayer;
            if (moviePlayer == null)
                return;

            var newVolume = (int) e.NewValue;
            moviePlayer.ChangeMediaVolume(newVolume);
        }

        #endregion

        #region Method -> ChangeMediaVolume

        /// <summary>
        /// Change the media volume
        /// </summary>
        /// <param name="newValue">New volume value</param>
        protected virtual void ChangeMediaVolume(int newValue)
        {
        }

        #endregion



        #region Method -> PlayMedia

        /// <summary>
        /// Play the media
        /// </summary>
        protected virtual void PlayMedia()
        {
        }

        #endregion

        #region Method -> PauseMedia

        /// <summary>
        /// Pause the media
        /// </summary>
        protected virtual void PauseMedia()
        {
        }

        #endregion

        #region Method -> MediaPlayerTimerTick

        /// <summary>
        /// Report the playing progress on the timeline
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void MediaPlayerTimerTick(object sender, EventArgs e)
        {
        }

        #endregion

        #region Method -> MediaPlayerPlayCanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        protected virtual void MediaPlayerPlayCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        #endregion

        #region Method -> MediaPlayerPlayExecuted

        /// <summary>
        /// Play media
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">ExecutedRoutedEventArgs</param>
        protected void MediaPlayerPlayExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PlayMedia();
        }

        #endregion

        #region Method -> MediaPlayerPauseCanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the media player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        protected virtual void MediaPlayerPauseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        #endregion

        #region Method -> MediaPlayerPauseExecuted

        /// <summary>
        /// Pause media
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        protected void MediaPlayerPauseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PauseMedia();
        }

        #endregion

        #region Method -> MediaSliderProgressDragStarted

        /// <summary>
        /// Report when dragging is used on media player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DragStartedEventArgs</param>
        protected void MediaSliderProgressDragStarted(object sender, DragStartedEventArgs e)
        {
            UserIsDraggingMediaPlayerSlider = true;
        }

        #endregion

        #region Method -> MediaSliderProgressDragCompleted

        /// <summary>
        /// Report when user has finished dragging the media player progress
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DragCompletedEventArgs</param>
        protected virtual void MediaSliderProgressDragCompleted(object sender, DragCompletedEventArgs e)
        {
        }

        #endregion

        #region Method -> MediaSliderProgressValueChanged

        /// <summary>
        /// Report runtime when media player slider's progress has changed
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">RoutedPropertyChangedEventArgs</param>
        protected virtual void MediaSliderProgressValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        #endregion

        #region Method -> MouseWheelMediaPlayer

        /// <summary>
        /// When user uses the mousewheel, update the volume
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">MouseWheelEventArgs</param>
        protected void MouseWheelMediaPlayer(object sender, MouseWheelEventArgs e)
        {
            if ((Volume <= 190 && e.Delta > 0) || (Volume >= 10 && e.Delta < 0))
            {
                Volume += (e.Delta > 0) ? 10 : -10;
            }
        }

        #endregion
    }
}