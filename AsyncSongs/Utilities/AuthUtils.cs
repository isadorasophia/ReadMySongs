using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AsyncSongs.Utilities
{
    public static class AuthUtils
    {
        private const string SecretsFile = @"secrets.txt";
        
        /// <summary>
        /// Retrieve the root directory.
        /// </summary>
        public readonly static string BasePath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

        public static async Task<string> ReadSecretAsync(string key)
        {
            string content = await ReadFileAsync(SecretsFile);
            var arguments = content.Split("\r\n")
                .Select(param => param.Split("="))
                .Where(param => param.Length > 1)
                .ToDictionary(param => param[0], param => param[1]);

            return arguments[key];
        }

        public static async Task<string> ReadFileAsync(string file)
        {
            string path = Path.Combine(BasePath, file);

            if (!File.Exists(path))
            {
                Debug.Fail("How are you trying to access {0}!? It does not exist.", file);
                throw new InvalidOperationException();
            }

            return await File.ReadAllTextAsync(path);
        }
    }
}
