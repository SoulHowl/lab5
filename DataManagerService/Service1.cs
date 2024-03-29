﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConfigurationMeneger;
using DataManager;
namespace DataManagerService
{
    public partial class Service1 : ServiceBase
    {

        Service ser;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ser = new Service();
            Thread loggerThread = new Thread(new ThreadStart(ser.StartAsync));
            loggerThread.Start();
            
        }

        protected override  void OnStop()
        {
            ser.Stop();
            Thread.Sleep(1000);
        }

        class Service
        {
            private int TimeThread;
            private ParsOptions options;
            private DataManager.DataManager datamanager;
            private Random rand;
            bool enabled = true;
            public Service()
            {
                TimeThread = 15000; 
                rand = new Random();
                if (File.Exists(@"C:\Users\Lenovo\source\repos\AdventureWorks\DataManagerService\bin\Debug\configDB.xml"))
                {
                    options = new ParsOptions(@"C:\Users\Lenovo\source\repos\AdventureWorks\DataManagerService\bin\Debug\configDB.xml");
                }
                else if (File.Exists(@"C:\Users\Lenovo\source\repos\AdventureWorks\DataManagerService\bin\Debug\appsettingsDB.json"))
                {
                    options = new ParsOptions(@"C:\Users\Lenovo\source\repos\AdventureWorks\DataManagerService\bin\Debug\appsettingsDB.json");
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(@"D:\csharp\log.txt", true, System.Text.Encoding.Default))
                    {
                        sw.WriteLineAsync("ERROR WITH CONFIG FILES");

                    }
                }
                datamanager = options.GetModel<DataManager.DataManager>();
                try
                {
                    using (StreamWriter sw = new StreamWriter(@"D:\csharp\log.txt", true, System.Text.Encoding.Default))
                    {
                        sw.WriteLineAsync(datamanager.connectionString);

                    }
                }
                catch (Exception e)
                {
                    using (StreamWriter sw = new StreamWriter(@"D:\csharp\log.txt", true, System.Text.Encoding.Default))
                    {
                        sw.WriteLineAsync(e.Message);
                    }
                }
               
            }

            public async void StartAsync()
            {
                await Task.Run(async() =>
                {
                    while (enabled)
                    {
                        Thread.Sleep(TimeThread);
                        try
                        {
                            ThreadPool.QueueUserWorkItem(async state =>
                            {
                                await datamanager.MakeTransaction(15);
                                await datamanager.SendInfo();
                            });
                           
                        }
                        catch (Exception e)
                        {
                            using (StreamWriter sw = new StreamWriter(@"D:\csharp\log.txt", true,
                                System.Text.Encoding.Default))
                            {
                                await sw.WriteLineAsync(e.Message);
                            }
                        }

                        enabled = false;
                    }
                });
            }
            public void Stop()
            {
                enabled = false;
            }

        }
    }
}
