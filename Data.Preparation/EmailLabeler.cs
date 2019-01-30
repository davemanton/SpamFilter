using Common;
using Deedle;

namespace Data.Preparation
{
    public class EmailLabeller : IEmailLabeller
    {
        private readonly IAppSettings _appSettings;

        public EmailLabeller(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public void MergeLabelsToEmailData(Frame<int, string> emailDataFrame)
        {
            var labelDataFrame = Frame.ReadCsv(_appSettings.LabelFilePath,
                hasHeaders: false, 
                separators: " ", 
                schema: "int,string");

            emailDataFrame.AddColumn(DataConstants.Ham, labelDataFrame.GetColumnAt<string>(0));
        }
    }
}