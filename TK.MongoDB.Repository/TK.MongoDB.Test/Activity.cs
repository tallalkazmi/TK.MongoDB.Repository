using MongoDB.Bson;
using TK.MongoDB.Models;

namespace TK.MongoDB.Test
{
    public class Activity : BaseEntity<ObjectId>
    {
        public string Name { get; set; }
    }
}
