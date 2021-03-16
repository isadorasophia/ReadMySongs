using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

using GeniusSong = Genius.Models.Song.Song;

namespace AsyncSongs.Genius
{
    /// <summary>
    /// Scrape the song lyrics from Genius html website.
    /// </summary>
    internal static class LyricsFetcher
    {
        /// <summary>
        /// In order to fetch the lyrics, we need to request for the html and manually fetch its contents. 
        /// </summary>
        internal static async Task<string?> FetchLyricsAsync(this GeniusSong song)
        {
            // TODO: This might have an extra /, remove it if so.
            string uri = $"https://localhost:44305/api/lyrics?songPath={song.Path}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
