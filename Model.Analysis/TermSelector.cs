using System.Collections.Generic;
using Common;
using Deedle;

namespace Model.Analysis
{
    public class TermSelector : ITermSelector
    {
        private readonly IAppSettings _appSettings;

        public TermSelector(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public IEnumerable<string> SelectSpamTerms(Frame<int, string> spamTermFrame)
        {
            spamTermFrame.RenameColumns(new [] { DataConstants.Term, DataConstants.Occurrences });
            var indexedSpamTerms = spamTermFrame.IndexRows<string>(DataConstants.Term);

            return indexedSpamTerms
                .Where(term => term.Value.GetAs<int>(DataConstants.Occurrences) >= _appSettings.MinimumTermOccurrences)
                .RowKeys;
        }
    }
}