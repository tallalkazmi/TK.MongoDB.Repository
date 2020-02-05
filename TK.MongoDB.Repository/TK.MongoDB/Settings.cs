using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using TK.MongoDB.Models;

namespace TK.MongoDB
{
    public class Settings
    {
        protected static string ConnectionStringSettingName = "MongoDocConnection";

        /// <summary>
        /// Configure connection string setting name added in *.Config
        /// </summary>
        /// <param name="connectionStringSettingName">Connection String name from *.config file. Default value is set to <i>MongoDocConnection</i></param>
        public static void Configure(string connectionStringSettingName = null)
        {
            if (!string.IsNullOrWhiteSpace(connectionStringSettingName)) ConnectionStringSettingName = connectionStringSettingName;
        }

        /// <summary>
        /// Configure document expiry index.
        /// </summary>
        /// <typeparam name="T">Collection model</typeparam>
        /// <param name="expireAfterSeconds">Documents expire after seconds</param>
        public static void Configure<T>(int expireAfterSeconds) where T : BaseEntity<ObjectId>
        {
            MongoDBContext Context = new MongoDBContext(ConnectionStringSettingName);
            IMongoCollection<T> Collection = Context.Database.GetCollection<T>(typeof(T).Name.ToLower());

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
