using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StripFromFileName
{
    class Program
    {
        static void Main(string[] args)
        {
            var query = from file in Directory.GetFiles(@"C:\Sathyaish\temp\Edit\Vidit's Birthday\Video\Nikon D3100", "*.MOV", SearchOption.TopDirectoryOnly)
                        where new FileInfo(file).Name.StartsWith("DSC_")
                        select file;

            int count = query.Count();
            int counter = 0;

            if (count > 0)
            {
                foreach (var filePath in query)
                {
                    var info = new FileInfo(filePath);
                    counter++;

                    var newFileName = info.Name.Substring("DSC_".Length);
                    var newFilePath = Path.Combine(info.DirectoryName, newFileName);

                    Console.WriteLine($"Renaming {info.Name} to {newFileName}");
                    File.Move(filePath, newFilePath);
                }

                Debug.Assert(count == counter);

                Console.WriteLine($"\n{counter} files renamed.");
            }
            else
            {
                Console.WriteLine("No matching files found.");
            }

            Console.ReadKey();       
        }
    }
}
