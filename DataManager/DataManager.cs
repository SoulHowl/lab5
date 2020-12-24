using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess_Layer;
using ErrorLogger;
using Modelss.MModels;
using Service_Layer;

namespace DataManager
{
    public class DataManager
    {
        public string connectionString { get; set; }
        public string Pathtosave { get; set; }
        public string Filename { get; set; }
        public string Schemaname { get; set; }
        public string TargetFolderPath { get; set; }
        public async Task MakeTransaction(int num)
        {
            await Task.Run(async() =>
            {
                try
                {
                    IFiller<PersonalInfo> filler = new Repository(connectionString, num);

                    IxmlGeneratorService ser1 = new XmlPerson();

                    SearchRes<PersonalInfo> info = await Task.Run(() => filler.GetPersonsAsync());
                    await ser1.GenerateXmlFileAsync(info, Pathtosave, Filename);
                    await ser1.GenerateXsdSchemaAsync(Pathtosave, Filename, Schemaname);
                }
                catch (Exception ex)
                {
                    IErLogger logger = new Logger();
                    await logger.WriteErrorAsync(ex);
                }
            });
        }
        public async Task SendInfo()
        {
            await Task.Run(async() =>
            {
                try
                {
                    IFileTransferService ser1 = new Transactions();
                    await ser1.TransferFiles(Filename, Schemaname, Pathtosave, TargetFolderPath);
                }
                catch (Exception ex)
                {
                    IErLogger logger = new Logger();
                    await logger.WriteErrorAsync(ex);
                }
            });
        }
    }
}
