using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Fix
{
    class Program
    {
        // folder /p:
        // wildcardsearchpattern /w:
        // Search subdirectories also /s. Defaults to looking only in the top-level directory
        // Verbose /v
        // Log /l:C:\\Sathyaish\\temp\\fix.log or just /l (in the same directory as fix-timestamp.log)
        // Preview and ask for a confirmation before making changes /ask
        // Preview -- just show me the changes that will be made without actually making them /preview
        static void Main(string[] args)
        {
            var len = args?.Length;

            if (len == null || len.Value == 0)
            {
                PrintHelp();
                return;
            }

            var path = args[0];

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Invalid argument. '{path}' is not a valid folder / directory.");
                return;
            }

            var files = Directory.GetFiles(path, "*.mp4", SearchOption.TopDirectoryOnly);

            var matches = 0;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var fileNameWithExtension = fileInfo.Name;

                var pattern = @".*?(?<Part>(\s*?\-*?\s*?[Pp][Aa][Rr][Tt]\s*?)(?<Number>\d{1,}))+?";
                var match = Regex.Match(fileNameWithExtension, pattern);
                
                if (match.Success)
                {
                    matches++;

                    Console.WriteLine(fileNameWithExtension);

                    var newFileNameWithExtension = Regex.Replace(fileNameWithExtension, pattern, m =>
                    {
                        if (m.Groups.Count > 0)
                        {
                            var partValue = m.Groups["Part"]?.Value;
                            var numberValue = m.Groups["Number"]?.Value;
                            var number = int.Parse(numberValue);

                            return fileNameWithExtension
                                .Replace(partValue, string.Empty)
                                .Insert(0, string.Format($"{number.ToString("D3")} - "));
                        }

                        return fileNameWithExtension;
                    });

                    Console.WriteLine(newFileNameWithExtension);
                    fileInfo.MoveTo(Path.Combine(fileInfo.DirectoryName, newFileNameWithExtension));
                    Console.WriteLine();
                }
            }

            Console.WriteLine($"\n{matches} matches affected.");
            Console.ReadKey();
        }

        private static void PrintHelp()
        {
            Console.WriteLine("This program numbers all existing MP4 files in a folder based on the numbers found in their present names.");
            Console.WriteLine("\nUSAGE: Fix <Path>\n\n");
        }
    }
}