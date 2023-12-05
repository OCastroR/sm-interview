using Dapper;
using SmartVault.Program.BusinessObjects;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

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
            long accountsTotalSize = 0;
            using (var connection = new SQLiteConnection("data source=testdb.sqlite"))
            {
                var accountsData = connection.Query<BusinessObjects.Document>($"SELECT * FROM Document");
                foreach (var accountData in accountsData)
                {
                    long accountLength = new FileInfo(accountData.FilePath).Length;
                    accountsTotalSize = accountLength + accountsTotalSize;
                }
                Console.WriteLine($"Size of all files: {accountsTotalSize / (1024 * 1024)} Mb");

            }
        }

        private static void WriteEveryThirdFileToFile(int accountId)
        {
            using (var connection = new SQLiteConnection("data source=testdb.sqlite"))
            {
                var accountsData = connection.Query<BusinessObjects.Document> ($"SELECT * FROM Document WHERE AccountId = {accountId}");
                foreach (var accountData in accountsData)
                {
                    string accountFileContent = File.ReadAllText(accountData.FilePath);
                    bool containsString = accountFileContent.Contains("test") ? true : false;
                    if (containsString) File.WriteAllText($"Account-{accountData.Id}.txt", accountFileContent);
                }
                
            }
                
        }
    }
}