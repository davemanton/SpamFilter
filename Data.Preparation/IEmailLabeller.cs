using Deedle;

namespace Data.Preparation
{
    public interface IEmailLabeller
    {
        void MergeLabelsToEmailData(Frame<int, string> emailDataFrame);
    }
}