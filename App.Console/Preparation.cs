using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Data.Preparation;
using Unity;

namespace App.Consoler
{
    class Preparation
    {
        

        static void Main(string[] args)
        {
            var unity = new UnityContainer();
            unity
                .RegisterType<IAppSettings, AppSettings>()
                .RegisterType<IDataPreparer, DataPreparer>()
                .RegisterType<IEmailReader, EmailReader>()
                .RegisterType<IEmailLabeller, EmailLabeller>();

            var preparer = unity.Resolve<IDataPreparer>();

            preparer.PrepareAndSaveDataAsCsv();
        }
    }
}
