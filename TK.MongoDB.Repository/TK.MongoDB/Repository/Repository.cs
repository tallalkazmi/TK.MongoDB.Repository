using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TK.MongoDB
{
    public class Repository<T> : Settings, IRepository<T> where T : IBaseEntity<ObjectId>
    {
        protected MongoDBContext Context { get; private set; }
        protected IMongoCollection<T> Collection { get; private set; }
        protected string CollectionName { get; private set; }

        public Repository()
        {
            Context = new MongoDBContext(ConnectionStringSettingName);
            CollectionName = typeof(T).Name.ToLower();
            Collection = Context.Database.GetCollection<T>(CollectionName);

            //Create index for CreationDate (descending) and Expires after 'ExpireAfterSecondsTimeSpan'
            var indexBuilder = Builders<T>.IndexKeys;
            var indexModel = new CreateIndexModel<T>(indexBuilder.Descending(x => x.CreationDate), new CreateIndexOptions { ExpireAfter = ExpireAfterSecondsTimeSpan });
            Collection.Indexes.CreateOneAsync(indexModel);
        }

        public void InitCollection()
        {
            Context.Database.DropCollection(CollectionName);
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> condition)
        {
            var query = await Collection.FindAsync<T>(condition);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(ObjectId id)
        {
            return await FindAsync(o => o.Id == id);
        }

        public async Task<Tuple<IEnumerable<T>, long>> GetAsync(int currentPage, int pageSize, Expression<Func<T, bool>> condition = null)
        {
            if (condition == null) condition = _ => true;
            var query = Collection.Find<T>(condition);
            long totalCount = await query.CountDocumentsAsync();
            List<T> records = await query.SortByDescending(x => x.CreationDate).Skip((currentPage - 1) * pageSize).Limit(pageSize).ToListAsync();
            return new Tuple<IEnumerable<T>, long>(records, totalCount);

            //IAsyncCursor<T> query;
            //if (condition == null) condition = _ => true;
            //query = await Collection.Find<T>(condition)
            //                         .Limit(Limit)
            //                         .ToCursorAsync();
            //return await query.ToListAsync();
        }

        public async Task<T> InsertAsync(T instance)
        {
            instance.Id = ObjectId.GenerateNewId();
            instance.CreationDate = DateTime.UtcNow;
            await Collection.InsertOneAsync(instance);
            return instance;
        }

        public async Task UpdateAsync(T instance)
        {
            Expression<Func<T, bool>> filter = x => x.Id == instance.Id;
            var update = new ObjectUpdateDefinition<T>(instance);
            await Collection.UpdateOneAsync<T>(filter, update);
        }

        public async Task DeleteAsync(ObjectId id, bool logical = true)
        {
            Expression<Func<T, bool>> filter = x => x.Id == id;
            if (logical)
            {
                var update = new JsonUpdateDefinition<T>("deleted:'true'");
                await Collection.UpdateOneAsync<T>(filter, update);
            }
            else
                Collection.DeleteOne<T>(filter);
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> condition = null)
        {
            if (condition == null) condition = _ => true;
            return await Collection.CountDocumentsAsync(condition);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> condition)
        {
            var result = await CountAsync(condition);
            return result > 0;
        }

        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
        }
    }
}
