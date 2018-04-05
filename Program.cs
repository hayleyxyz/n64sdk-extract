using System;
using System.IO;

namespace n64sdk_extract
{
    class Program
    {

        static void Main(string[] args) {
            if(args.Length < 3) {
                Console.WriteLine(
                    "usage: {0} <.idb file> <.dev file> <output dir>",
                    Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])
                );

                Environment.Exit(1);
            }

            var idbPath = args[0];
            var devPath = args[1];
            var dst = args[2];

            using(var idbStream = File.OpenRead(idbPath)) {
                var idb = new IDB(idbStream);

                using(var dataStream = File.OpenRead(devPath)) {
                    var baseFile = new BaseFile(idb, dataStream);

                    foreach(var entry in baseFile.ExtractTo(dst)) {
                        if (entry.Type == IDB.Descriptor.DescriptorType.Symlink) {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine("symlinks not implemented: {0} => {1}", entry.Destination, entry.Symlink);
                            Console.ResetColor();
                        }
                        else {
                            Console.WriteLine(entry.Destination);
                        }
                    }
                }
            }

            Environment.Exit(0);
        }
    }
}
