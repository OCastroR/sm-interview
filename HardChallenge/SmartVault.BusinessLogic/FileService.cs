using SmartVaul.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVault.BusinessLogic
{
    public class FileService
    {
        public long GetAllFileSizesInBytes()
        {
            var documentRepository = new DocumentRepository();
            long accountsTotalSize = 0;
            var accountsFilePaths = documentRepository.GetAllFilesPath();
            foreach (var filePath in accountsFilePaths)
            {
                long accountLength = new FileInfo(filePath).Length;
                accountsTotalSize = accountLength + accountsTotalSize;
            }
            return accountsTotalSize;
        }

        public void WriteEveryThirdFileToFile(int accountId, string stringToMatch)
        {
            var documentRepository = new DocumentRepository();
            IEnumerable<(int id, string filePath)> documentsData = documentRepository.GetFilePath(accountId);
            int fileCounter = 0;
            foreach (var accountData in documentsData)
            {
                if ((fileCounter % 3) == 0)
                {
                    string accountFileContent = File.ReadAllText(accountData.filePath);
                    bool containsString = accountFileContent.Contains(stringToMatch) ? true : false;
                    if (containsString) File.WriteAllText($"Account-{accountData.id}.txt", accountFileContent);
                }

                fileCounter++;
            }

        }
    }
}
