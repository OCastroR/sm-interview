using SmartVault.BusinessLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.BusinessLogic
{
    public class FileManagement : IFileManagement
    {
        public long GetFileLenght(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        public string ReadAll(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public void WriteAll(string fileName, string fileContent)
        {
            File.WriteAllText(fileName, fileContent);
        }
    }
}
