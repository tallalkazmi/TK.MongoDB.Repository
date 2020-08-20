using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using TK.MongoDB.Models;

namespace TK.MongoDB
{
    /// <summary>
    /// Database <i>ConnectionString</i>, <i>expireAfterSeconds</i> index, and dependency tracking settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Connection String name from *.config file. Default value is set to <i>MongoDocConnection</i>.
        /// </summary>
        public static string ConnectionStringSettingName { get; set; } = "MongoDocConnection";

        /// <summary>
        /// Commands to NOT track while Dependency Tracking is active.
        /// </summary>
        public static IEnumerable<string> NotTrackedCommands { get; set; } = new[] { "isMaster", "buildInfo", "getLastError", "saslStart", "saslContinue", "listIndexes" };

        /// <summary>
        /// Configure document expiry index.
        /// </summary>
        /// <typeparam name="T">Collection model</typeparam>
        /// <param name="expireAfterSeconds">Documents expire after seconds</param>
        public static void Configure<T>(int expireAfterSeconds) where T : BaseEntity
        {
            string connectionString = ConfigurationManager.ConnectionStrings[ConnectionStringSettingName].ConnectionString;
            string DatabaseName = new MongoUrl(connectionString).DatabaseName;
            MongoClient Client = new MongoClient(connectionString);
            IMongoCollection<T> Collection = Client.GetDatabase(DatabaseName).GetCollection<T>(typeof(T).Name.ToLower());

            TimeSpan timeSpan = new TimeSpan(TimeSpan.TicksPerSecond * expireAfterSeconds);

            //Get all indexes with name like 'creationdate' and drop them
            var indexes = Collection.Indexes.List().ToList();
            var cd_indexes = indexes.Where(x => x.GetValue("name").AsString.ToLower().Contains("creationdate"));
            foreach (var index in cd_indexes)
            {
                string indexName = index.GetValue("name").AsString;
                Collection.Indexes.DropOne(indexName);
            }

            //Create index for CreationDate (descending) and Expires after 'ExpireAfterSecondsTimeSpan'
            var indexBuilder = Builders<T>.IndexKeys;
            var indexModel = new CreateIndexModel<T>(indexBuilder.Descending(x => x.CreationDate), new CreateIndexOptions { ExpireAfter = timeSpan, Name = "CreationDateIndex" });
            Collection.Indexes.CreateOne(indexModel);
        }
    }
}
