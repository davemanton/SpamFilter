using System.Collections.Generic;
using Deedle;

namespace Model.Analysis
{
    public interface ITermSelector
    {
        IEnumerable<string> SelectSpamTerms(Frame<int, string> spamTermFrame);
    }
}