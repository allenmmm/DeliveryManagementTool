//using Microsoft.VisualBasic.FileIO;
using DMT.SharedKernel.Interface;
using ExcelDataReader;
using SharedKernel.Test.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Test
{
    public static class FileReadUtility
    {
        public static void FileRead(IProcessData processData,
                                     string specificTestDataFile = "")
        {
  
            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string absolouteTestDataFullPath = "";
            if (specificTestDataFile == "")
            {
                absolouteTestDataFullPath = assemblyPath.Substring(0, assemblyPath.IndexOf(Assembly.GetCallingAssembly().GetName().Name));
                absolouteTestDataFullPath += "\\SharedKernel.Test\\TestData\\MIS PLY Purchasing Data-201941_031019_144839.csv";

            }
            else
            {
                absolouteTestDataFullPath = assemblyPath.Substring(0, assemblyPath.IndexOf("\\bin")) + "\\TestData\\" + specificTestDataFile;
            }
            using (var stream = File.Open(absolouteTestDataFullPath, FileMode.Open, FileAccess.Read))
            {
                var reader = ExcelReaderFactory.CreateCsvReader(stream);
                reader.Read();
                    
                while (reader.Read())
                {
                    List<String> rawData = new List<String>();
                         
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rawData.Add(reader.GetString(i));
                    }
                    processData.ProcessLine(rawData);
                }
            }
        }
    }
}
