using System.Configuration;
using System.IO;

namespace Common
{
    public interface IAppSettings
    {
        string RawDataDirectory { get; }
        string ProcessedDataDirectory { get; }
        string LabelFilePath { get; }
        string TransformedFilePath { get; }
    }

    public class AppSettings : IAppSettings
    {        
        public string RawDataDirectory => ConfigurationManager.AppSettings["RawDataDirectory"];
        public string ProcessedDataDirectory => ConfigurationManager.AppSettings["ProcessedDataDirectory"];
        public string LabelFilePath => Path.Combine(RawDataDirectory, ConfigurationManager.AppSettings["LabelFilename"]);
        public string TransformedFilePath => Path.Combine(ProcessedDataDirectory, ConfigurationManager.AppSettings["TransformedFilename"]);
    }
}