using System;

namespace TK.MongoDB
{
    public class Settings
    {
        protected static string ConnectionStringSettingName = "MongoDocConnection";
        protected static TimeSpan ExpireAfterSecondsTimeSpan = new TimeSpan(30, 0, 0, 0); //2592000 Seconds

        /// <summary>
        /// Configure connection string and Expire after seconds index
        /// </summary>
        /// <param name="expireAfterSeconds">Expire after seconds idex. Default is set to 2592000 seconds or 30 days</param>
        /// /// <param name="connectionStringSettingName">Connection String name from *.config file</param>
        public static void Configure(int expireAfterSeconds = 2592000, string connectionStringSettingName = null)
        {
            if (!string.IsNullOrWhiteSpace(connectionStringSettingName)) ConnectionStringSettingName = connectionStringSettingName;
            ExpireAfterSecondsTimeSpan = new TimeSpan(TimeSpan.TicksPerSecond * expireAfterSeconds);
        }
    }
}
