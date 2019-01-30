using Deedle;

namespace Data.Preparation
{
    public interface IEmailReader
    {
        Frame<int, string> ConvertEmailsToDataFrame();
    }
}