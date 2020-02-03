using System;

namespace TK.MongoDB.Models
{
    public abstract class BaseEntity<T>
    {
        /// <summary>
        /// Primary Key. Generates new <c>ObjectId</c> on insert.
        /// </summary>
        public T Id { get; set; }

        /// <summary>
        /// Soft delete. Defaults to <c>False</c> on insert.
        /// </summary>
        public bool Deleted { get; internal set; }

        /// <summary>
        /// Record created on date. Automatically sets <c>DateTime.UtcNow</c> on insert.
        /// </summary>
        public DateTime CreationDate { get; internal set; }

        /// <summary>
        /// Record updated on date. Defaults to <c>null</c> and automatically sets <c>DateTime.UtcNow</c> on update.
        /// </summary>
        public DateTime? UpdationDate { get; internal set; }
    }
}
