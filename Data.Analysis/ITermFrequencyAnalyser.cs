using Deedle;

namespace Data.Analysis
{
    public interface ITermFrequencyAnalyser
    {
        Series<string, double> CalculateWordFrequency(Frame<int, string> frame, string classifierColumnName,
            int classification);

        void GenerateTermProportionsGraph(Series<string, double> termsSeries, int totalTerms,
            Series<string, double> comparisonSeries, int comparisonTotal, string graphTitle);
    }
}