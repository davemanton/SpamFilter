using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common;
using Deedle;
using EAGetMail;

namespace Data.Preparation
{
    public interface IEmailReader
    {
        Frame<int, string> ConvertEmailsToDataFrame();
    }

    public class EmailReader : IEmailReader
    {        
        private readonly IAppSettings _appSettings;

        public EmailReader(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public Frame<int, string> ConvertEmailsToDataFrame()
        {
            var emailFiles = Directory.GetFiles(_appSettings.RawDataDirectory, "*.eml");

            var emailDataFrame = ParseEmailFiles(emailFiles);

            return emailDataFrame;
        }

        private static Frame<int, string> ParseEmailFiles(IEnumerable<string> files)
        {
            const string eaTrialVersionRemark = "{Trial Version)";

            var parsedEmails = files.AsParallel().Select((file, index) =>
            {
                var filename = Path.GetFileName(file);

                var email = new Mail("TryIt");
                email.Load(file, false);

                return new
                {
                    index,
                    filename,
                    from = email.From,
                    subject = email.Subject.EndsWith(eaTrialVersionRemark)
                        ? email.Subject.Substring(0, email.Subject.Length - eaTrialVersionRemark.Length)
                        : email.Subject,
                    body = email.TextBody,
                };
            }).OrderBy(x => x.index).ToList();

            return Frame.FromRecords(parsedEmails);
        }
    }
}
