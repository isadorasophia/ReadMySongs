using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private const string DefaultPlaylistTextBoxContent = "Playlist name goes here...";
        private const string DefaultLyricsTextBoxContent = "Lyrics go here...";
        private const string SongLabelPrefix = "Song Name:";

        private const string ServerURL = @"https://localhost:44353/ReadSongs";
        private readonly HttpClient httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            
            HideProgressElements();
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

                //var songSearchingTask = Task.Run(async delegate
                //{
                //    Song song = await ReadSongsService.SearchSong(text, playlist);

                //    Dispatcher.Invoke(() =>
                //    {
                //        songLabel.Content = song != null ? $"{SongLabelPrefix} {song.Name}." :
                //            $"{SongLabelPrefix} Not found.";

                //        HideProgressElements();
                //        searchButton.IsEnabled = true;
                //    });
                //}).ConfigureAwait(false);
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loginButton.IsEnabled = false;

            // Fire and forget...
            var t = Task.Run(async delegate
            {
                var result = await httpClient.GetAsync($"{ServerURL}/Login");
                loginButton.IsEnabled = true;
            });
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
    }
}
