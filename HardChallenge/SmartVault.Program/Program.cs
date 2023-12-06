using SmartVault.BusinessLogic;
using System;


namespace SmartVault.Program
{
    partial class Program
    {
        static void Main(string[] args)
        {
           
            WriteEveryThirdFileToFile(1);
            GetAllFileSizes();
        }

        private static void GetAllFileSizes()
        {
            var fileService = new FileService();
            long totalFileSize = fileService.GetAllFileSizesInBytes();
            Console.WriteLine($"Size of all files: {totalFileSize / (1024 * 1024)} Mb");
        }

        private static void WriteEveryThirdFileToFile(int accountId)
        {
            var fileService = new FileService();
            fileService.WriteEveryThirdFileToFile(accountId, "test");

        }
    }
}