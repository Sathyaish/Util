using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ReflectionPackageStore
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var path = @"C:\Sathyaish\temp\packages";

                var allDlls = new DirectoryInfo(path).GetFiles("*.dll", SearchOption.AllDirectories);

                var list = allDlls.Where(IsNet45Assembly)
                    .ToList();
                
                if (!Directory.Exists("C:\\Sathyaish\\temp\\Reflect"))
                {
                    Directory.CreateDirectory("C:\\Sathyaish\\temp\\Reflect");
                }

                list.ForEach(dll =>
                {
                    var fileName = Path.Combine("C:\\Sathyaish\\temp\\Reflect", dll.Name);
                    dll.CopyTo(fileName, true);
                });

                Console.WriteLine($"\n\n{list.Count} files copied.");

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        public static bool IsNet45Assembly(FileInfo dll)
        {
            Console.WriteLine($"{dll.Directory.Name}\\{dll.Name}");
            return dll.Directory.Name.Equals("net45");
        }
    }
}
