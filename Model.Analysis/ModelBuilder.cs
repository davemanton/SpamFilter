using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Common;

namespace Model.Analysis
{
    public class ModelBuilder : IModelBuilder
    {
        private readonly IAppSettings _appSettings;

        public ModelBuilder(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public CrossValidationResult<LogisticRegression, double[], int> BuildModel(double[][] inputs, int[] outputs)
        {
            var cvLogisticRegressionClassifier =
                CrossValidation.Create<LogisticRegression,
                    IterativeReweightedLeastSquares<LogisticRegression>,
                    double[],
                    int>(
                    k: _appSettings.ModelNumFolds,
                    learner: (p) => new IterativeReweightedLeastSquares<LogisticRegression>
                    {
                        MaxIterations = 100,
                        Regularization = 1e-6
                    },
                    loss: (actual, expected, p) => new ZeroOneLoss(expected).Loss(actual),
                    fit: (teacher, x, y, w) => teacher.Learn(x, y, w),
                    x: inputs,
                    y: outputs
                );

            var result = cvLogisticRegressionClassifier.Learn(inputs, outputs);

            return result;
        }
    }
}