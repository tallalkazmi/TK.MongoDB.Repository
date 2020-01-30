using MongoDB.Driver;
using System;
using System.Configuration;

namespace TK.MongoDB
{
    public class MongoDBContext : IDisposable
    {
        readonly string DatabaseName;
        MongoClient Client;

        /// <summary>
        /// Creates an instance of IMongoDatabase from connection string provided
        /// </summary>
        /// <param name="connStringName">Setting name to read from 'connectionStrings' section of Web.config</param>
        public MongoDBContext(string connStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            DatabaseName = new MongoUrl(connectionString).DatabaseName;
            Client = new MongoClient(connectionString);
        }

        /// <summary>
        /// Represents a database of type IMongoDatabase in MongoDB
        /// </summary>
        public IMongoDatabase Database
        {
            get { return Client.GetDatabase(DatabaseName); }
        }

        #region Dispose
        protected bool Disposed { get; private set; }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                if (disposing)
                {
                    if (Client != null)
                        Client = null;
                }
                this.Disposed = true;
            }
        }
        #endregion
    }
}
