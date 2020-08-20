using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TK.MongoDB.Test
{
    [TestClass]
    public class RepoUnitTest : BaseTest
    {
        public RepoUnitTest()
        {
            Settings.ConnectionStringSettingName = "MongoDocConnection";
            Settings.Configure<Activity>(2592000);
        }

        [TestMethod]
        public void Find()
        {
            Activity result = ActivityRepository.FindAsync(x => x.Id == new ObjectId("5e36997898d2c15a400f8968")).Result;
            Console.WriteLine($"Output:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void GetById()
        {
            Activity result = ActivityRepository.GetAsync(new ObjectId("5e36997898d2c15a400f8968")).Result;
            Console.WriteLine($"Output:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void Get()
        {
            var result = ActivityRepository.GetAsync(1, 10, x => x.Name.Contains("abc") && x.Deleted == false).Result;
            Console.WriteLine($"Output:\nTotal: {result.Item2}\n{JToken.Parse(JsonConvert.SerializeObject(result.Item1)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void GetNonPaged()
        {
            var result = ActivityRepository.Get(x => x.Name.Contains("abc") && x.Deleted == false);
            Console.WriteLine($"Output:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void In()
        {
            List<string> names = new List<string> { "abc", "def", "ghi" };
            var result = ActivityRepository.In(x => x.Name, names);
            Console.WriteLine($"Output:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void Insert()
        {
            Activity activity = new Activity()
            {
                Name = "abc"
            };

            Activity result = ActivityRepository.InsertAsync(activity).Result;
            Console.WriteLine($"Inserted:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");
        }

        //[TestMethod]
        public void Update()
        {
            Activity activity = new Activity()
            {
                Id = new ObjectId("5e36998998d2c1540ca23894"),
                Name = "abc3"
            };

            bool result = ActivityRepository.UpdateAsync(activity).Result;
            Console.WriteLine($"Updated: {result}");
        }

        //[TestMethod]
        public void Delete()
        {
            bool result = ActivityRepository.DeleteAsync(new ObjectId("5e36998998d2c1540ca23894")).Result;
            Console.WriteLine($"Deleted: {result}");
        }

        [TestMethod]
        public void Count()
        {
            long result = ActivityRepository.CountAsync().Result;
            Console.WriteLine($"Count: {result}");
        }

        [TestMethod]
        public void Exists()
        {
            bool result = ActivityRepository.ExistsAsync(x => x.Name == "abc").Result;
            Console.WriteLine($"Exists: {result}");
        }
    }
}
