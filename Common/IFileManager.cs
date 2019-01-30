using System.Collections.Generic;
using Deedle;

namespace Common
{
    public interface IFileManager
    {
        IEnumerable<string> ReadFromFile(string filepath);
        void SaveToFile(Series<string, double> data, string filepath);
    }
}