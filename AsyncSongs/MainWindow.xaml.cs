using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncSongs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly JoinableTaskContext joinableTaskContext = new JoinableTaskContext();
        private readonly JoinableTaskFactory joinableTaskFactory;
        private readonly JoinableTaskCollection joinableTaskCollection;

        const string defaultPlaylistTextBoxContent = "Playlist name goes here...";
        const string defaultLyricsTextBoxContent = "Lyrics go here...";
        const string songLabelPrefix = "Song Name:";

        public MainWindow()
        {
            InitializeComponent();
            
            HideProgressElements();
            
            this.joinableTaskCollection = this.joinableTaskContext.CreateCollection();
            this.joinableTaskFactory = this.joinableTaskContext.CreateFactory(this.joinableTaskCollection);
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

                Playlist playlist = new(playlistTextBox.Text);

                this.joinableTaskFactory.RunAsync(async delegate
                {
                    Song song = await playlist.TryFindSong(lyricsTextBox.Text);

                    if (song != null)
                    {
                        Lyrics lyrics = await song.FetchLyrics();

                        songLabel.Content = $"{songLabelPrefix} {song.Name}.";
                    }
                    else
                    {
                        songLabel.Content = $"{songLabelPrefix} Not found.";
                    }

                    HideProgressElements();
                    searchButton.IsEnabled = true;
                });
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            loginButton.IsEnabled = false;

            this.joinableTaskFactory.RunAsync(async delegate
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
