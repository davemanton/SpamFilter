using System.Linq;
using System.Text.RegularExpressions;
using Common;
using Deedle;

namespace Data.Analysis
{
    public interface ITermsExtractor
    {
        Frame<int, string> CreateTermDataFrame(Frame<int, string> frame, string termsColumn, string classifierColumnName);
    }

    public class TermsExtractor : ITermsExtractor
    {
        private readonly IFileManager _fileManager;
        private readonly IAppSettings _appSettings;

        public TermsExtractor(IFileManager fileManager, IAppSettings appSettings)
        {
            _fileManager = fileManager;
            _appSettings = appSettings;
        }

        public Frame<int, string> CreateTermDataFrame(Frame<int, string> frame, string termsColumn, string classifierColumnName)
        {
            var stopWords = _fileManager.ReadFromFile(_appSettings.StopWordsFilePath)
                .Distinct()
                .Select(x => x.ToLower())
                .ToList();

            var termsByRows = frame.GetColumn<string>(termsColumn).GetAllValues().Select((row, index) =>
            {
                var seriesBuilder = new SeriesBuilder<string, int>();

                var rowWords = Regex.Matches(row.Value, "[a-zA-Z]+('(s|d|t|ve|m))?")
                    .Cast<Match>()
                    .Where(word => !stopWords.Contains(word.Value.ToLower()))
                    .Select(word => word.Value.ToLower())
                    .Distinct();

                foreach (var word in rowWords)
                {
                    seriesBuilder.Add(word, 1);
                }

                return KeyValue.Create(index, seriesBuilder.Series);
            });

            var termDataFrame = Frame.FromRows(termsByRows).FillMissing(0);
            termDataFrame.AddColumn(classifierColumnName, frame.GetColumn<int>(classifierColumnName));

            termDataFrame.SaveCsv(_appSettings.TermsVectorFilePath);

            return termDataFrame;
        }
    }
}