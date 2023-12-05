using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SmartVault.Library;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Transactions;
using System.Xml.Serialization;

namespace SmartVault.DataGeneration
{
    partial class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            SQLiteConnection.CreateFile(configuration["DatabaseFileName"]);
            File.WriteAllText("TestDoc.txt", $"This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}This is my test document{Environment.NewLine}");

            using (var connection = new SQLiteConnection(string.Format(configuration?["ConnectionStrings:DefaultConnection"] ?? "", configuration?["DatabaseFileName"])))
            {
                connection.Open();

                var files = Directory.GetFiles(@"..\..\..\..\BusinessObjectSchema");
                for (int i = 0; i <= 2; i++)
                {
                    var serializer = new XmlSerializer(typeof(BusinessObject));
                    var businessObject = serializer.Deserialize(new StreamReader(files[i])) as BusinessObject;
                    connection.Execute(businessObject?.Script);

                }
                BulkInsertAccounts(configuration?["ConnectionStrings:DefaultConnection"], configuration?["DatabaseFileName"]);
                BulkInsertUsers(configuration?["ConnectionStrings:DefaultConnection"], configuration?["DatabaseFileName"]);
                BulkInsertDocuments(configuration?["ConnectionStrings:DefaultConnection"], configuration?["DatabaseFileName"]);

                var accountData = connection.Query("SELECT COUNT(*) FROM Account;");
                Console.WriteLine($"AccountCount: {JsonConvert.SerializeObject(accountData)}");
                var documentData = connection.Query("SELECT COUNT(*) FROM Document;");
                Console.WriteLine($"DocumentCount: {JsonConvert.SerializeObject(documentData)}");
                var userData = connection.Query("SELECT COUNT(*) FROM User;");
                Console.WriteLine($"UserCount: {JsonConvert.SerializeObject(userData)}");
            }
        }

        static void BulkInsertDocuments(string connectionString, string databaseFileName)
        {
            try
            {
                using (var connection = new SQLiteConnection(string.Format(connectionString ?? "", databaseFileName)))
                {

                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;

                            command.CommandText = @"INSERT INTO Document (Id, Name, FilePath, Length, AccountId) VALUES ($Id, $Name, $FilePath, $Length, $AccountId)";

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

                            var accountIdParam = command.CreateParameter();
                            accountIdParam.ParameterName = "$AccountId";
                            command.Parameters.Add(accountIdParam);

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
                                    accountIdParam.Value = i;
                                    command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        static void BulkInsertUsers(string connectionString, string databaseFileName)
        {
            try
            {
                using (var connection = new SQLiteConnection(string.Format(connectionString ?? "", databaseFileName)))
                {

                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            command.Connection = connection;
                            command.Transaction = transaction;

                            command.CommandText = @"INSERT INTO User (Id, FirstName, LastName, DateOfBirth, AccountId, Username, Password) VALUES ($Id, $FirstName, $LastName, $DateOfBirth, $AccountId, $Username, $Password)";

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
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                }
            }
            catch
            {

            }

        }

        static void BulkInsertAccounts(string connectionString, string databaseFileName)
        {
            try
            {
                using (var connection = new SQLiteConnection(string.Format(connectionString ?? "", databaseFileName)))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        using (var transaction = connection.BeginTransaction())
                        {
                            command.CommandText = @"INSERT INTO Account (Id, Name) VALUES ($Id, $Name)";

                            var id = command.CreateParameter();
                            id.ParameterName = "$Id";
                            command.Parameters.Add(id);

                            var name = command.CreateParameter();
                            name.ParameterName = "$Name";
                            command.Parameters.Add(name);

                            for (int i = 0; i < 100; i++)
                            {
                                id.Value = i;
                                name.Value = $"Account{i}";
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }
                }
            }
            catch
            {

            }

        }

        static IEnumerable<DateTime> RandomDay()
        {
            DateTime start = new DateTime(1985, 1, 1);
            Random gen = new Random();
            int range = (DateTime.Today - start).Days;
            while (true)
                yield return start.AddDays(gen.Next(range));
        }
    }
}