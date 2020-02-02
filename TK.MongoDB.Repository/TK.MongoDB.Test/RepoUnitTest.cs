using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TK.MongoDB.Test
{
    [TestClass]
    public class RepoUnitTest
    {
        public RepoUnitTest()
        {
            Settings.Configure(2592000, "MongoDocConnection");
        }

        [TestMethod]
        public void Find()
        {
            Repository<Activity> repository = new Repository<Activity>();
            Activity result = repository.FindAsync(x => x.Id == new ObjectId("5e36997898d2c15a400f8968")).Result;
            Console.WriteLine($"Output:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void GetById()
        {
            Repository<Activity> repository = new Repository<Activity>();
            Activity result = repository.GetAsync(new ObjectId("5e36997898d2c15a400f8968")).Result;
            Console.WriteLine($"Output:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");

        }

        [TestMethod]
        public void Get()
        {
            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.GetAsync(1, 10, x => x.Name.Contains("abc") && x.Deleted == false).Result;
            Console.WriteLine($"Output:\nTotal: {result.Item2}\n{JToken.Parse(JsonConvert.SerializeObject(result.Item1)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void Insert()
        {
            Activity activity = new Activity()
            {
                Name = "abc"
            };

            Repository<Activity> repository = new Repository<Activity>();
            Activity result = repository.InsertAsync(activity).Result;
            Console.WriteLine($"Inserted:\n{JToken.Parse(JsonConvert.SerializeObject(result)).ToString(Formatting.Indented)}");
        }

        [TestMethod]
        public void Update()
        {
            Activity activity = new Activity()
            {
                Id = new ObjectId("5e36998998d2c1540ca23894"),
                Name = "abc3",
                
            };

            Repository<Activity> repository = new Repository<Activity>();
            bool result = repository.UpdateAsync(activity).Result;
            Console.WriteLine($"Updated: {result}");
        }

        [TestMethod]
        public void Delete()
        {
            Repository<Activity> repository = new Repository<Activity>();
            bool result = repository.DeleteAsync(new ObjectId("5e36998998d2c1540ca23894")).Result;
            Console.WriteLine($"Deleted: {result}");
        }

        [TestMethod]
        public void Count()
        {
            Repository<Activity> repository = new Repository<Activity>();
            long result = repository.CountAsync().Result;
            Console.WriteLine($"Count: {result}");
        }

        [TestMethod]
        public void Exists()
        {
            Repository<Activity> repository = new Repository<Activity>();
            bool result = repository.ExistsAsync(x => x.Name == "abc").Result;
            Console.WriteLine($"Exists: {result}");
        }
    }
}
