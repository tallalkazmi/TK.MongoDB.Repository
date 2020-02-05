using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TK.MongoDB.Models;

namespace TK.MongoDB
{
    public interface IRepository<T> : IDisposable where T : BaseEntity<ObjectId>
    {
        /// <summary>
        /// Initializes collection by dropping collection if exists.
        /// </summary>
        void InitCollection();

        /// <summary>
        /// Find single document by condition specified.
        /// </summary>
        /// <param name="condition">Lamda expression</param>
        /// <returns>Document</returns>
        Task<T> FindAsync(Expression<Func<T, bool>> condition);

        /// <summary>
        /// Gets document by Id.
        /// </summary>
        /// <param name="id">Key</param>
        /// <returns>Document</returns>
        Task<T> GetAsync(ObjectId id);

        /// <summary>
        /// Gets document by condition specified or gets all documents if condition is not passed. Paged records.
        /// </summary>
        /// <param name="currentPage">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="condition">Lamda expression</param>
        /// <returns>Tuple of records and total number of records</returns>
        Task<Tuple<IEnumerable<T>, long>> GetAsync(int currentPage, int pageSize, Expression<Func<T, bool>> condition = null);

        /// <summary>
        /// Inserts single record.
        /// </summary>
        /// <param name="instance">Document</param>
        /// <returns>Document</returns>
        Task<T> InsertAsync(T instance);

        /// <summary>
        /// Updates single record based on Id.
        /// </summary>
        /// <param name="instance">Document</param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T instance);

        /// <summary>
        /// Deletes record based on Id hard or soft based on logical value.
        /// </summary>
        /// <param name="id">Key</param>
        /// <param name="logical">Soft delete</param>
        /// <returns></returns>
        Task<bool> DeleteAsync(ObjectId id, bool logical = true);

        /// <summary>
        /// Counts documents based on condition specifed or counts all documents if condition is not passed.
        /// </summary>
        /// <param name="condition">Lamda expression</param>
        /// <returns></returns>
        Task<long> CountAsync(Expression<Func<T, bool>> condition = null);

        /// <summary>
        /// Checks if the document exists based on the condition specified.
        /// </summary>
        /// <param name="condition">Lamda expression</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> condition);
    }
}
