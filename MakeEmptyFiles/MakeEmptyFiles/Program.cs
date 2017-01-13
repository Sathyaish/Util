using System;
using System.IO;
using System.Linq;

namespace MakeEmptyFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            var len = args?.Length;

            if (len == null || len.Value < 2)
            {
                PrintHelp();
                return;
            }

            var path = args[0];
            var destination = args[1];

            if (!Directory.Exists(path))
            {
                Console.WriteLine("Source location does not exist or is not a folder / directory.");
                return;
            }

            if (!Directory.Exists(destination))
            {
                Console.WriteLine("Destination not a folder / directory.");
                return;
            }

            var fileInfos = from file in Directory.GetFiles(path)
                             select new FileInfo(file);

            var count = 0;
            foreach (var info in fileInfos)
            {
                count++;

                var fileName = info.Name;
                var fullNameOfNewFile = Path.Combine(destination, fileName);

                File.Create(fullNameOfNewFile);
            }

            Console.WriteLine($"\n\n{count} files created in '{destination}'.");
        }

        private static void PrintHelp()
        {
            Console.WriteLine("This program makes empty files in a destination directory based on file names \nof existing files in a source directory.");
            Console.WriteLine("\nUSAGE: Empty <SourceDirectory> <DestinationDirectory>\n");
        }
    }
}