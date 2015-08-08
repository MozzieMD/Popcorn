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
    /// Interaction logic for MoviePlayer.xaml
    /// </summary>
    public class MediaPlayer : UserControl
    {
        #region Properties

        protected bool Disposed;

        #region Property -> MediaPlayerIsPlaying

        /// <summary>
        /// Indicates if a movie is playing 
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
        /// Do action when loaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void OnLoaded(object sender, EventArgs e)
        {
        }

        #endregion

        #region Method -> OnUnloaded

        /// <summary>
        /// Do action when unloaded
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected void OnUnloaded(object sender, EventArgs e)
        {
            PauseMedia();
        }

        #endregion

        #region Method -> MediaPlayer_EndReached

        /// <summary>
        /// When a media has been fully played, save Seen property into database
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void MediaPlayer_EndReached(object sender, EventArgs e)
        {
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
            var moviePlayer = obj as MediaPlayer;
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
        protected virtual void ChangeMediaVolume(int newValue)
        {
        }

        #endregion

        #region Method -> OnStoppedPlayingMedia

        /// <summary>
        /// When media has finished playing, stop player and reset timer
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected void OnStoppedPlayingMedia(object sender, EventArgs e)
        {
            Dispose();
        }

        #endregion

        #region Method -> PlayMedia

        /// <summary>
        /// Play the movie
        /// </summary>
        protected virtual void PlayMedia()
        {
        }

        #endregion

        #region Method -> PauseMedia

        /// <summary>
        /// Pause the movie
        /// </summary>
        protected virtual void PauseMedia()
        {
        }

        #endregion

        #region Method -> MediaPlayerTimer_Tick

        /// <summary>
        /// Report the playing progress on the timeline
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void MediaPlayerTimer_Tick(object sender, EventArgs e)
        {
        }

        #endregion

        #region Method -> MediaPlayerPlay_CanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        protected virtual void MediaPlayerPlay_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        #endregion

        #region Method -> MediaPlayerPlay_Executed

        /// <summary>
        /// Play the current movie
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">ExecutedRoutedEventArgs</param>
        protected void MediaPlayerPlay_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PlayMedia();
        }

        #endregion

        #region Method -> MediaPlayerPause_CanExecute

        /// <summary>
        /// Each time the CanExecute play command change, update the visibility of Play/Pause buttons in the media player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        protected virtual void MediaPlayerPause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        #endregion

        #region Method -> MediaPlayerPause_Executed

        /// <summary>
        /// Pause the media
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">CanExecuteRoutedEventArgs</param>
        protected void MediaPlayerPause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PauseMedia();
        }

        #endregion

        #region Method -> MediaSliderProgress_DragStarted

        /// <summary>
        /// Report when dragging is used on media player
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DragStartedEventArgs</param>
        protected void MediaSliderProgress_DragStarted(object sender, DragStartedEventArgs e)
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
        protected virtual void MediaSliderProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
        }

        #endregion

        #region Method -> MovieSliderProgress_ValueChanged

        /// <summary>
        /// Report runtime when movie player progress changed
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">RoutedPropertyChangedEventArgs</param>
        protected virtual void MediaSliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
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

        #region Method -> OnInactivity

        /// <summary>
        /// Hide the PlayerStatusBar on mouse inactivity 
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void OnInactivity(object sender, EventArgs e)
        {
        }

        #endregion

        #region Method -> OnActivity

        /// <summary>
        /// Show the PlayerStatusBar on mouse activity 
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        protected virtual void OnActivity(object sender, PreProcessInputEventArgs e)
        {
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Free resources
        /// </summary>
        protected virtual void Dispose()
        {
        }

        #endregion
    }
}