using MongoDB.Bson;
using System;

namespace TK.MongoDB.Test
{
    public class Activity : IBaseEntity<ObjectId>
    {
        public ObjectId Id { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
