using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RemovePEFiles
{
    class Program
    {
        private static List<string> directoryNames = new List<string>()
        {
            "bin", "obj", "packages"
        };

        // RemovePEFiles -v
        // RemovePEFiles "C:\Sathyaish\Practice" -v
        // RemovePEFiles -v "C:\Sathyaish\Practice"
        // RemovePEFiles "C:\Sathyaish\Practice"
        static void Main(string[] args)
        {
            try
            {
                PrintHeader();

                if (args.Length == 0)
                {
                    throw new InvalidOperationException("Incorrect usage. You must provide the directory path from where to delete PE files.");
                }

                var verboseModeArg = args?.FirstOrDefault(arg => arg.Equals("-v", StringComparison.InvariantCultureIgnoreCase));

                if (verboseModeArg != null && args.Length == 1)
                {
                    throw new InvalidOperationException("Incorrect usage. You must provide the directory path from where to delete PE files.");
                }

                bool verbose = verboseModeArg != null;
                var path = args?.FirstOrDefault(arg => !arg.Equals("-v", StringComparison.InvariantCultureIgnoreCase));
                
                var directoryInfo = new DirectoryInfo(path);

                var result = DeleteFromDirectorySilently(directoryInfo);
                Console.WriteLine($"{result.NumberOfDirectoriesDeleted} directories with {result.NumberOfFilesDeleted} files deleted.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        private static void PrintHeader()
        {
            var builder = new StringBuilder();

            builder.Append("This program removes the bin, obj and packages directories and their contents from the specified directory and its subdirectories. Specify commandline argument \"-v\" to run it in Verbose mode.");
            builder.Append("(C) Sathyaish Chakravarthy 2016. All rights reserved.");
            builder.AppendLine();

            Console.WriteLine(builder.ToString());
        }

        // bin
        // obj
        // packages
        private static Result DeleteFromDirectory(DirectoryInfo directoryInfo, bool verbose)
        {
            return new Result();

            //if (directoryInfo == null) return new Result();

            //long effectiveFileCount = 0;
            //long effectiveDirectoryCount = 0;

            //var directories = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);

            //if (directories.Length == 0) return new Result();

            //foreach (var directory in directories)
            //{
            //    if (directoryNames.Contains(directoryInfo.Name, StringComparer.InvariantCultureIgnoreCase))
            //    {
            //        ++effectiveDirectoryCount;

            //        var files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            //        effectiveFileCount += files.LongCount();

            //        if (verbose)
            //        {
            //            Console.Write($"\nScanning directory {directoryInfo.FullName}...");
            //            Console.Write($"{files.Count()} files found.");
            //            Console.WriteLine();
            //            Console.WriteLine("Deleting files...");
            //        }

            //        foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
            //        {
            //            if (verbose)
            //            {
            //                Console.WriteLine($"Deleting file {file.FullName}");
            //            }

            //            file.Delete();
            //        }

            //        var subDirectories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);

            //        if (verbose)
            //        {
            //            Console.Write($"\nSearching directory {directoryInfo.FullName} for sub-directories...");

            //            if (subDirectories.Length > 0)
            //            {
            //                Console.Write($"{subDirectories.Count()} sub-directories found.");
            //                Console.WriteLine();
            //                Console.WriteLine("Deleting sub-directories...");
            //            }
            //        }

            //        if (subDirectories.Length > 0)
            //        {
            //            foreach(var subDirectory in subDirectories)
            //            {

            //            }
            //        }
            //    }
        //    }

        //    return new Result { NumberOfDirectoriesDeleted = effectiveDirectoryCount, NumberOfFilesDeleted = effectiveFileCount };
        }

        private static Result DeleteFromDirectorySilently(DirectoryInfo directoryInfo)
        {
            var result = new Result();

            if (directoryInfo == null) return result;

            var matchingSubdirectories = directoryInfo.GetDirectories("*", SearchOption.AllDirectories)
                .Where(d => directoryNames.Contains(d.Name, StringComparer.InvariantCultureIgnoreCase))
                .ToArray();

            result.NumberOfDirectoriesDeleted += matchingSubdirectories?.Length ?? 0;

            matchingSubdirectories?.ToList().ForEach(d =>
            {
                var files = d.GetFiles("*", SearchOption.AllDirectories);

                result.NumberOfFilesDeleted += files?.Length ?? 0;

                try
                {

                    d.Delete(true);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error deleting directory {d.FullName}. {ex.Message}");
                    throw;
                }
            });

            return result;
        }
    }
}
