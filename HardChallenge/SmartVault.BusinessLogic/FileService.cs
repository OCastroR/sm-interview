using SmartVaul.Repository.Interfaces;
using SmartVault.BusinessLogic.Interfaces;
using System.Collections.Generic;

namespace SmartVault.BusinessLogic
{
    public class FileService
    {
        private IFileManagement _fileManagementService;
        private IDocumentRespository _documentRepository;

        public FileService(IFileManagement fileManagement, IDocumentRespository documentRespository)
        {
            _fileManagementService = fileManagement;
            _documentRepository = documentRespository;
            
        }
        public long GetAllFileSizesInBytes()
        {
            long documentsTotalSize = 0;
            var accountsFilePaths = _documentRepository.GetAllFilesPath();
            foreach (var filePath in accountsFilePaths)
            {
                long documentLength = _fileManagementService.GetFileLenght(filePath);
                documentsTotalSize = documentLength + documentsTotalSize;
            }
            return documentsTotalSize;
        }

        public void WriteEveryThirdFileToFile(int accountId, string stringToMatch)
        {
            IEnumerable<(int id, string filePath)> documentsData = _documentRepository.GetFilePath(accountId);
            int fileCounter = 1;
            foreach (var accountData in documentsData)
            {
                if ((fileCounter % 3) == 0)
                {
                    string accountFileContent = _fileManagementService.ReadAll(accountData.filePath);
                    bool containsString = accountFileContent.Contains(stringToMatch) ? true : false;
                    if (containsString) _fileManagementService.WriteAll($"Account-{accountData.id}.txt", accountFileContent);
                }

                fileCounter++;
            }

        }
    }
}
