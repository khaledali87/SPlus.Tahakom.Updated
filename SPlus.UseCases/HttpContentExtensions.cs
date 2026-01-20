using System.Collections.Generic;
namespace SPlus.UseCases
{
    public class HttpPostedData
    {
        public HttpPostedData(IDictionary<string, HttpPostedField> fields, IDictionary<string, HttpPostedFile> files)
        {
            Fields = fields;
            Files = files;
        }

        public IDictionary<string, HttpPostedField> Fields { get; private set; }
        public IDictionary<string, HttpPostedFile> Files { get; private set; }
    }

    public class HttpPostedFile
    {
        public HttpPostedFile(string name, string filename, byte[] file)
        {
            File = file;
            Name = name;
            Filename = filename;
        }
        public string Name { get; private set; }
        public string Filename { get; private set; }
        public byte[] File { private set; get; }
    }
    public class HttpPostedField
    {
        public HttpPostedField(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public string Name { get; private set; }
        public string Value { get; private set; }
    }
}