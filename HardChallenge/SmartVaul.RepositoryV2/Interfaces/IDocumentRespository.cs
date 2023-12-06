using System.Collections.Generic;

namespace SmartVaul.Repository.Interfaces
{
    public interface IDocumentRespository
    {
        public int BulkInsert();
        public IEnumerable<string> GetAllFilesPath();
        public IEnumerable<(int id, string filePath)> GetFilePath(int accountId);
    }
}
