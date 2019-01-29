using System;
using Common;
using Deedle;

namespace Data.Preparation
{
    public interface IDataPreparer
    {
        void PrepareAndSaveDataAsCsv();
    }

    public class DataPreparer : IDataPreparer
    {
        private readonly IEmailReader _emailReader;
        private readonly IEmailLabeller _emailLabeller;
        private readonly IAppSettings _appSettings;

        public DataPreparer(IEmailReader emailReader, IEmailLabeller emailLabeller, IAppSettings appSettings)
        {
            _emailReader = emailReader;
            _emailLabeller = emailLabeller;
            _appSettings = appSettings;
        }

        public void PrepareAndSaveDataAsCsv()
        {
            var startTime = DateTime.Now;
            Console.WriteLine("--- Preparing Data ---");
            Console.WriteLine();
            Console.WriteLine("--- Parsing Email Files ---");
            var emailDataFrame = _emailReader.ConvertEmailsToDataFrame();

            var parsedTime = DateTime.Now;
            var parseDuration = (parsedTime - startTime).Milliseconds;
            Console.WriteLine($"Emails Converted To Data Frame: {parseDuration}ms");

            Console.WriteLine();
            Console.WriteLine("--- Merging Labels With Email Data ---");
            _emailLabeller.MergeLabelsToEmailData(emailDataFrame);

            var labelTime = DateTime.Now;
            var labelDuration = (labelTime - parsedTime).Milliseconds;
            var totalDuration = (labelTime - startTime).Milliseconds;
            Console.WriteLine($"Emails Labelled: {labelDuration}ms, {totalDuration}ms");

            Console.WriteLine();
            Console.WriteLine($"--- Saving data as {_appSettings.TransformedFilePath} ---");
            emailDataFrame.SaveCsv(_appSettings.TransformedFilePath);

            var saveTime = DateTime.Now;
            var saveDuration = (saveTime - labelTime).Milliseconds;
            totalDuration = (saveTime - startTime).Milliseconds;
            Console.WriteLine($"Emails Labelled: {saveDuration}ms, {totalDuration}ms");

            Console.WriteLine();
            Console.WriteLine("--- Preparation Complete ---");
            totalDuration = (DateTime.Now - startTime).Milliseconds;
            Console.WriteLine($@"Total Duration: {totalDuration}ms");
            Console.ReadKey();
        }
    }
}