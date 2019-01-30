using Accord.MachineLearning.Performance;
using Accord.Statistics.Models.Regression;

namespace Model.Analysis
{
    public interface IModelBuilder
    {
        CrossValidationResult<LogisticRegression, double[], int> BuildModel(double[][] inputs, int[] outputs);
    }
}