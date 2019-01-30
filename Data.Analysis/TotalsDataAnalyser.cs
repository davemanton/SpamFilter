using Accord.Controls;
using Common;
using Deedle;

namespace Data.Analysis
{
    public class TotalsDataAnalyser : ITotalsDataAnalyser
    {
        public int CalculateTotalClassification(Frame<int, string> frame, string classificationColumnName, int classification) =>
            frame.Where(row => row.Value.GetAs<int>(classificationColumnName) == classification).RowCount;

        public void GenerateTotalsGraph(Frame<int, string> frame, int hamTotal, int spamTotal)
        {
            var barChart = DataBarBox.Show(
                new string[] { "Ham", "Spam" },
                new double[] { hamTotal, spamTotal }
            );

            barChart.SetTitle("Ham vs. Spam in Sample Set");
        }
    }
}