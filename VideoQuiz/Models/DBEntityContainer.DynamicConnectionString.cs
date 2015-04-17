using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace VideoQuiz.Models
{
    public partial class DBEntityContainer : DbContext
    {
        public DBEntityContainer(string connectionString)
            : base(connectionString)
        {
        }

        public static DBEntityContainer ConnectToDatabase(string server, string database, string username, string password)
        {
            var builder = new EntityConnectionStringBuilder();

            builder.Provider = "System.Data.SqlClient";

            builder.Metadata = @"res://*/Models.DBEntity.csdl|
                                 res://*/Models.DBEntity.ssdl|
                                 res://*/Models.DBEntity.msl";

            var providerBuilder = new SqlConnectionStringBuilder();
            providerBuilder.UserID = username;
            providerBuilder.Password = password;
            providerBuilder.IntegratedSecurity = false;
            providerBuilder.DataSource = server;
            providerBuilder.InitialCatalog = database;

            string providerString = providerBuilder.ToString();
            builder.ProviderConnectionString = providerString;

            return new DBEntityContainer(builder.ToString());
        }
    }
}