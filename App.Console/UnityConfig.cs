using Common;
using Data.Analysis;
using Data.Preparation;
using Unity;

namespace App.Consoler
{
    public static class UnityConfig
    {
        public static UnityContainer ConfigureDependencies()
        {
            var container = new UnityContainer();

            container
                .RegisterType<IAppSettings, AppSettings>()
                .RegisterType<IFileManager, FileManager>()
                .RegisterType<IDataPreparer, DataPreparer>()
                .RegisterType<IEmailReader, EmailReader>()
                .RegisterType<IEmailLabeller, EmailLabeller>()
                .RegisterType<ITermFrequencyAnalyser, TermFrequencyAnalyser>()
                .RegisterType<ITotalsDataAnalyser, TotalsDataAnalyser>()
                .RegisterType<ITermsExtractor, TermsExtractor>()
                .RegisterType<IDataAnalyser, DataAnalyser>()
                ;

            return container;
        }
    }
}