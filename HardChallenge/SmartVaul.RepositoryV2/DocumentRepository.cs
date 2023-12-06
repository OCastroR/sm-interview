using System;
using System.Collections.Generic;
using System.IO;
using SQLiteContext = SmartVault.SQLiteRepository.SQLiteContext;

namespace SmartVaul.Repository
{
    public class DocumentRepository : SQLiteContext
    {
        public DocumentRepository(string connectionString) : base(connectionString)
        {
        }

        public int BulkInsert()
        {
            int documentCount = 0;
            try
            {
                using (_dbConn)
                {

                    _dbConn.Open();
                    using (var command = _dbConn.CreateCommand())
                    {
                        using (var transaction = _dbConn.BeginTransaction())
                        {
                            command.Connection = _dbConn;
                            command.Transaction = transaction;

                            command.CommandText = @"INSERT INTO Document (Id, Name, FilePath, Length, AccountId, CreatedOn) VALUES ($Id, $Name, $FilePath, $Length, $AccountId, $CreatedOn)";

                            var id = command.CreateParameter();
                            id.ParameterName = "$Id";
                            command.Parameters.Add(id);

                            var name = command.CreateParameter();
                            name.ParameterName = "$Name";
                            command.Parameters.Add(name);

                            var filePath = command.CreateParameter();
                            filePath.ParameterName = "$FilePath";
                            command.Parameters.Add(filePath);

                            var length = command.CreateParameter();
                            length.ParameterName = "$Length";
                            command.Parameters.Add(length);

                            var accountId = command.CreateParameter();
                            accountId.ParameterName = "$AccountId";
                            command.Parameters.Add(accountId);

                            var createdOn = command.CreateParameter();
                            createdOn.ParameterName = "$CreatedOn";
                            command.Parameters.Add(createdOn);

                            int documentNumber = 0;

                            for (int i = 0; i < 100; i++)
                            {
                                for (int d = 0; d < 10000; d++, documentNumber++)
                                {
                                    var documentPath = new FileInfo("TestDoc.txt").FullName;
                                    id.Value = documentNumber;
                                    name.Value = $"Document{i}-{d}.txt";
                                    filePath.Value = documentPath;
                                    length.Value = new FileInfo(documentPath).Length;
                                    accountId.Value = i;
                                    createdOn.Value = DateTime.UtcNow;
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                    }

                    documentCount = Select<int>("SELECT COUNT(*) FROM Document;");
                }
            }
            catch
            {
                throw;
            }

            return documentCount;
        }

        public long GetAllFileSizesInBytes()
        {
            var sql = @"SELECT FilePath FROM Document";
            long accountsTotalSize = 0;
            var accountsFilePaths = SelectList<string>(sql);
            foreach (var filePath in accountsFilePaths)
            {
                long accountLength = new FileInfo(filePath).Length;
                accountsTotalSize = accountLength + accountsTotalSize;
            }
            return accountsTotalSize;
        }

        public void WriteEveryThirdFileToFile(int accountId, string stringToMatch)
        {
            var sql = @"SELECT Id, FilePath FROM Document WHERE AccountId = @accountId";
            IEnumerable<(int id, string filePath)> accountsData = SelectList<(int id, string filePath)>(sql, new { accountId });
            int fileCounter = 0;
            foreach (var accountData in accountsData)
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
