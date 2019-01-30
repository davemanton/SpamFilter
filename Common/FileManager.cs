using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deedle;

namespace Common
{
    public class FileManager : IFileManager
    {
        public IEnumerable<string> ReadFromFile(string filepath)
        {
            return File.ReadLines(filepath);
        }

        public void SaveToFile(Series<string, double> data, string filepath)
        {
            File.WriteAllLines(filepath, data.Keys.Zip(data.Values, (key, value) => $"{key},{value}"));
        }
    }
}