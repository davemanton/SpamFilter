using System.Linq;
using Data.Analysis;
using Data.Preparation;
using Model.Analysis;
using Unity;

namespace App.Console
{
    class SpamFilter
    {        
        static void Main(string[] args)
        {
            var dependencyManager = UnityConfig.ConfigureDependencies();

            var dataPreparer = dependencyManager.Resolve<IDataPreparer>();
            var dataAnalyser = dependencyManager.Resolve<IDataAnalyser>();
            var modelAnalyser = dependencyManager.Resolve<IModelAnalyser>();

            var action = args.Any() ? args[0] : string.Empty;

            switch (action)
            {
                case "prepare":                       
                    dataPreparer.PrepareAndSaveDataAsCsv();
                    break;
                case "analyse":
                    dataAnalyser.Analyse();
                    break;
                case "model":
                    modelAnalyser.Analyse();
                    break;
                default:
                    dataPreparer.PrepareAndSaveDataAsCsv();
                    dataAnalyser.Analyse();
                    modelAnalyser.Analyse();
                    break;
            }

            
        }
    }
}
