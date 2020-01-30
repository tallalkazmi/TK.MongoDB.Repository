using System;

namespace TK.MongoDB
{
    public interface IBaseEntity<T>
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        T Id { get; set; }
        
        /// <summary>
        /// Soft delete
        /// </summary>
        bool Deleted { get; set; }
        
        /// <summary>
        /// Created date
        /// </summary>
        DateTime CreationDate { get; set; }
    }
}
