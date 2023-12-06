using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.BusinessLogic.Interfaces
{
    public interface IFileManagement
    {
        public string ReadAll(string filePath);
        public void WriteAll(string fileName, string fileContent);
        public long GetFileLenght(string filePath);
    }
}
