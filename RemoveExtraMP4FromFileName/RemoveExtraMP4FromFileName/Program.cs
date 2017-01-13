using System;
using System.IO;
using System.Linq;

namespace RemoveExtraMP4FromFileName
{
    class Program
    {
        static void Main(string[] args)
        {
            var fileInfos = from file in Directory.GetFiles(@"C:\Sathyaish\temp\Videos\Tech\For job\SQL\SQL Server Tutorial for Beginners -- Kudvenkat")
                        where file.EndsWith(".mp4.mp4")
                        select new FileInfo(file);

            var count = 0;

            foreach(var info in fileInfos)
            {
                count++;

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(info.Name);

                info.MoveTo(fileNameWithoutExtension);
            }

            Console.WriteLine($"{count} files renamed.");
            Console.ReadKey();
        }
    }
}
