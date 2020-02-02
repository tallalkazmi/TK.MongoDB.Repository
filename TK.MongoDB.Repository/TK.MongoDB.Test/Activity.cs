using MongoDB.Bson;

namespace TK.MongoDB.Test
{
    public class Activity : BaseEntity<ObjectId>
    {
        public string Name { get; set; }
    }
}
