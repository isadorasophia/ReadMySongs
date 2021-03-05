using AsyncSongs.Spotify;
using AsyncSongs.ReadSongs;

using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AsyncSongs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DefaultPlaylistTextBoxContent = "Playlist name goes here...";
        private const string DefaultLyricsTextBoxContent = "Lyrics go here...";
        private const string SongLabelPrefix = "Song Name:";

        public MainWindow()
        {
            InitializeComponent();
            
            HideProgressElements();

            InitializeSpotify();
        }

        private void InitializeSpotify()
        {
            // Listen to any logins at the spotify wrapper.
            SpotifyRequests.Instance.OnLogin(SetUserAsync);
            _ = Task.Run(SpotifyRequests.Instance.InitializeAsync);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchButton.IsEnabled = false;

            if (lyricsTextBox.Text == DefaultLyricsTextBoxContent || lyricsTextBox.Text == string.Empty)
            {
                // error
            }
            else if (playlistTextBox.Text == DefaultPlaylistTextBoxContent || playlistTextBox.Text == string.Empty)
            {
                // error
            }
            else
            {
                ShowProgressElements();

                var text = lyricsTextBox.Text;
                var playlist = playlistTextBox.Text;

                var songSearchingTask = Task.Run(async delegate
                {
                    Song song = await ReadSongsService.SearchSong(text, playlist);

                    await Dispatcher.BeginInvoke(delegate
                    {
                        songLabel.Content = song != null ? $"{SongLabelPrefix} {song.Name}." :
                            $"{SongLabelPrefix} Not found.";

                        HideProgressElements();
                        searchButton.IsEnabled = true;
                    }, DispatcherPriority.Render);
                }).ConfigureAwait(false);
            }
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            bool requestSent = false;
            loginButton.IsEnabled = false;

            try
            {
                requestSent = await Task.Run(SpotifyRequests.Instance.LoginAsync);
            }
            catch
            {
                Debug.Fail("Login failed.");
            }

            loginButton.IsEnabled = !requestSent;
        }

        private void playlistTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox playlistTextBox && playlistTextBox.Text == DefaultPlaylistTextBoxContent)
            {
                playlistTextBox.Text = string.Empty;
            }
        }

        private void playlistTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox playlistTextBox && playlistTextBox.Text == string.Empty)
            {
                playlistTextBox.Text = DefaultPlaylistTextBoxContent;
            }
        }

        private void lyricsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox lyricsTextBox && lyricsTextBox.Text == DefaultLyricsTextBoxContent)
            {
                lyricsTextBox.Text = string.Empty;
            }
        }

        private void lyricsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox lyricsTextBox && lyricsTextBox.Text == string.Empty)
            {
                lyricsTextBox.Text = DefaultLyricsTextBoxContent;
            }
        }

        private void HideProgressElements()
        {
            progressLabel.Visibility = Visibility.Hidden;
            searchProgressBar.Visibility = Visibility.Hidden;
        }

        private void ShowProgressElements()
        {
            progressLabel.Visibility = Visibility.Visible;
            searchProgressBar.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Helper callback for setting a new user after a successful login.
        /// </summary>
        public async Task SetUserAsync()
        {
            string username = await SpotifyRequests.Instance.GetUsernameAsync().ConfigureAwait(false);

            Dispatcher.Invoke(delegate
            {
                userLabel.Content = string.Format("Welcome, {0}!", username);

                loginButton.IsEnabled = false;
                loginButton.Content = "Logged in";
            }, DispatcherPriority.Render);
        }
    }
}
