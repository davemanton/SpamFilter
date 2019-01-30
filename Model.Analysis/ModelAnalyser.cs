using System;
using Accord.MachineLearning;
using Common;
using Deedle;

namespace Model.Analysis
{
    public interface IModelAnalyser
    {
        void Analyse();
    }

    public class ModelAnalyser : IModelAnalyser
    {
        private readonly IAppSettings _appSettings;
        private readonly ITermSelector _termSelector;
        private readonly IFeatureSelector _featureSelector;
        private readonly IModelBuilder _modelBuilder;

        public ModelAnalyser(IAppSettings appSettings, ITermSelector termSelector, IFeatureSelector featureSelector, IModelBuilder modelBuilder)
        {
            _appSettings = appSettings;
            _termSelector = termSelector;
            _featureSelector = featureSelector;
            _modelBuilder = modelBuilder;
        }

        public void Analyse()
        {
            var startTime = DateTime.Now;
            Console.WriteLine("--- Starting Model Analysis ---");

            Console.WriteLine();
            Console.WriteLine("--- Reading in Analysed Data ---");
            var termsVectorDataFrame = Frame.ReadCsv(
                _appSettings.TermsVectorFilePath,
                hasHeaders: true,
                inferTypes: true);
            var spamTermFrequencyDataFrame = Frame.ReadCsv(
                _appSettings.SpamTermsFilePath,
                hasHeaders: false,
                inferTypes: false,
                schema: "string,int");

            var fileReadTime = DateTime.Now;
            Console.WriteLine($"File Read in Duration: {(fileReadTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Selecting Features ---");
            var termFeatures = _termSelector.SelectSpamTerms(spamTermFrequencyDataFrame);

            var inputs = _featureSelector.SelectInputFeatures(termsVectorDataFrame, termFeatures);
            var outputs = _featureSelector.SelectOutputClassifications(termsVectorDataFrame);

            var featureSelectionTime = DateTime.Now;
            Console.WriteLine(
                $"Feature Selection Duration: {(featureSelectionTime - fileReadTime).Milliseconds}ms, {(featureSelectionTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Building Model ---");

            var result = _modelBuilder.BuildModel(inputs, outputs);

            var modelBuildTime = DateTime.Now;
            Console.WriteLine(
                $"Model Building Duration: {(modelBuildTime - featureSelectionTime).Milliseconds}ms, {(modelBuildTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Model Analysis ---");

            var numberOfSamples = result.NumberOfSamples;
            var numberOfInputs = result.NumberOfInputs;
            var numberOfOutputs = result.NumberOfOutputs;

            var trainingError = result.Training.Mean;
            var validationError = result.Validation.Mean;

            Console.WriteLine();
            Console.WriteLine("--- Confusion Matrix ---");
            var gcm = result.ToConfusionMatrix(inputs, outputs);

            Console.WriteLine();
            Console.Write("\t\tActual 0\t\tActual 1\n");
            for (var i = 0; i < gcm.Matrix.GetLength(0); i++)
            {
                Console.Write($"Pred {i} :\t");
                for (var j = 0; j < gcm.Matrix.GetLength(1); j++)
                {
                    Console.Write(gcm.Matrix[i, j] + "\t\t\t");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("--- Sample Size ---");
            Console.WriteLine(
                $"# samples: {numberOfSamples}, # inputs: {numberOfInputs}, # outputs: {numberOfOutputs}");
            Console.WriteLine($"training error: {trainingError}");
            Console.WriteLine($"validation error: {validationError}");

            Console.WriteLine();
            Console.WriteLine("--- Calculating Accuracy, Precision, Recall ---");

            var truePositive = (float) gcm.Matrix[1, 1];
            var trueNegative = (float) gcm.Matrix[0, 0];
            var falsePositive = (float) gcm.Matrix[1, 0];
            var falseNegative = (float) gcm.Matrix[0, 1];

            Console.WriteLine($"Accuracy: {(truePositive + trueNegative) / numberOfSamples}");
            Console.WriteLine($"Precision: {(truePositive / (truePositive + falsePositive))}");
            Console.WriteLine($"Recall: {(truePositive / (truePositive + falseNegative))}");

            var modelAnalysisTime = DateTime.Now;
            Console.WriteLine();            
            Console.WriteLine($"Model Building Duration: {(modelAnalysisTime - modelBuildTime).Milliseconds}ms, {(modelAnalysisTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Model Analysis Complete ---");
            Console.ReadKey();
        }
    }
}
