using Dapper;
using SmartVault.Library;
using System.IO;
using System.Xml.Serialization;
using SQLiteContext = SmartVault.SQLiteRepository.SQLiteContext;

namespace SmartVaul.Repository
{
    public class DatabaseIntializer : SQLiteContext
    {
        public DatabaseIntializer(string connectionString) : base(connectionString)
        {
        }

        public void CreateDatabaseTables(string[] files) 
        {
            using (_dbConn)
            {
                Open();

                for (int i = 0; i <= 2; i++)
                {
                    var serializer = new XmlSerializer(typeof(BusinessObject));
                    var businessObject = serializer.Deserialize(new StreamReader(files[i])) as BusinessObject;
                    _dbConn.Execute(businessObject?.Script);
                }
            }
        }
    }
}
