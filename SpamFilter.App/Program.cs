using Deedle;
using EAGetMail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var dataDirectory = Path.Combine("C:/Sandbox/machine-learning/SpamFilter/RawData");
            var emailFiles = Directory.GetFiles(dataDirectory, "*.eml");

            var emailDataFrame = ParseEmails(emailFiles.AsEnumerable());

            var labelDataFrame = Frame.ReadCsv($"{dataDirectory}\\SPAMTrain.label", 
                hasHeaders: false, separators: " ", schema: "int,string");

            emailDataFrame.AddColumn("is_ham", labelDataFrame.GetColumnAt<string>(0));
            emailDataFrame.SaveCsv("C:/Sandbox/machine-learning/SpamFilter/ProcessedData/transformed.csv");

            Console.WriteLine("Data Prep Done");
            Console.ReadKey();
        }

        private static Frame<int, string> ParseEmails(IEnumerable<string> files)
        {
            var rows = files.Select((file, index) =>
            {
                var email = new Mail("TryIt");
                email.Load(file, false);

                return new {
                    emailNumber = index,
                    subject = email.Subject
                };
            });

            return Frame.FromRecords(rows);
        }
    }
}
