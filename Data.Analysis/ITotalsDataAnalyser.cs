using Deedle;

namespace Data.Analysis
{
    public interface ITotalsDataAnalyser
    {
        int CalculateTotalClassification(Frame<int, string> frame, string classificationColumnName, int classification);
        void GenerateTotalsGraph(Frame<int, string> frame, int hamTotal, int spamTotal);
    }
}