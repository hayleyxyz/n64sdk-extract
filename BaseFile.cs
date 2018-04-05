using System;
using System.IO;
using System.Collections.Generic;

namespace n64sdk_extract
{
    internal class BaseFile
    {
        public Stream Stream { get; set; }
        public long Offset { get; set; }
        public IDB IDB { get; set; }

        public BaseFile(IDB idb, Stream stream)
        {
            this.Stream = stream;
            Offset = stream.Position;
            this.IDB = idb;
        }

        public IEnumerable<IDB.Descriptor> ExtractTo(string directory)
        {
            Stream.Seek(Offset, SeekOrigin.Begin);
            // Seek past header
            Stream.Seek(0xd, SeekOrigin.Current);

            foreach (var entry in IDB.Files)
            {
                var dst = Path.Combine(directory, entry.Destination);

                switch (entry.Type)
                {
                    case IDB.Descriptor.DescriptorType.Directory:
                        Directory.CreateDirectory(dst);
                        break;

                    case IDB.Descriptor.DescriptorType.File:
                        var dstDir = Path.GetDirectoryName(dst);

                        if (!Directory.Exists(dstDir))
                        {
                            Directory.CreateDirectory(dstDir);
                        }

                        var buffer = new byte[entry.Size];
                        Stream.Read(buffer, 0, entry.Size);
                        File.WriteAllBytes(dst, buffer);

                        break;

                    case IDB.Descriptor.DescriptorType.Symlink:
                        // Not implemented
                        break;
                }

                yield return entry;

            }

        }
    }
}
