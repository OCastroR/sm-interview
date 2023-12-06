using System;
using SQLiteContext = SmartVault.SQLiteRepository.SQLiteContext;

namespace SmartVault.Repository
{
    public class AccountRepository : SQLiteContext
    {
        public AccountRepository(string connectionString) : base(connectionString)
        {
        }

        public int BulkInsert()
        {
            int accountCount = 0;
            try
            {
                using (_dbConn)
                {
                    Open();
                    using (var command = _dbConn.CreateCommand())
                    {
                        using (var transaction = _dbConn.BeginTransaction())
                        {
                            command.CommandText = @"INSERT INTO Account (Id, Name, CreatedOn) VALUES ($Id, $Name, $CreatedOn)";

                            var id = command.CreateParameter();
                            id.ParameterName = "$Id";
                            command.Parameters.Add(id);

                            var name = command.CreateParameter();
                            name.ParameterName = "$Name";
                            command.Parameters.Add(name);

                            var createdOn = command.CreateParameter();
                            createdOn.ParameterName = "$CreatedOn";
                            command.Parameters.Add(createdOn);

                            for (int i = 0; i < 100; i++)
                            {
                                id.Value = i;
                                name.Value = $"Account{i}";
                                createdOn.Value = DateTime.UtcNow;
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                    accountCount = Select<int>("SELECT COUNT(*) FROM Account;");
                }
            }
            catch
            {

            }

            return accountCount;

        }


    }
}
