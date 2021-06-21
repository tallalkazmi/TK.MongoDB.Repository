using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TK.MongoDB.Interfaces;
using TK.MongoDB.Models;

namespace TK.MongoDB.Data
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly DbContext Context;
        protected IMongoCollection<T> Collection { get; private set; }
        protected string CollectionName { get; private set; }

        public Repository()
        {
            if (Context == null) Context = new DbContext();
            CollectionName = typeof(T).Name.ToLower();
            Collection = Context.Database.GetCollection<T>(CollectionName);

            //Create index for CreationDate (descending), if it does not exists
            var indexes = Collection.Indexes.List().ToList();
            bool DoesIndexExists = indexes.Any(x => x.GetValue("name").AsString == "CreationDateIndex");
            if (!DoesIndexExists)
            {
                var indexBuilder = Builders<T>.IndexKeys;
                var indexModel = new CreateIndexModel<T>(indexBuilder.Descending(x => x.CreationDate), new CreateIndexOptions { Name = "CreationDateIndex" });
                Collection.Indexes.CreateOneAsync(indexModel);
            }
        }

        public Repository(IDependencyTracker dependencyTracker)
        {
            if (Context == null) Context = new DbContext(dependencyTracker);
            CollectionName = typeof(T).Name.ToLower();
            Collection = Context.Database.GetCollection<T>(CollectionName);

            //Create index for CreationDate (descending), if it does not exists
            var indexes = Collection.Indexes.List().ToList();
            bool DoesIndexExists = indexes.Any(x => x.GetValue("name").AsString == "CreationDateIndex");
            if (!DoesIndexExists)
            {
                var indexBuilder = Builders<T>.IndexKeys;
                var indexModel = new CreateIndexModel<T>(indexBuilder.Descending(x => x.CreationDate), new CreateIndexOptions { Name = "CreationDateIndex" });
                Collection.Indexes.CreateOneAsync(indexModel);
            }
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
        public T Find(Expression<Func<T, bool>> condition)
        {
            var query = Collection.Find<T>(condition);
            return query.FirstOrDefault();
        }

        public async Task<T> GetAsync(ObjectId id)
        {
            return await FindAsync(o => o.Id == id);
        }
        public T Get(ObjectId id)
        {
            return Find(o => o.Id == id);
        }

        public async Task<Tuple<IEnumerable<T>, long>> GetAsync(int currentPage, int pageSize, Expression<Func<T, bool>> condition = null)
        {
            if (condition == null) condition = _ => true;
            var query = Collection.Find<T>(condition);
            long totalCount = await query.CountDocumentsAsync();
            List<T> records = await query.SortByDescending(x => x.CreationDate).Skip((currentPage - 1) * pageSize).Limit(pageSize).ToListAsync();
            return new Tuple<IEnumerable<T>, long>(records, totalCount);
        }
        public Tuple<IEnumerable<T>, long> Get(int currentPage, int pageSize, Expression<Func<T, bool>> condition = null)
        {
            if (condition == null) condition = _ => true;
            var query = Collection.Find<T>(condition);
            long totalCount = query.CountDocuments();
            List<T> records = query.SortByDescending(x => x.CreationDate).Skip((currentPage - 1) * pageSize).Limit(pageSize).ToList();
            return new Tuple<IEnumerable<T>, long>(records, totalCount);
        }

        public async Task<Tuple<IEnumerable<T>, long>> GetAsync(int currentPage, int pageSize, FilterDefinition<T> filter = null, SortDefinition<T> sort = null)
        {
            var query = filter == null ? Collection.Find<T>(_ => true) : Collection.Find<T>(filter);
            long totalCount = await query.CountDocumentsAsync();

            if (sort == null) sort = Builders<T>.Sort.Descending(x => x.CreationDate);
            List<T> records = await query.Sort(sort).Skip((currentPage - 1) * pageSize).Limit(pageSize).ToListAsync();
            return new Tuple<IEnumerable<T>, long>(records, totalCount);
        }
        public Tuple<IEnumerable<T>, long> Get(int currentPage, int pageSize, FilterDefinition<T> filter = null, SortDefinition<T> sort = null)
        {
            var query = filter == null ? Collection.Find<T>(_ => true) : Collection.Find<T>(filter);
            long totalCount = query.CountDocuments();

            if (sort == null) sort = Builders<T>.Sort.Descending(x => x.CreationDate);
            List<T> records = query.Sort(sort).Skip((currentPage - 1) * pageSize).Limit(pageSize).ToList();
            return new Tuple<IEnumerable<T>, long>(records, totalCount);
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> condition = null)
        {
            if (condition == null) condition = _ => true;
            var query = Collection.Find<T>(condition);
            List<T> records = query.SortByDescending(x => x.CreationDate).ToList();
            return records;
        }

        public IEnumerable<T> In<TField>(Expression<Func<T, TField>> field, IEnumerable<TField> values) where TField : class
        {
            var builder = Builders<T>.Filter;
            var filter = builder.In<TField>(field, values);
            var query = Collection.Find<T>(filter);
            List<T> records = query.SortByDescending(x => x.CreationDate).ToList();
            return records;
        }

        public virtual async Task<T> InsertAsync(T instance)
        {
            instance.Id = ObjectId.GenerateNewId();
            instance.CreationDate = DateTime.UtcNow;
            instance.UpdationDate = null;
            await Collection.InsertOneAsync(instance);
            return instance;
        }
        public virtual T Insert(T instance)
        {
            instance.Id = ObjectId.GenerateNewId();
            instance.CreationDate = DateTime.UtcNow;
            instance.UpdationDate = null;
            Collection.InsertOne(instance);
            return instance;
        }

        public virtual async Task<bool> UpdateAsync(T instance)
        {
            var query = await Collection.FindAsync<T>(x => x.Id == instance.Id);
            T _instance = await query.FirstOrDefaultAsync();
            if (_instance == null)
                throw new KeyNotFoundException($"Object with Id: '{instance.Id}' was not found.");
            else
            {
                instance.CreationDate = _instance.CreationDate;
                instance.UpdationDate = DateTime.UtcNow;
            }

            ReplaceOneResult result = await Collection.ReplaceOneAsync<T>(x => x.Id == instance.Id, instance);
            return result.ModifiedCount != 0;
        }
        public virtual bool Update(T instance)
        {
            var query = Collection.Find<T>(x => x.Id == instance.Id);
            T _instance = query.FirstOrDefault();
            if (_instance == null)
                throw new KeyNotFoundException($"Object with Id: '{instance.Id}' was not found.");
            else
            {
                instance.CreationDate = _instance.CreationDate;
                instance.UpdationDate = DateTime.UtcNow;
            }

            ReplaceOneResult result = Collection.ReplaceOne<T>(x => x.Id == instance.Id, instance);
            return result.ModifiedCount != 0;
        }

        public virtual async Task<bool> DeleteAsync(ObjectId id, bool logical = true)
        {
            var query = await Collection.FindAsync<T>(x => x.Id == id);
            T _instance = await query.FirstOrDefaultAsync();
            if (_instance == null)
                throw new KeyNotFoundException($"Object with Id: '{id}' was not found.");

            if (logical)
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(x => x.Deleted, true)
                    .Set(x => x.UpdationDate, DateTime.UtcNow);
                UpdateResult result = await Collection.UpdateOneAsync(x => x.Id == id, update);
                return result.ModifiedCount != 0;
            }
            else
            {
                DeleteResult result = await Collection.DeleteOneAsync(x => x.Id == id);
                return result.DeletedCount != 0;
            }
        }
        public virtual bool Delete(ObjectId id, bool logical = true)
        {
            var query = Collection.Find<T>(x => x.Id == id);
            T _instance = query.FirstOrDefault();
            if (_instance == null)
                throw new KeyNotFoundException($"Object with Id: '{id}' was not found.");

            if (logical)
            {
                UpdateDefinition<T> update = Builders<T>.Update
                    .Set(x => x.Deleted, true)
                    .Set(x => x.UpdationDate, DateTime.UtcNow);
                UpdateResult result = Collection.UpdateOne(x => x.Id == id, update);
                return result.ModifiedCount != 0;
            }
            else
            {
                DeleteResult result = Collection.DeleteOne(x => x.Id == id);
                return result.DeletedCount != 0;
            }
        }

        public async Task<long> CountAsync(Expression<Func<T, bool>> condition = null)
        {
            if (condition == null) condition = _ => true;
            return await Collection.CountDocumentsAsync(condition);
        }
        public long Count(Expression<Func<T, bool>> condition = null)
        {
            if (condition == null) condition = _ => true;
            return Collection.CountDocuments(condition);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> condition)
        {
            var result = await CountAsync(condition);
            return result > 0;
        }
        public bool Exists(Expression<Func<T, bool>> condition)
        {
            var result = Count(condition);
            return result > 0;
        }

        public void Dispose()
        {
            if (Context != null)
                Context.Dispose();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
