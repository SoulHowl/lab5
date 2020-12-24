using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess_Layer;
using Modelss.MModels;
namespace Service_Layer
{
    public interface IxmlGeneratorService
    {
        public Task GenerateXmlFileAsync(SearchRes<PersonalInfo> data, string Pathtosave, string Filename);

        public Task GenerateXsdSchemaAsync(string Pathtosave, string Filename, string Shemaname);


    }

    public interface IFileTransferService
    {
        public Task TransferFiles(string filename, string xsdschema, string sourcefolder, string TargetFolderPath);
    }
}
