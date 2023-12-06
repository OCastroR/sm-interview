using System;
using System.Collections.Generic;
using SQLiteContext = SmartVault.SQLiteRepository.SQLiteContext;

namespace SmartVaul.Repository
{
    public class UserRepository : SQLiteContext
    {
        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        public int BulkInsert()
        {
            int userCount = 0;
            try
            {
                using (_dbConn)
                {

                    Open();
                    using (var command = _dbConn.CreateCommand())
                    {
                        using (var transaction = _dbConn.BeginTransaction())
                        {
                            command.Connection = _dbConn;
                            command.Transaction = transaction;

                            command.CommandText = @"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password, CreatedOn) VALUES ($Id, $FirstName, $LastName, $DateOfBirth, $AccountId, $Username, $Password, $CreatedOn)";

                            var id = command.CreateParameter();
                            id.ParameterName = "$Id";
                            command.Parameters.Add(id);

                            var firstName = command.CreateParameter();
                            firstName.ParameterName = "$FirstName";
                            command.Parameters.Add(firstName);

                            var lastName = command.CreateParameter();
                            lastName.ParameterName = "$LastName";
                            command.Parameters.Add(lastName);

                            var dateOfBirth = command.CreateParameter();
                            dateOfBirth.ParameterName = "$DateOfBirth";
                            command.Parameters.Add(dateOfBirth);

                            var accountIdParameter = command.CreateParameter();
                            accountIdParameter.ParameterName = "$AccountId";
                            command.Parameters.Add(accountIdParameter);

                            var userName = command.CreateParameter();
                            userName.ParameterName = "$Username";
                            command.Parameters.Add(userName);

                            var password = command.CreateParameter();
                            password.ParameterName = "$Password";
                            command.Parameters.Add(password);

                            var createdOn = command.CreateParameter();
                            createdOn.ParameterName = "$CreatedOn";
                            command.Parameters.Add(createdOn);

                            for (int i = 0; i < 100; i++)
                            {
                                var randomDayIterator = RandomDay().GetEnumerator();
                                randomDayIterator.MoveNext();

                                id.Value = i;
                                firstName.Value = $"FName{i}";
                                lastName.Value = $"LName{i}";
                                dateOfBirth.Value = randomDayIterator.Current.ToString("yyyy-MM-dd");
                                accountIdParameter.Value = i;
                                userName.Value = $"UserName-{i}";
                                password.Value = "e10adc3949ba59abbe56e057f20f883e";
                                createdOn.Value = DateTime.UtcNow;
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }

                    userCount = Select<int>("SELECT COUNT(*) FROM User;");
                }
            }
            catch
            {
                throw;
            }

            return userCount;

        }

        private IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}
