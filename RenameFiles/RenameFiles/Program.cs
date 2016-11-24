using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RenameFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            RemovePrefix(@"[Mm][Aa][Hh][Aa][Bb][Hh][Aa][Rr][Aa][Tt][Aa]?\s+[Ee][Pp][Ii]*[Ss][Oo]*[Dd]*[Ee]*\s*\-*\s*\-*", 
                @"C:\Users\computer\Videos\4K Video Downloader\Mahabharat - With Good Sound", 
                @"[Mm][Aa][Hh][Aa][Bb][Hh][Aa][Rr][Aa][Tt][Aa]?\s+[Ee][Pp][Ii]*[Ss][Oo]*[Dd]*[Ee]*\s*\-*\s*\-*.*\.[Mm][Pp]4");
        }

        static void RemovePrefix(string prefixToRemove, string parentFolder, string pattern, bool verbose = true)
        {
            var files = Directory.GetFiles(parentFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => Regex.IsMatch(new FileInfo(f).Name, prefixToRemove))
                .Select(f => new FileInfo(f));

            if (files == null) return;

            var count = files.Count();

            if (verbose) Console.WriteLine($"{count} matching files found.");

            var n = 0;

            try
            {
                foreach (var file in files)
                {
                    var newFileName = Regex.Replace(file.Name, prefixToRemove, string.Empty);
                    newFileName = newFileName.Replace(" with English Subtitles", string.Empty);
                    var newFullName = Path.Combine(file.Directory.FullName, newFileName);

                    if (verbose)
                    {
                        Console.WriteLine($"Renaming \"{file.Name}\" to \"{newFileName}\"");
                        Debug.Print(string.Format($"Renamed \"{file.Name}\" to \"{newFullName}\"."));
                    }

                    file.MoveTo(newFullName);
                    n++;
                }

                if (verbose) Console.WriteLine($"{n} matching files renamed.");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }
        }
    }
}