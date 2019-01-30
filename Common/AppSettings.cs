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
        string StopWordsFilePath { get; }
        string HamTermsFilePath { get; }
        string SpamTermsFilePath { get; }
        string TermsVectorFilePath { get; }
        int NumberTermsToGraph { get; }
        int MinimumTermOccurrences { get; }
    }

    public class AppSettings : IAppSettings
    {        
        public string RawDataDirectory => ConfigurationManager.AppSettings["RawDataDirectory"];
        public string ProcessedDataDirectory => ConfigurationManager.AppSettings["ProcessedDataDirectory"];
        public string LabelFilePath => Path.Combine(RawDataDirectory, ConfigurationManager.AppSettings["LabelFilename"]);
        public string TransformedFilePath => Path.Combine(ProcessedDataDirectory, ConfigurationManager.AppSettings["TransformedFilename"]);
        public string StopWordsFilePath => ConfigurationManager.AppSettings["StopWordsFilePath"];
        public string HamTermsFilePath => Path.Combine(ProcessedDataDirectory, ConfigurationManager.AppSettings["HamTermsFilename"]);
        public string SpamTermsFilePath => Path.Combine(ProcessedDataDirectory, ConfigurationManager.AppSettings["SpamTermsFilename"]);
        public string TermsVectorFilePath => Path.Combine(ProcessedDataDirectory, ConfigurationManager.AppSettings["TermsVectorFilename"]);
        public int NumberTermsToGraph => int.Parse(ConfigurationManager.AppSettings["NumberTermsToGraph"]);
        public int MinimumTermOccurrences => int.Parse(ConfigurationManager.AppSettings["MinimumTermOccurrences"]);
    }
}