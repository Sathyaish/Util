using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NumberAsPerYouTubePlaylist
{
    class Program
    {
        // EXE folder youtubePlaylistURL
        static void Main(string[] args)
        {
            try
            {
                var len = args?.Length;

                if (len == null || len < 2)
                {
                    PrintHelp();
                    return;
                }

                var path = args[0];
                var playlistURL = args[1];

                // Validate folder path
                if (!Directory.Exists(path))
                {
                    Console.WriteLine($"'{path}' is not a folder / directory.\n");
                    return;
                }

                // if folder has no files, tell him and go
                var files = Directory.GetFiles(path);

                if (files.Count() == 0)
                {
                    Console.WriteLine($"The folder / directory '{path}' is empty.\n");
                    return;
                }

                var playlistId = GetPlaylistId(playlistURL);

                // Validate youtube playlist
                if (string.IsNullOrEmpty(playlistId))
                {
                    Console.WriteLine($"'{playlistURL}' does not seem like a valid YouTube playlist URL. It could be that the playlist is private. This program requires that the playlist be public.\n");
                    return;
                }

                var fileInfos = files.Select(f => new FileInfo(f));

                var videoTitles = GetVideoTitles(playlistId);

                var numVideosInPlaylist = videoTitles.Count;

                // if playlist has no files, tell him and go
                if (numVideosInPlaylist == 0)
                {
                    Console.WriteLine($"The YouTube playlist is empty.\n");
                    return;
                }

                var maxLen = numVideosInPlaylist.ToString().Length;
                var totalMatchCount = 0;
                var duplicateSets = 0;
                var duplicateVideoNames = 0;

                foreach (var info in fileInfos)
                {
                    var fileNameWithExtension = info.Name;

                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(info.FullName);

                    var matches = videoTitles.GetVideoTitlesOrDefault(fileNameWithoutExtension);

                    if (matches == null || matches.Count == 0)
                    {
                        continue;
                    }

                    if (matches.Count > 1)
                    {
                        duplicateSets++;
                        duplicateVideoNames += matches.Count;
                    }
                    else if (matches.Count == 1)
                    {
                        totalMatchCount++;

                        var currentTitle = matches[0];
                        var thisOnesLength = currentTitle.Order.ToString().Length;

                        var padLength = (maxLen == thisOnesLength ? 0 : (maxLen - thisOnesLength));
                        var padding = new String('0', padLength);

                        var newFileName = string.Concat(padding, currentTitle.Order, " - ", fileNameWithExtension);

                        var newFileNameWithPath = Path.Combine(info.DirectoryName, newFileName);

                        info.MoveTo(newFileName);

                        Console.WriteLine($"{fileNameWithExtension}\n{newFileName}\n");
                    }
                }
                if (totalMatchCount == 0)
                {
                    // if not even a single match found, tell him this doesn't look like from this playlist
                    Console.WriteLine($"None of the names of the files in the folder '{path}' match the titles of the videos in the YouTube playlist '{playlistURL}'.\n");
                    return;
                }

                Console.WriteLine();
                Console.WriteLine($"Total matches affected: {totalMatchCount}");
                Console.WriteLine($"Unaffected matches: {duplicateVideoNames} duplicates found in {duplicateSets} duplicate titles.");
                Console.WriteLine("\n");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        private static VideoTitles GetVideoTitles(string playlistId)
        {
            string nextPageToken = null;
            var videoTitles = new VideoTitles();
            var itemsCount = 0;
            Func<string, string> youTubeVideoUrlMaker = (id) => string.Concat("https://www.youtube.com/watch?v=", id);
            var counter = 0;

            do
            {
                var result = GetVideosInPlaylistAsync(playlistId, nextPageToken).Result;

                itemsCount += result.items.Count;
                nextPageToken = result.nextPageToken;

                if (itemsCount == 0) return videoTitles;

                foreach (var item in result.items)
                {
                    var videoTitle = new VideoTitle(item.snippet.title.ToString(),  
                        ++counter,
                        youTubeVideoUrlMaker, 
                        item.snippet.resourceId.videoId.ToString());

                    videoTitles.Add(videoTitle);
                }
            } while (nextPageToken != null);

            return videoTitles;
        }

        private static async Task<dynamic> GetVideosInPlaylistAsync(string playlistId, 
            string nextPageToken)
        {
            var parameters = new Dictionary<string, string>
            {
                ["key"] = ConfigurationManager.AppSettings["APIKey"],
                ["playlistId"] = playlistId,
                ["part"] = "snippet",
                ["fields"] = "nextPageToken, pageInfo, items/snippet(title, resourceId/videoId)",
                ["maxResults"] = "50"
            };

            if (!string.IsNullOrEmpty(nextPageToken))
                parameters.Add("pageToken", nextPageToken);

            var baseUrl = "https://www.googleapis.com/youtube/v3/playlistItems?";
            var fullUrl = MakeUrlWithQuery(baseUrl, parameters);

            var result = await new HttpClient().GetStringAsync(fullUrl);

            if (result != null)
            {return JsonConvert.DeserializeObject(result);
            }

            return default(dynamic);
        }

        private static string MakeUrlWithQuery(string baseUrl,
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (string.IsNullOrEmpty(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            if (parameters == null || parameters.Count() == 0)
                return baseUrl;

            return parameters.Aggregate(baseUrl,
                (accumulated, kvp) => string.Format($"{accumulated}{kvp.Key}={kvp.Value}&"));
        }

        private static string GetPlaylistId(string playlistURL)
        {
            ValidateUrl(playlistURL);

            return ExtractId(playlistURL);
        }

        private static void PrintHelp()
        {
            Console.WriteLine("This program numbers the files you downloaded previously from a YouTube playlist as per their order in the playlist.");
            Console.WriteLine("\nUSAGE: Order <Path> <YouTubePlaylistURL>\n\n");
        }

        private static void ValidateUrl(string url)
        {
            var uri = new Uri(url);
            if (!uri.Host.Equals("www.youtube.com", StringComparison.InvariantCultureIgnoreCase) ||
                !uri.IsAbsoluteUri ||
                uri.IsFile ||
                uri.IsLoopback ||
                uri.IsUnc)
            {
                throw new Exception(string.Format($"The Url '{url}' does not seem to be a valid YouTube playlist URL."));
            }
        }

        private static string ExtractId(string url)
        {
            var pattern = @"[Hh][Tt]{2}[Pp][Ss]?\:\/{2}[Ww]{3}\.[Yy][Oo][Uu][Tt][Uu][Bb][Ee]\.[Cc][Oo][Mm]\/[Pp][Ll][Aa][Yy][Ll][Ii][Ss][Tt]\?((\w|\d|&)*?)(?<PlaylistSegment>[Ll][Ii][Ss][Tt]\=(?<PlaylistId>[^&]+))&*?.*?";

            var match = Regex.Match(url, pattern);

            var value = match.Groups?["PlaylistId"]?.Value;

            return value;
        }
    }
}