using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Deedle;

namespace Data.Analysis
{
    public class DataAnalyser : IDataAnalyser
    {
        private readonly IAppSettings _appSettings;
        private readonly ITotalsDataAnalyser _totalsDataAnalyser;
        private readonly ITermFrequencyAnalyser _termFrequencyAnalyser;
        private readonly IFileManager _fileManager;
        private readonly ITermsExtractor _termsExtractor;

        public DataAnalyser(IAppSettings appSettings, ITotalsDataAnalyser totalsDataAnalyser, ITermFrequencyAnalyser termFrequencyAnalyser, IFileManager fileManager, ITermsExtractor termsExtractor)
        {
            _appSettings = appSettings;
            _totalsDataAnalyser = totalsDataAnalyser;
            _termFrequencyAnalyser = termFrequencyAnalyser;
            _fileManager = fileManager;
            _termsExtractor = termsExtractor;
        }

        public void Analyse()
        {
            var startTime = DateTime.Now;
            Console.WriteLine("--- Starting Data Analysis ---");

            Console.WriteLine();
            Console.WriteLine("--- Reading in Prepared Data CSV ---");
            var emailDataFrame = Frame.ReadCsv(_appSettings.TransformedFilePath);

            var fileReadTime = DateTime.Now;
            Console.WriteLine($"File Read in Duration: {(fileReadTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Analysing Total Classifications ---");
            var hamCount = _totalsDataAnalyser.CalculateTotalClassification(emailDataFrame, DataConstants.Ham, 1);
            var spamCount = _totalsDataAnalyser.CalculateTotalClassification(emailDataFrame, DataConstants.Ham, 0);
            _totalsDataAnalyser.GenerateTotalsGraph(emailDataFrame, hamCount, spamCount);

            var totalsAnalysisTime = DateTime.Now;
            Console.WriteLine($"Total Analysis Duration: {(totalsAnalysisTime - fileReadTime).Milliseconds}ms, {(totalsAnalysisTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Extracting Subject Terms ---");
            var subjectTermsDataFrame = _termsExtractor.CreateTermDataFrame(emailDataFrame, DataConstants.Subject, DataConstants.Ham);

            var termsExtractionTime = DateTime.Now;
            Console.WriteLine($"Total Analysis Duration: {(termsExtractionTime - totalsAnalysisTime).Milliseconds}ms, {(termsExtractionTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Analysing Term Frequencies ---");
            var hamSubjectTerms = _termFrequencyAnalyser.CalculateWordFrequency(subjectTermsDataFrame, DataConstants.Ham, 1);
            _fileManager.SaveToFile(hamSubjectTerms, _appSettings.HamTermsFilePath);

            var spamSubjectTerms = _termFrequencyAnalyser.CalculateWordFrequency(subjectTermsDataFrame, DataConstants.Ham, 0);
            _fileManager.SaveToFile(hamSubjectTerms, _appSettings.SpamTermsFilePath);

            _termFrequencyAnalyser.GenerateTermProportionsGraph(hamSubjectTerms, hamCount, spamSubjectTerms, spamCount, "Top Terms in Ham Emails (blue: Ham, red: Spam");
            _termFrequencyAnalyser.GenerateTermProportionsGraph(spamSubjectTerms, spamCount, hamSubjectTerms, hamCount, "Top Terms in Spam Emails (blue: Spam, red: Ham");
            var termFrequencyAnalysisTime = DateTime.Now;
            Console.WriteLine($"Term Frequency Analysis Duration: {(termFrequencyAnalysisTime - termsExtractionTime).Milliseconds}ms, {(termFrequencyAnalysisTime - startTime).Milliseconds}ms");

            Console.WriteLine();
            Console.WriteLine("--- Analysis Complete ---");
            Console.ReadKey();
        }
    }
}
