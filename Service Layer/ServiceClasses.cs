using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using DataAccess_Layer;
using ErrorLogger;
using Modelss.MModels;

namespace Service_Layer
{
    public class XmlPerson : IxmlGeneratorService
    {

        public async Task GenerateXmlFileAsync(SearchRes<PersonalInfo> data, string Pathtosave, string Filename)
        {
            await Task.Run(() =>
            {
                List<PersonalInfo> list = new List<PersonalInfo>();
                foreach (var variable in data.Entities)
                {
                    list.Add(variable);
                }

                XmlSerializer formatter = new XmlSerializer(typeof(List<PersonalInfo>));
                using (FileStream fs = new FileStream(Path.Combine(Pathtosave, Filename), FileMode.Append))
                {
                    formatter.Serialize(fs, list);
                }
            });

        }

        public async Task GenerateXsdSchemaAsync(string pathtosave, string filename, string shemaname)
        {
            await Task.Run(() =>
            {
                XmlReader reader = XmlReader.Create(Path.Combine(pathtosave, filename));
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                XmlSchemaInference schema = new XmlSchemaInference();
                schemaSet = schema.InferSchema(reader);
                reader.Dispose();

                XmlSerializer formatter = new XmlSerializer(typeof(XmlSchema));

                using (FileStream fs = new FileStream(Path.Combine(pathtosave, shemaname), FileMode.OpenOrCreate))
                {
                    foreach (XmlSchema s in schemaSet.Schemas())
                    {
                        formatter.Serialize(fs, s);

                    }
                }
            });
        }
    }

    public class Transactions : IFileTransferService
    {

        public async Task TransferFiles(string filename, string xsdschema, string sourcefolder, string TargetFolderPath)
        {
            await Task.Run(async() =>
            {
                try
                {
                    string str = DateTime.Now.ToString();
                    str = str.Replace(' ', '_');
                    str = str.Replace('.', '_');
                    str = str.Replace(':', '_');
                    var k = '.';
                    var i = filename.Length - 1;
                    while (k != filename[i])
                        i--;

                    string file = filename.Insert(i, str);

                    FileInfo file1 = new FileInfo(Path.Combine(sourcefolder, filename));
                    file1.MoveTo(Path.Combine(TargetFolderPath, file));

                    FileInfo file2 = new FileInfo(Path.Combine(sourcefolder, xsdschema));
                    if (File.Exists(Path.Combine(TargetFolderPath, xsdschema)))
                    {
                       File.Delete(Path.Combine(TargetFolderPath, xsdschema));
                    }
                    file2.MoveTo(Path.Combine(TargetFolderPath, xsdschema));
                }
                catch (Exception e)
                {
                    IErLogger logger = new Logger();
                    await logger.WriteErrorAsync(e);
                }
            });



        }
    }

}
