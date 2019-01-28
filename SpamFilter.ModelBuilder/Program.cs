using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Accord;
using Accord.IO;
using Accord.MachineLearning;
using Accord.MachineLearning.Performance;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;
using Deedle;

namespace SpamFilter.ModelBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            // Read in the file we created in the Data Preparation step
            // TODO: change the path to point to your data directory
            string dataDirPath = "C:/Sandbox/machine-learning/SpamFilter/ProcessedData";
            // Load the data into a data frame and set the "emailNum" column as an index
            var wordVecDF = Frame.ReadCsv(
                Path.Combine(dataDirPath, "subjectWordVector.csv"),
                hasHeaders: true,
                inferTypes: true
            );
            // Load the transformed data from data preparation step to get "is_ham" column
            var rawDF = Frame.ReadCsv(
                Path.Combine(dataDirPath, "transformed.csv"),
                hasHeaders: true,
                inferTypes: false,
                schema: "int,string,int"
            ).IndexRows<int>("emailNumber").SortRowsByKey();
            // Load Term Frequency Data
            var spamTermFrequencyDF = Frame.ReadCsv(
                Path.Combine(dataDirPath, "is_ham_0.csv"),
                hasHeaders: false,
                inferTypes: false,
                schema: "string,int"
            );
            spamTermFrequencyDF.RenameColumns(new [] { "word", "num_occurrences" });
            var indexedSpamTermFrequencyDF = spamTermFrequencyDF.IndexRows<string>("word");

            var numFolds = 3;

            var targetVariables = 1 - rawDF.GetColumn<int>("is_ham");
            Console.WriteLine("{0} spams vs. {1} hams", targetVariables.NumSum(), (targetVariables.KeyCount - targetVariables.NumSum()));

            int minNumOccurrences = 25;
            string[] wordFeatures = indexedSpamTermFrequencyDF.Where(
                x => x.Value.GetAs<int>("num_occurrences") >= minNumOccurrences
            ).RowKeys.ToArray();
            Console.WriteLine("Num Features Selected: {0}", wordFeatures.Count());

            double[][] input = wordVecDF.Columns[wordFeatures].Rows.Select(
                x => Array.ConvertAll<object, double>(x.Value.ValuesAll.ToArray(), o => Convert.ToDouble(o))
            ).ValuesAll.ToArray();
            int[] output = targetVariables.Values.ToArray();
            
            Console.WriteLine($" --- Preparation Time Duration: {(DateTime.Now - startTime).TotalMilliseconds}ms ---");
       
            var cvLogisticRegressionClassifier =
                CrossValidation.Create<LogisticRegression,
                    IterativeReweightedLeastSquares<LogisticRegression>,
                    double[],
                    int>(
                    k: numFolds,
                    learner: (p) => new IterativeReweightedLeastSquares<LogisticRegression>
                        {
                            MaxIterations = 100,
                            Regularization = 1e-6
                        },
                    loss: (actual, expected, p) => new ZeroOneLoss(expected).Loss(actual),
                    fit: (teacher, x, y, w) => teacher.Learn(x, y, w),
                    x: input,
                    y: output
                    );

            var result = cvLogisticRegressionClassifier.Learn(input, output);
            

            //result.Save(
            //    $"C:/Sandbox/machine-learning/SpamFilter/ProcessedData/CVLogRegClassifier_{DateTime.Now:dd-MM-yyyy_hh.mm}_Occ-{minNumOccurrences}.accord");

            Console.WriteLine($" --- Model Building Time Duration: {(DateTime.Now - startTime).TotalMilliseconds}ms ---");

            // Sample Size
            int numberOfSamples = result.NumberOfSamples;
            int numberOfInputs = result.NumberOfInputs;
            int numberOfOutputs = result.NumberOfOutputs;

            // Training & Validation Errors
            double trainingError = result.Training.Mean;
            double validationError = result.Validation.Mean;

            // Confusion Matrix
            Console.WriteLine("\n---- Confusion Matrix ----");
            GeneralConfusionMatrix gcm = result.ToConfusionMatrix(input, output);
            Console.WriteLine("");
            Console.Write("\t\tActual 0\t\tActual 1\n");
            for (int i = 0; i < gcm.Matrix.GetLength(0); i++)
            {
                Console.Write("Pred {0} :\t", i);
                for (int j = 0; j < gcm.Matrix.GetLength(1); j++)
                {
                    Console.Write(gcm.Matrix[i, j] + "\t\t\t");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n---- Sample Size ----");
            Console.WriteLine("# samples: {0}, # inputs: {1}, # outputs: {2}", numberOfSamples, numberOfInputs, numberOfOutputs);
            Console.WriteLine("training error: {0}", trainingError);
            Console.WriteLine("validation error: {0}\n", validationError);

            Console.WriteLine("\n---- Calculating Accuracy, Precision, Recall ----");

            float truePositive = (float)gcm.Matrix[1, 1];
            float trueNegative = (float)gcm.Matrix[0, 0];
            float falsePositive = (float)gcm.Matrix[1, 0];
            float falseNegative = (float)gcm.Matrix[0, 1];

            // Accuracy
            Console.WriteLine(
                "Accuracy: {0}",
                (truePositive + trueNegative) / numberOfSamples
            );
            // True-Positive / (True-Positive + False-Positive)
            Console.WriteLine("Precision: {0}", (truePositive / (truePositive + falsePositive)));
            // True-Positive / (True-Positive + False-Negative)
            Console.WriteLine("Recall: {0}", (truePositive / (truePositive + falseNegative)));

            Console.WriteLine($" --- Total Duration: {(DateTime.Now - startTime).TotalMilliseconds}ms ---");

            var modelRun = DateTime.Now;

            Console.WriteLine($" --- Model Run Duration: {(DateTime.Now - modelRun).TotalMilliseconds}ms ---");

           var test = result.Models.First().Model.Decide(input[0]);
           Console.WriteLine(test);

            Console.ReadKey();

        }

    
    }
}
