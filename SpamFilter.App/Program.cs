using Accord.Controls;
using Deedle;
using EAGetMail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string processedDataDirectory = "C:/Sandbox/machine-learning/SpamFilter/ProcessedData";

            var emailDataFrame = ProcessEmailsAndCreateCsv(processedDataDirectory);

            Console.WriteLine(" --- Creating Subject Word Vector --- ");
            var subjectWordVectorDataFrame = CreateWordVector(emailDataFrame.GetColumn<string>("subject"));

            Console.WriteLine(" --- Adding Label to Subject Word Vector Data Frame --- ");
            subjectWordVectorDataFrame.AddColumn("is_ham", emailDataFrame.GetColumn<string>("is_ham"));

            //Console.WriteLine(" --- Creating Subject Word Vector Csv File --- ");
            //var copy = subjectWordVectorDataFrame.Clone();
            //copy.AddColumn("email_subject", emailDataFrame.GetColumn<string>("subject"));

            //copy.SaveCsv("C:/Sandbox/machine-learning/SpamFilter/ProcessedData/subjectWordVector.csv");

            var hamCount = CountColumnValue(subjectWordVectorDataFrame, "is_ham", 1);
            var spamCount = CountColumnValue(subjectWordVectorDataFrame, "is_ham", 0);
            CreateTotalsBarChart(hamCount, spamCount);

            var topN = 10;

            var hamTermProportions = CalculateTermsAndSave(subjectWordVectorDataFrame, "is_ham", 1, hamCount, processedDataDirectory);
            var topHamTerms = hamTermProportions.Keys.Take(topN);
            var topHamTermProportions = hamTermProportions.Values.Take(topN);

            var spamTermProportions = CalculateTermsAndSave(subjectWordVectorDataFrame, "is_ham", 0, spamCount, processedDataDirectory);
            var topSpamTerms = spamTermProportions.Keys.Take(topN);
            var topSpamTermProportions = spamTermProportions.Values.Take(topN);

            CreateTopTermsBarchart(topHamTerms, topHamTermProportions, spamTermProportions, "Ham", "Spam");
            CreateTopTermsBarchart(topSpamTerms, topSpamTermProportions, hamTermProportions, "Spam", "Ham");


            Console.WriteLine(" --- Data Preparation Complete --- ");
            Console.ReadKey();
        }

        private static Frame<int, string> ProcessEmailsAndCreateCsv(string processedDataDirectory)
        {
            Console.WriteLine(" --- Processing Emails --- ");
            var dataDirectory = "C:/Sandbox/machine-learning/SpamFilter/RawData";
            var emailFiles = Directory.GetFiles(dataDirectory, "*.eml");

            var emailDataFrame = ParseEmails(emailFiles.AsEnumerable());

            var labelDataFrame = Frame.ReadCsv($"{dataDirectory}\\SPAMTrain.label",
                hasHeaders: false, separators: " ", schema: "int,string");

            Console.WriteLine(" --- Creating Csv From Emails --- ");
            emailDataFrame.AddColumn("is_ham", labelDataFrame.GetColumnAt<string>(0));
            emailDataFrame.SaveCsv($"{processedDataDirectory}/transformed.csv");
            
            return emailDataFrame;
        }


        private static Frame<int, string> ParseEmails(IEnumerable<string> files)
        {
            const string eaTrialVersionRemark = "(Trial Version)";

            var rows = files.Select((file, index) =>
            {
                Console.WriteLine($"Parsing Email: {index}");

                var email = new Mail("TryIt");
                email.Load(file, false);

                return new {
                    emailNumber = index,
                    subject = email.Subject.EndsWith(eaTrialVersionRemark) 
                                ? email.Subject.Substring(0, email.Subject.Length - eaTrialVersionRemark.Length) 
                                : email.Subject

                };
            });

            return Frame.FromRecords(rows);
        }

        private static Frame<int, string> CreateWordVector(Series<int, string> rows)
        {
            var stopwords = File.ReadLines("C:/Sandbox/machine-learning/SpamFilter/Config/stopwords.txt")
                .Distinct()
                .Select(x => x.ToLower())
                .ToList();
           
            var wordsByRows = rows.GetAllValues().Select((row, index) =>
            {
                Console.WriteLine($"Parsing Row: {index}");
                var seriesBuilder = new SeriesBuilder<string, int>();

                var rowWords = Regex.Matches(row.Value, "[a-zA-Z]+('(s|d|t|ve|m))?")
                .Cast<Match>()
                .Where(word => !stopwords.Contains(word.Value.ToLower()))
                .Select(word => word.Value.ToLower())
                .Distinct();

                foreach (var word in rowWords)
                {
                    seriesBuilder.Add(word, 1);
                }

                return KeyValue.Create(index, seriesBuilder.Series);
            });

            var wordVectorDataFrame = Frame.FromRows(wordsByRows).FillMissing(0);

            return wordVectorDataFrame;
        }

        private static Series<string, double>  CalculateTermFrequencies(Frame<int, string> frame, string columnName, int value)
            => frame.Where(row => row.Value.GetAs<int>(columnName) == value)
                .Sum()
                .Sort()
                .Reversed
                .Where(x => x.Key != columnName);

        private static int CountColumnValue(Frame<int, string> frame, string columnName, int value)
            => frame.Where(row => row.Value.GetAs<int>(columnName) == value)
                .RowCount;

        private static Series<string, double> CalculateTermsAndSave(
            Frame<int, string> frame, string columnName, int value, int count, string dataDirectory)
        {
            Console.WriteLine($" --- Processing Top Terms for {columnName} where value is {value} --- ");

            var termFrequences = CalculateTermFrequencies(frame, columnName, value);

            File.WriteAllLines($"{dataDirectory}/{columnName}_{value}.csv",
                termFrequences.Keys.Zip(
                    termFrequences.Values,
                    (a, b) => $"{a},{b}"));

            return termFrequences / count;
        }

        private static void CreateTotalsBarChart(int hamCount, int spamCount)
        {
            var barChart = DataBarBox.Show(
                new string[] { "Hame", "Spam" },
                new double[] { hamCount , spamCount }
                );

            barChart.SetTitle("Ham vs. Spam in Sample Set");
        }

        private static void CreateTopTermsBarchart(IEnumerable<string> terms, IEnumerable<double> proportions, Series<string, double> alternativeProportions, string titleSet, string altSet)
        {
            var barChart = DataBarBox.Show(
                terms.ToArray(),
                new double[][]
                    {
                        proportions.ToArray(),
                        alternativeProportions.GetItems(terms).Values.ToArray(),
                    });

            barChart.SetTitle($"Top Terms in {titleSet} Emails (blue: {titleSet.ToUpper()}, red: {altSet})");
        }
    }
}
