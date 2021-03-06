using System;
using System.Linq;
using System.Net;
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
        private const string GeniusSongUrl = "https://genius.com{0}";
        private const string LyricsHtmlPath = "//div[@class='song_body column_layout']/div[@class='column_layout-column_span column_layout-column_span--primary']/div[@class='song_body-lyrics']/div[@initial-content-for='lyrics']/div[@class='lyrics']";

        /// <summary>
        /// In order to fetch the lyrics, we need to request for the html and manually fetch its contents. 
        /// </summary>
        internal static async Task<string?> FetchLyricsAsync(this GeniusSong song)
        {
            WebClient wb = new();
            string path = song.Path;

            var htmlContent = await wb.DownloadStringTaskAsync(new Uri(string.Format(GeniusSongUrl, path)));

            // Scrape the lyrics from the html element.
            HtmlDocument document = new();
            document.LoadHtml(htmlContent);
            
            // Look for our lyrics path and hope for the best...
            var nodeCollection = document.DocumentNode.SelectNodes(LyricsHtmlPath);
            string? lyricsHtmlContent = nodeCollection?.FirstOrDefault()?.InnerHtml;
            if (lyricsHtmlContent is null)
            {
                // Unable to find element, did it change? :(
                // TODO: Find out different paths to the lyrics.
                return null;
            }

            return FormatHtml(lyricsHtmlContent);
        }

        // See https://stackoverflow.com/questions/286813/how-do-you-convert-html-to-plain-text
        private static string FormatHtml(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";
            const string stripFormatting = @"<[^>]*(>|$)";
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";

            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;

            // Decode html specific characters
            text = WebUtility.HtmlDecode(text);

            // Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");

            // Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);

            // Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
    }
}
