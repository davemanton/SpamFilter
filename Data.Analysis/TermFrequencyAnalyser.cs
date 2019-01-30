using System.Linq;
using Accord.Controls;
using Common;
using Deedle;

namespace Data.Analysis
{
    public class TermFrequencyAnalyser : ITermFrequencyAnalyser
    {
        private readonly IAppSettings _appSettings;

        public TermFrequencyAnalyser(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public Series<string, double> CalculateWordFrequency(Frame<int, string> frame, string classifierColumnName,
            int classification)
        {
            return frame.Where(row => row.Value.GetAs<int>(classifierColumnName) == classification)
                .Sum()
                .Sort()
                .Reversed
                .Where(x => x.Key != classifierColumnName);
        }

        public void GenerateTermProportionsGraph(Series<string, double> termsSeries, int totalTerms, Series<string, double> comparisonSeries, int comparisonTotal, string graphTitle)
        {
            var termsProportions = termsSeries / totalTerms;
            var topTerms = termsProportions.Keys.Take(_appSettings.NumberTermsToGraph).ToList();
            var topTermsProportions = termsProportions.Values.Take(_appSettings.NumberTermsToGraph);

            var comparisonProportions = comparisonSeries / comparisonTotal;

            var barChart = DataBarBox.Show(
                topTerms.ToArray(),
                new double[][]
                {
                    topTermsProportions.ToArray(),
                    comparisonProportions.GetItems(topTerms).Values.ToArray(),
                });

            barChart.SetTitle(graphTitle);
        }
    }
}