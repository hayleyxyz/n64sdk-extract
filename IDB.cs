using System.IO;
using System.Collections.Generic;
using System.Text;

namespace n64sdk_extract
{
    internal class IDB
    {
        public List<Descriptor> Files;

        public IDB(Stream input)
        {
            Files = new List<Descriptor>();

            using (var reader = new StreamReader(input, Encoding.ASCII, true, 1024, true))
            {
                Read(reader);
            }
        }

        protected void Read(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var parts = line.Split(' ');

                var descriptor = new Descriptor()
                {
                    Type = (Descriptor.DescriptorType)parts[0][0],
                    Permissions = ushort.Parse(parts[1]),
                    User = parts[2],
                    Group = parts[3],
                    Source = parts[4],
                    Destination = parts[5]
                };

                foreach (var part in parts)
                {
                    var open = part.IndexOf('(');
                    if (open > 0 && part.LastIndexOf(')') == part.Length - 1)
                    {
                        var name = part.Substring(0, open);
                        var value = part.Substring(open + 1, part.Length - (open + 2));

                        descriptor.MiscAttributes.Add(name, value);

                        switch (name)
                        {
                            case "size":
                                descriptor.Size = int.Parse(value);
                                break;

                            case "symval":
                                descriptor.Symlink = value;
                                break;
                        }
                    }
                }

                Files.Add(descriptor);
            }
        }

        public class Descriptor
        {
            public enum DescriptorType
            {
                Directory = 'd',
                File = 'f',
                Symlink = 'l'
            };

            public DescriptorType Type { get; set; }
            public ushort Permissions { get; set; }
            public string User { get; set; }
            public string Group { get; set; }
            public string Source { get; set; }
            public string Destination { get; set; }
            public int Size { get; set; }
            public string Symlink { get; set; }
            public Dictionary<string, object> MiscAttributes { get; set; }

            public Descriptor()
            {
                MiscAttributes = new Dictionary<string, object>();
            }
        }
    }
}
