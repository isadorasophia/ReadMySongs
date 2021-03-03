using AsyncSongsUWP.ReadSongs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AsyncSongsUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string DefaultPlaylistTextBoxContent = "Playlist name goes here...";
        private const string DefaultLyricsTextBoxContent = "Lyrics go here...";
        private const string SongLabelPrefix = "Song Name:";

        public MainPage()
        {
            this.InitializeComponent();

            this.HideProgressElements();
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

                    Dispatcher.RunAsync(priority: Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        songLabel.Text = song != null ? $"{SongLabelPrefix} {song.Name}." :
                            $"{SongLabelPrefix} Not found.";

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
            progressLabel.Visibility = Visibility.Collapsed;
            searchProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowProgressElements()
        {
            progressLabel.Visibility = Visibility.Visible;
            searchProgressBar.Visibility = Visibility.Visible;
        }
    }
}
