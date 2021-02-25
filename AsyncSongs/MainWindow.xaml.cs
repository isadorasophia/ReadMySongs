using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AsyncSongs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string defaultPlaylistTextBoxContent = "Playlist name goes here...";
        const string defaultLyricsTextBoxContent = "Lyrics go here...";
        const string songLabelPrefix = "Song Name:";

        public MainWindow()
        {
            InitializeComponent();
            
            HideProgressElements();
        }

        private async Task<Song> SearchSongAsync(string text, string playlistName = null)
        {
            List<Playlist> playlists = new();
            if (playlistName is null)
            {
                // Search through all playlists instead.
                // playlists = await FetchPlaylists();
            }
            else
            {
                playlists.Add(new(playlistName));
            }

            List<Task<Song>> tasks = new();
            foreach (Playlist playlist in playlists)
            {
                tasks.Add(playlist.TryFindSongAsync(text));
            }

            Song[] songs = await Task.WhenAll(tasks);
            return songs.FirstOrDefault(s => s is not null);
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchButton.IsEnabled = false;

            if (lyricsTextBox.Text == defaultLyricsTextBoxContent || lyricsTextBox.Text == string.Empty)
            {
                // error
            }
            else if (playlistTextBox.Text == defaultPlaylistTextBoxContent || playlistTextBox.Text == string.Empty)
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
                    Song song = await SearchSongAsync(text, playlist);

                    if (song != null)
                    {
                        Dispatcher.Invoke(() => 
                        { 
                            songLabel.Content = $"{songLabelPrefix} {song.Name}."; 
                        });
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            songLabel.Content = $"{songLabelPrefix} Not found.";
                        });
                    }

                    Dispatcher.Invoke(() =>
                    {
                        HideProgressElements();
                        searchButton.IsEnabled = true;
                    });
                }).ConfigureAwait(false);
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loginButton.IsEnabled = false;

            // Fire and forget...
            var t = Task.Run(async delegate
            {
                await Task.Delay(3000);
                loginButton.IsEnabled = true;
            });
        }

        private void playlistTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox playlistTextBox && playlistTextBox.Text == defaultPlaylistTextBoxContent)
            {
                playlistTextBox.Text = string.Empty;
            }
        }

        private void playlistTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox playlistTextBox && playlistTextBox.Text == string.Empty)
            {
                playlistTextBox.Text = defaultPlaylistTextBoxContent;
            }
        }

        private void lyricsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox lyricsTextBox && lyricsTextBox.Text == defaultLyricsTextBoxContent)
            {
                lyricsTextBox.Text = string.Empty;
            }
        }

        private void lyricsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox lyricsTextBox && lyricsTextBox.Text == string.Empty)
            {
                lyricsTextBox.Text = defaultLyricsTextBoxContent;
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
    }
}
