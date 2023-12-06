using SmartVaul.Repository;
using System;


namespace SmartVault.Program
{
    partial class Program
    {
        static string connectionString = "data source=testdb.sqlite";

        static void Main(string[] args)
        {
           
            WriteEveryThirdFileToFile(1);
            GetAllFileSizes();
        }

        private static void GetAllFileSizes()
        {
            var documentRepository = new DocumentRepository(connectionString);
            long totalFileSize = documentRepository.GetAllFileSizesInBytes();
            Console.WriteLine($"Size of all files: {totalFileSize / (1024 * 1024)} Mb");
        }

        private static void WriteEveryThirdFileToFile(int accountId)
        {
            var documentRepository = new DocumentRepository(connectionString);
            documentRepository.WriteEveryThirdFileToFile(accountId, "Smith Property");

        }
    }
}