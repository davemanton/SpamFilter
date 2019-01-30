using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Consoler;
using Common;
using Data.Analysis;
using Data.Preparation;
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
                default:
                    dataPreparer.PrepareAndSaveDataAsCsv();
                    dataAnalyser.Analyse();
                    break;
            }

            
        }
    }
}
