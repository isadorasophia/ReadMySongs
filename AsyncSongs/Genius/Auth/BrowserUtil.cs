using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AsyncSongs.Genius
{
    public static class BrowserUtil
    {
        public static void Open(Uri uri)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var uriStr = uri.ToString().Replace("&", "^&");
                Process.Start(new ProcessStartInfo($"cmd", $"/c start {uriStr}"));

                return;
            }

            Debug.Fail("Unsupported OS.");
        }
    }
}
