using AsyncSongs.Spotify;
using AsyncSongs.Genius;
using AsyncSongs.ReadSongs;

using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Documents;

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

            InitializeApis();

            Utilities.CacheUtils.IsCacheInvalid = false;
        }

        private void InitializeApis()
        {
            // Listen to any logins at the spotify wrapper.
            SpotifyRequests.Instance.OnLogin(SetUserAsync);

            _ = Task.Run(SpotifyRequests.Instance.InitializeAsync);
            _ = Task.Run(GeniusRequests.Instance.InitializeAsync);
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
                _lyricsLabel.Text = "";
                _lyricsLabel.Visibility = Visibility.Hidden;
                ShowProgressElements();

                var text = lyricsTextBox.Text;
                var playlist = playlistTextBox.Text;

                var songSearchingTask = Task.Run(async delegate
                {
                    Song? song = await ReadSongsService.SearchSongAsync(text, playlist);

                    await Dispatcher.BeginInvoke(delegate
                    {
                        HideProgressElements();
                        searchButton.IsEnabled = true;

                        if (song is not null)
                        {
                            songLabel.Content = $"{SongLabelPrefix} {song.Name}.";

                            var lines = song.Lyrics.GetExerpt(text);
                            foreach (var line in lines)
                            {
                                _lyricsLabel.Inlines.Add(new Run(line) { FontStyle = FontStyles.Italic });
                            }

                            _lyricsLabel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            songLabel.Content = $"{SongLabelPrefix} Not found.";
                        }
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
                requestSent = await Task.Run(SpotifyRequests.Instance.LoginAsync) && 
                    await Task.Run(GeniusRequests.Instance.LoginAsync);
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
            string? username = await SpotifyRequests.Instance.GetUsernameAsync().ConfigureAwait(false);

            Dispatcher.Invoke(delegate
            {
                if (username is not null)
                {
                    userLabel.Content = string.Format("Welcome, {0}!", username);

                    loginButton.IsEnabled = false;
                    loginButton.Content = "Logged in";
                }
                else
                {
                    loginButton.IsEnabled = true;
                }
            }, DispatcherPriority.Render);
        }
    }
}
