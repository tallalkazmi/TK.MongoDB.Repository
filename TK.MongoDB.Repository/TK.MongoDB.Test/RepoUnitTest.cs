using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;

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
            var result = repository.FindAsync(x => x.Id == new ObjectId("5e32e13898d2c16694b9b931")).Result;
        }

        [TestMethod]
        public void GetById()
        {
            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.GetAsync(new ObjectId("5e32e13898d2c16694b9b931")).Result;
        }

        [TestMethod]
        public void Get()
        {
            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.GetAsync(1, 10, x => x.Name.Contains("abc") && x.Deleted == false).Result;
        }

        [TestMethod]
        public void Insert()
        {
            Activity activity = new Activity()
            {
                Name = "abc"
            };

            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.InsertAsync(activity).Result;
        }

        [TestMethod]
        public void Update()
        {
            Activity activity = new Activity()
            {
                Id = new ObjectId("5e32e13898d2c16694b9b931"),
                Name = "abc2"
            };

            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.UpdateAsync(activity);
        }

        [TestMethod]
        public void Delete()
        {
            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.DeleteAsync(new ObjectId("5e32e13898d2c16694b9b931"));
        }

        [TestMethod]
        public void Count()
        {
            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.CountAsync().Result;
        }

        [TestMethod]
        public void Exists()
        {
            Repository<Activity> repository = new Repository<Activity>();
            var result = repository.ExistsAsync(x => x.Name == "5e32e0f598d2c15544e1ee0b");
        }
    }
}
